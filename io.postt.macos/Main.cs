using System;
using MonoMac.AppKit;

namespace io.postt.macos {
  class MainClass {
    static void Main(string[] args) {
      Console.WriteLine("Initializing...");
      NSApplication.Init();
      NSApplication.Main(args);
    }
  }
}
