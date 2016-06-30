using System;
using MonoMac.AppKit;

namespace com.posttapp {
  class MainClass {
    static void Main(string[] args) {
      Console.WriteLine("Initializing...");
      NSApplication.Init();
      NSApplication.Main(args);
    }
  }
}
