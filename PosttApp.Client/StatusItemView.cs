using System;
using System.Web;
using System.Drawing;

using MonoMac.AppKit;
using MonoMac.Foundation;

namespace io.postt.macos {
  public class XYMenuDelegate : NSMenuDelegate {
    readonly StatusItemView view;

    public XYMenuDelegate(StatusItemView view) {
      this.view = view;
    }

    public override void MenuWillOpen(NSMenu menu) {
      view.IsMenuVisible = true;
      view.NeedsDisplay = true;
    }

    public override void MenuWillHighlightItem(NSMenu menu, NSMenuItem item) {

    }

    public override void MenuDidClose(NSMenu menu) {
      view.IsMenuVisible = false;
      view.NeedsDisplay = true;
    }
  }

  public class StatusItemView : NSView {

    static NSStatusItem parentStatusItem;
    Action<string> dropped;
    NSImage icon;
    NSImage highlightedIcon;
    public bool IsMenuVisible;

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
      Console.WriteLine("DrawRect(dirtyRect={0})", dirtyRect);
      // http://undefinedvalue.com/2009/07/07/adding-custom-view-nsstatusitem
      parentStatusItem.DrawStatusBarBackground(this.Bounds, IsMenuVisible);

      NSImage drawnImage = IsMenuVisible ? highlightedIcon : icon;

      RectangleF centeredRect = RectangleF.Empty;
      if (drawnImage != null) {
        // NSStatusBar.SystemStatusBar.Thickness == Bounds.Bottom? dunno
        centeredRect = new RectangleF((Bounds.Right - drawnImage.Size.Width) / 2, (Bounds.Bottom - drawnImage.Size.Height) / 2, drawnImage.Size.Width, drawnImage.Size.Height);
        drawnImage.Draw(centeredRect, RectangleF.Empty, NSCompositingOperation.SourceOver, 1f); // 1f is for full opacity
      }
    }

    [Export("draggingEntered:")]
    public override NSDragOperation DraggingEntered(NSDraggingInfo sender) {
      // display "copy" icon when dragging over area
      return NSDragOperation.Copy;
    }

    [Export("performDragOperation:")]
    public override bool PerformDragOperation(NSDraggingInfo sender) {
      NSPasteboard pb = sender.DraggingPasteboard;

      foreach (var x in pb.PasteboardItems) {
        Console.WriteLine("x => {0}", x.Types);
        foreach (var y in x.Types) {
          Console.WriteLine("{0} => {1}", y, pb.GetStringForType(y));
        }
      }

      var file = pb.GetStringForType("public.file-url");
      if (!string.IsNullOrEmpty(file)) {
        if (dropped != null) {
          file = HttpUtility.UrlDecode(file);
          dropped.Invoke(file);
        }

        return true;
      }

      Console.WriteLine("Got no string");
      return false;
    }

    public override void RightMouseDown(NSEvent theEvent) {
      base.RightMouseDown(theEvent);
      MouseDown(theEvent);
    }

    public override void MouseDown(NSEvent theEvent) {
      base.MouseDown(theEvent);
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