// mcs nwncsharp.cs /cvs/mono/mcs/class/Mono.Options/Mono.Options/Options.cs -r:/cvs/mt/xamarin-macios/src/build/mac/full-64/Xamarin.Mac.dll  && MONO_PATH=/cvs/mt/xamarin-macios/src/build/mac/full-64/ DYLD_LIBRARY_PATH=/cvs/mt/xamarin-macios/runtime/.libs/mac/ mono --debug nwncsharp.exe  www.google.com 80
//
// for listener:
// mcs nwncsharp.cs /cvs/mono/mcs/class/Mono.Options/Mono.Options/Options.cs -r:/cvs/mt/xamarin-macios/src/build/mac/full-64/Xamarin.Mac.dll  && MONO_PATH=/cvs/mt/xamarin-macios/src/build/mac/full-64/ DYLD_LIBRARY_PATH=/cvs/mt/xamarin-macios/runtime/.libs/mac/ mono --debug nwncsharp.exe -v  -l 8000
// then netcat to port 8000 from another window

using Network;
using AppKit;
using System;
using Security;
using Mono.Options;
using System.Linq;
using System.IO;
using CoreFoundation;
using System.Threading;
using System.Runtime.InteropServices;

class X {
	static bool ipv4, ipv6;
	static bool wantListener;		// Create a listener
	static bool verbose;		// verbose
	static bool bonjour;
	static bool useTls;
	static bool useUdp;
	static bool detached;   	// Ignore stdin
	static string localPort, localAddr, psk;
	static NWConnection inboundConnection;
	
	static OptionSet options =
		new OptionSet () {
			{ "4" , "Use IPV4", v => ipv4 = true },
			{ "6" , "Use IPV6", v => ipv6 = true },
			{ "b", "Use Bonjour", v => bonjour = true },
			{ "l|listener", "Create a listener to accept inbound connections", v => wantListener = true },
			{ "p=|port=", "Use a local port for outbound connections", v => localPort = v },
			{ "s=|localaddr=", "Sets the local address for outbound connections", v=> localAddr = v },
			{ "t|tls", "Add TLS/DTLS as applicable", v => useTls = true },
			{ "u|udp", "Use UDP instead of TCP", v => useUdp = true },
			{ "v|verbose", "Verbose", v => verbose = true },
			{ "k=|psk=", "Specify the TLS Pre-Shared Key", v => psk = v },
			{ "h|help", "Show this help", v => ShowHelp (0, true) },
	};

	static void ShowHelp (int status = 1, bool showBanner = false)
	{
		if (showBanner)
			Console.Error.WriteLine (
				"Network.framework-based 'netcat' app built with C#\n" +
				"nwcat [options] name [port]"
				);
		options.WriteOptionDescriptions(Console.Error);
		Environment.Exit (status);
	}

	static void warn (string msg) => Console.Error.WriteLine ("=> " + msg);
	static void err (string msg) { Console.Error.WriteLine (msg); Environment.Exit (1); }
	
	static int Main (string [] args)
	{
		var rest = options.Parse (args);
		string hostname, port;
		
		// Validate options
		if (rest.Count == 0){
			if (wantListener)
				warn ("Missing port with option -l");
			if (bonjour)
				warn ("Missing bonjour name -b");
			ShowHelp ();
			return 1;
		} else if (rest.Count == 1){
			if (!wantListener && !bonjour){
				warn ("Missing hostname and port");
				ShowHelp ();
			}
			if (bonjour){
				hostname = rest [0];
				port = null;
			} else {
				hostname = null;
				port = rest [0];
			}
		} else {
			if (!wantListener && bonjour){
				warn ("Cannot set port for non-listening bonjour connection");
				ShowHelp ();
			}
			hostname = rest [0];
			port = rest [1];
		}

		if (wantListener){
			if (localAddr != null)
				err ("Cannot use -s and -l");
			if (localPort != null)
				err ("Cannot use -p and -l");
			if (useTls && psk == null)
				err ("Must use -k if both -t and -l are specified");
		}
		if (psk != null && !useTls)
			err ("must use -t with -k");

		NSApplication.Init ();
		if (wantListener){
			using (var listener = CreateAndStartListener (hostname, port)){
				if (listener == null)
					err ($"Cannot create a listener on {hostname}:{port}");
				DispatchQueue.MainIteration ();
			}
		} else {
			using (var connection = CreateOutboundConnection (hostname, port)){
				if (connection == null)
					err ($"Cannot create connection to {hostname}:{port}");
				StartConnection (connection);
				StartSendReceiveLoop (connection);
				DispatchQueue.MainIteration ();
			}
		}
		
			
		return 0;
	}

