#region Using directives
using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
#endregion

namespace com.posttapp {
	class MainClass {
		static void Main(string [] args) {
      Console.WriteLine("Initializing...");
			NSApplication.Init();
			NSApplication.Main(args);
		}
	}
}	

