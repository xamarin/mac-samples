using System;

using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ScriptingBridge;

namespace ScriptingBridgeFinder
{
	public class FinderFile : NSObject
	{
		SBObject sbFile;
		
		public FinderFile (SBObject fileObject)
		{
			sbFile = fileObject;
		}
		
		[Export("name")]
		public string Name 
		{
			get { return sbFile.ValueForKey((NSString)"name") == null ? "" : sbFile.ValueForKey((NSString)"name").ToString();}
		}
		
		[Export("displayedName")]
		public string DisplayedName
		{
			get { return sbFile.ValueForKey((NSString)"displayedName") == null ? "" : sbFile.ValueForKey((NSString)"displayedName").ToString();}
		}
		
		internal SBObject FinderObject 
		{
			get { return sbFile;}	
		}
	}
}

