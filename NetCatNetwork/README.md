This is a port of Apple's
[ImplementingNetcatWithNetworkFramework](https://developer.apple.com/documentation/network/implementing_netcat_with_network_framework)
sample code to C# and Xamarin.

This requires an unreleased version of Xamarin to try it out, for now,
your best bet is to build locally with the Network patches and edit
the Makefile to point to your installation directory.

You can build a self-contained executable like this:

```
$ make bundle
```

Which will produce the standalone `ncsharp` tool:


```
$ ./ncsharp
Network.framework-based 'netcat' app built with C#
nwcat [options] name [port]
  -4                         Use IPV4
  -6                         Use IPV6
  -b                         Use Bonjour
  -l, --listener             Create a listener to accept inbound connections
  -p, --port=VALUE           Use a local port for outbound connections
  -s, --localaddr=VALUE      Sets the local address for outbound connections
  -t, --tls                  Add TLS/DTLS as applicable
  -u, --udp                  Use UDP instead of TCP
  -v, --verbose              Verbose
  -k, --psk=VALUE            Specify the TLS Pre-Shared Key
  -h, --help                 Show this help
```

By default connections use TCP, if you want to use UDP, pass the `-u` flag.

The `-v` flag provides verbose logging of what is taking place

The "name" can either be a hostname resolved with DNS, an IPV4 or IPV6
address, or in the case of bonjour a Bonjour name.

The port can be either a number (like 80), or a name, like "http"

# Examples

## Connect to www.google.com http server

This shows how to use the tool connecting to port 80

```
$ make run COMMAND='www.google.com 80'
$
```

## Connect to www.google.com using https:

This shows how to use the tool connecting to the "https" port, we let
the Network framework sort out what it is:

```
$ make run COMMAND='-t www.google.com https'
$
```

## Using Bonjour

You can either connect to a Bonjour service, or listen on a name for a
bonjour service and do this over TCP or UDP.

To register a Bonjour service, specify the service name and the port
you want to bind it to.  For example to set the bonjour name to
migueldeicaza and bind this to port 9000 use this:

```
$ make run COMMAND='--listener -b migueldeicaza 9000'
```

To connect to it from another window, merely specify the name:

```
$ make run COMMAND='-b migueldeicaza'
```


# Debugging

While debugging and iterating, it is easier to use the Makefile target
with `make run COMMAND=args` where `args` is the set of arguments you
would like to pass to the C# version of Netcat.