	//
	// Returns a block that can configure Tls as we desire, or uses the default based on the user settings
	//
	static Action<NWProtocolOptions> SetupTls ()
	{
		if (useTls && psk != null){
			return (NWProtocolOptions options) => {
				var secOptions = options.TlsProtocolOptions;
				var pskData = DispatchData.FromByteBuffer (System.Text.Encoding.UTF8.GetBytes (psk));
				secOptions.AddPreSharedKey (pskData);
				secOptions.AddTlsCipherSuite (SslCipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256);
			};
		}

		// Use default TLS options.
		return null;
	}

	// Returns the Bonjour service type based on whether we are using tcp or udp
	static string GetServiceType () => useUdp ? "_nwcat._udp" : "_nwcat._tcp";

	static NWListener CreateAndStartListener (string host, string port)
	{
		Action<NWProtocolOptions> configureTls = SetupTls ();
		NWParameters parameters;
		
		// Create the parameters, either TLS or no TLS, and with UDP or no UDP
		if (useUdp){
			if (useTls)
				parameters = NWParameters.CreateSecureUdp (configureTls: configureTls, configureUdp: null);
			else
				parameters = NWParameters.CreateUdp (configureUdp: null);
		} else {
			if (useTls)
				parameters = NWParameters.CreateSecureTcp (configureTls: configureTls, configureTcp: null);
			else
				parameters = NWParameters.CreateTcp (configureTcp: null);
		}

		// If specified, set the IP version
		using (NWProtocolStack protocolStack = parameters.ProtocolStack){
			if (ipv4 || ipv6){
				NWProtocolOptions ipOptions = protocolStack.InternetProtocol;
				ipOptions.IPSetVersion (ipv4 ? NWIPVersion.Version4 : NWIPVersion.Version6);
			}
		}

		// Bind to local address and port
		string address = bonjour ? null : host;
		if (address != null || port != null){
			NWEndpoint localEndpoint = NWEndpoint.Create (address != null ? address : "::", port != null ? port : "0");
			Console.WriteLine ("Getting {0} and {1}", address != null ? address : "::", port != null ? port : "0");
			parameters.LocalEndpoint = localEndpoint;
			Console.WriteLine ("With port: " + localEndpoint.Port);
		}

		var listener = NWListener.Create (parameters);

		if (bonjour && host != null){
			listener.SetAdvertiseDescriptor (NWAdvertiseDescriptor.CreateBonjourService (host, GetServiceType (), "local"));
			listener.SetAdvertisedEndpointChangedHandler ((NWEndpoint advertisedEndpoint, bool added) => {
				if (verbose){
					var astr = added ? "added" : "removed";
					warn ($"Listener {astr} on {advertisedEndpoint.BonjourServiceName} on ({advertisedEndpoint.BonjourServiceName}.{GetServiceType()}.local");
				}
			});
		}

		listener.SetQueue (DispatchQueue.MainQueue);
		listener.SetStateChangedHandler ((listenerState,error)=>{
			var errno = (SslStatus) (error == null ? 0 : error.ErrorCode);
			switch (listenerState){
			case NWListenerState.Waiting:
				if (verbose)
					warn ($"Listener on port {listener.Port} udp={useUdp} waiting");
				break;
			case NWListenerState.Failed:
				warn ($"Listener on port {listener.Port} udp={useUdp} failed");
				break;
			case NWListenerState.Ready:
				if (verbose)
					warn ($"Listener on port {listener.Port} udp={useUdp} ready");
				break;
			case NWListenerState.Cancelled:
				listener = null;
				break;
			}
		});

		listener.SetNewConnectionHandler ((connection)=>{
			if (inboundConnection != null){
				// We only support one connection at a time, so if we already
				// have one, reject the incoming connection.
				connection.Cancel ();
			} else {
				if (verbose)
					warn ($"New Connection on {connection.Handle} with {connection.Endpoint}");
				// Accept the incoming connection and start sending  and receiving on it
				inboundConnection = connection;
				StartConnection (inboundConnection);
				StartSendReceiveLoop (inboundConnection);
			}
		});
		listener.Start ();

		return listener;
	}
	
