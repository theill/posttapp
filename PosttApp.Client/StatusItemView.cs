#region Using directives
using System;
using System.Drawing;

using MonoMac.AppKit;
using MonoMac.Foundation;

#endregion

namespace com.posttapp {
  public class StatusItemView : NSView {
    public class XYMenuDelegate : NSMenuDelegate {
      private StatusItemView view;

      public XYMenuDelegate(StatusItemView view) {
        this.view = view;
      }

      public override void MenuWillOpen(NSMenu menu) {
        Console.WriteLine("MenuWillOpen");
        view.isMenuVisible = true;
        view.NeedsDisplay = true;
      }

      public override void MenuWillHighlightItem(NSMenu menu, NSMenuItem menuItem) {
        Console.WriteLine("MenuWillHighlightItem");
      }

      public override void MenuDidClose(NSMenu menu) {
        Console.WriteLine("MenuDidClose");
        view.isMenuVisible = false;
        view.NeedsDisplay = true;
      }
    }

    private NSStatusItem parentStatusItem;
    private Action<string> dropped;
    private NSImage icon;
    private NSImage highlightedIcon;
    private bool isMenuVisible;

    public StatusItemView(NSStatusItem statusItem, Action<string> dropped) {
      Console.WriteLine("StatusItemView");
      this.dropped = dropped;

      icon = NSImage.ImageNamed("pin-black.png");
      highlightedIcon = NSImage.ImageNamed("pin-white.png");

      parentStatusItem = statusItem;
      parentStatusItem.Menu.Delegate = new XYMenuDelegate(this);

      RegisterForDraggedTypes(new string[] { NSPasteboard.NSFilenamesType, NSPasteboard.NSFileContentsType, NSPasteboard.NSStringType });
    }

    public override void DrawRect(RectangleF dirtyRect) {
      // http://undefinedvalue.com/2009/07/07/adding-custom-view-nsstatusitem
      parentStatusItem.DrawStatusBarBackgroundInRectwithHighlight(this.Bounds, isMenuVisible);

      NSImage drawnImage = isMenuVisible ? highlightedIcon : icon;

      RectangleF centeredRect = RectangleF.Empty;
      if (drawnImage != null) {
        // NSStatusBar.SystemStatusBar.Thickness == Bounds.Bottom? dunno
        centeredRect = new RectangleF((Bounds.Right - drawnImage.Size.Width) / 2, (Bounds.Bottom - drawnImage.Size.Height) / 2, drawnImage.Size.Width, drawnImage.Size.Height);
        drawnImage.Draw(centeredRect, RectangleF.Empty, NSCompositingOperation.SourceOver, 1f); // 1f is for full opacity
      }
    }

    [Export("draggingEntered:")]
    NSDragOperation DraggingEntered(NSDraggingInfo sender) {
      Console.WriteLine("DraggingEntered");
//       NSPasteboard pboard;
      return NSDragOperation.All;
    }

    [Export("performDragOperation:")]
    bool PerformDragOperation(NSDraggingInfo sender) {
      NSPasteboard pb = sender.DraggingPasteboard;

      foreach (var x in pb.PasteboardItems) {
        Console.WriteLine("x => {0}", x.Types);
        foreach (var y in x.Types) {
          Console.WriteLine("{0} => {1}", y, pb.GetStringForType(y));
        }
      }

      if (!string.IsNullOrEmpty(pb.GetStringForType("public.file-url"))) {
        if (dropped != null) {
          dropped.Invoke(pb.GetStringForType("public.file-url"));
        }

        return true;
      }

      Console.WriteLine("Got no string");
      return false;
    }

    public override void RightMouseDown(NSEvent theEvent) {
      MouseDown(theEvent);
    }

    public override void MouseDown(NSEvent theEvent) {
      parentStatusItem.PopUpStatusItemMenu(parentStatusItem.Menu);
      NeedsDisplay = true;
    }

    //- (BOOL)statusItemView:(BCStatusItemView *)view performDragOperation:(id <NSDraggingInfo>)info
//{
// NSPasteboard *pb = [info draggingPasteboard];
// if(![pb availableTypeFromArray:[NSArray arrayWithObjects:@"public.file-url", nil]])
//   return NSDragOperationNone;
//
// NSString *urlString = [[info draggingPasteboard] stringForType:@"public.file-url"];
// NSURL *url = [NSURL URLWithString:urlString];
// if([url isFileURL])
// {
//   [log debug:@"Enqueing dropped file: %@", [url path]];
//   [self processScreenshotAtPath:[url path] modifiedAtDate:[NSDate date]]; // we could use the file's modified date?
//   return YES;
// }
// return NO;
//}
  }
}