	static NWConnection CreateOutboundConnection (string name, string port)
	{
		NWEndpoint endpoint;
		
		if (bonjour){
			endpoint = NWEndpoint.CreateBonjourService (name, GetServiceType (), "local");
		} else {
			endpoint = NWEndpoint.Create (name, port);
		}

		Action<NWProtocolOptions> configureTls = SetupTls ();
		NWParameters parameters;

		if (useUdp){
			if (useTls)
				parameters = NWParameters.CreateSecureUdp (configureTls: configureTls, configureUdp: null);
			else
				parameters = NWParameters.CreateUdp (configureUdp: null);
		} else {
			if (useTls)
				parameters = NWParameters.CreateSecureTcp (configureTls: configureTls, configureTcp: null);
			else
				parameters = NWParameters.CreateTcp (configureTcp: null);
		}

		using (NWProtocolStack protocolStack = parameters.ProtocolStack){
			if (ipv4 || ipv6){
				NWProtocolOptions ipOptions = protocolStack.InternetProtocol;
				ipOptions.IPSetVersion (ipv4 ? NWIPVersion.Version4 : NWIPVersion.Version6);
			}
		}
		
		if (localAddr != null || localPort != null){
			using (NWEndpoint localEndpoint = NWEndpoint.Create (localAddr != null ? localAddr : "::", port == null ? "0" : port))
				parameters.LocalEndpoint = localEndpoint;
		}

		var connection = new NWConnection (endpoint, parameters);

		endpoint.Dispose ();
		parameters.Dispose ();
		return connection;
	}

	static void StartConnection (NWConnection connection)
	{
		connection.SetQueue (DispatchQueue.MainQueue);
		warn ($"Start Connection on {connection.Handle} with {connection.Endpoint}");
		
		connection.SetStateChangeHandler ((state, error) => {
			var remote = connection.Endpoint;
			var errno = (SslStatus)(error != null ? error.ErrorCode : 0);
			switch (state){
			case NWConnectionState.Waiting:
				warn ($"Connect to {remote.Hostname} port {remote.Port} udp={useUdp} failed, is waiting");
				break;
			case NWConnectionState.Failed:
				warn ($"Connect to {remote.Hostname} port {remote.Port} udp={useUdp} failed, error {errno}");
				break;
			case NWConnectionState.Ready:
				if (verbose)
					warn ($"Connect to {remote.Hostname} port {remote.Port} udp={useUdp} succeeded");
				break;
			case NWConnectionState.Cancelled:
				// Release reference
				connection = null;
				break;
				
			}
		});
		connection.Start ();
	}

	static void SendLoop (NWConnection connection)
	{
		const int STDIN_FILENO = 0;
		
		DispatchIO.Read (STDIN_FILENO, 8192, DispatchQueue.MainQueue, (DispatchData readData, int stdinError)=>{
			if (stdinError != 0)
				warn ($"Standard input error: {stdinError}");
			else if (readData == null|| readData.Size == 0){
				// Null data represent EOF
				// Send a "write close" on the connection, by sending
				// null data with the final message context marked as
				// complete.  Note that it is valid to send with null
				// data but a non-null context.
				connection.Send ((byte [] )null, context: NWContentContext.FinalMessage, isComplete: true, callback: (NWError error) => {
					if (error != null)
						warn ($"send error: {error.ErrorCode}");
				});
				// Stop reading from stdin, do not schedule another SendLoop
			} else {
				connection.Send (readData, context: NWContentContext.DefaultMessage, isComplete: true, callback: (NWError error) => {
					if (error != null)
						warn ($"send error: {error.ErrorCode}");
					else {
						// continue reading from stdin
						SendLoop (connection);
					}
				});
			}
			
		});
	}

	static void ReceiveLoop (NWConnection connection)
	{
		connection.ReceiveData (1, uint.MaxValue, (DispatchData dispatchData, NWContentContext context, bool isComplete, NWError error)=>{
			Action scheduleNext = () => {
				// If the context is marked as complete, and is the final context,
				// we're read-closed.
				if (isComplete) {
					if (context != null && context.IsFinal){
						if (verbose)
							warn ("Exiting because isComplete && context.IsFinal");
						Environment.Exit (0);
					}
					if (dispatchData == null) {
						if (verbose)
							warn ($"Exiting because isComplete && data == zero;  error={error}");
						Environment.Exit (0);
					}
				}
				if (error == null)
					ReceiveLoop (connection);
					
			};
			if (dispatchData != null){
				const int STDOUT_FILENO = 1;
				DispatchIO.Write (STDOUT_FILENO, dispatchData, DispatchQueue.MainQueue, (data, stdoutError) => {
					if (stdoutError != 0)
						warn ("stdout write error");
					scheduleNext ();
				});
			} else 
				scheduleNext ();
		});
	}
	
	static void StartSendReceiveLoop (NWConnection connection)
	{
		// Start reading from stdin
		if (!detached)
			SendLoop (connection); 

		if (verbose)
			warn ("Start Receive Loop");
		
		// Start reading from the connection
		ReceiveLoop (connection);
	}
	
}
