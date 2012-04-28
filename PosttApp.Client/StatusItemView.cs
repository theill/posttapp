#region Using directives
using System;
using System.Drawing;

using MonoMac.AppKit;
using MonoMac.Foundation;
#endregion

namespace com.posttapp {
	public class StatusItemView : NSView {
		private NSStatusItem parentStatusItem;
		private NSImage image;
		private Action<string> dropped;

		public StatusItemView(NSStatusItem statusItem, Action<string> dropped) {
			this.dropped = dropped;

			float length = statusItem.Length == -1 ? 32.0f : statusItem.Length;
			RectangleF frame = new RectangleF(0, 0, length, NSStatusBar.SystemStatusBar.Thickness);

			parentStatusItem = statusItem;
			image = statusItem.Image;
		}

		public override void DrawRect(RectangleF dirtyRect) {
//      base.DrawRect(dirtyRect);
      Console.WriteLine("Draw!");

      // http://undefinedvalue.com/2009/07/07/adding-custom-view-nsstatusitem
      parentStatusItem.DrawStatusBarBackgroundInRectwithHighlight(this.Bounds, isMenuVisible);

      NSImage drawnImage = image;

      RectangleF centeredRect = RectangleF.Empty;
      if (drawnImage != null) {
        centeredRect = new RectangleF(0, 0, drawnImage.Size.Width, drawnImage.Size.Height);
        // centeredRect = NSIntegralRect(centeredRect);
        centeredRect.Y = ((this.Bounds.Right - this.Bounds.Left) / 2) - (drawnImage.Size.Height / 2);

        drawnImage.Draw(centeredRect, RectangleF.Empty, NSCompositingOperation.SourceOver, 1.0f);
      }
    }

		[Export("draggingEntered:")]
		NSDragOperation DraggingEntered(NSDraggingInfo sender) {
//				NSPasteboard pboard;
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

    private bool isMenuVisible;

    public override void RightMouseDown(NSEvent theEvent) {
      MouseDown(theEvent);
    }

    public override void MouseDown(NSEvent theEvent) {
      Console.WriteLine("den er god");

      if (isMenuVisible) {
        isMenuVisible = false;
      }
      else {
        isMenuVisible = true;
        parentStatusItem.PopUpStatusItemMenu(parentStatusItem.Menu);
      }

      NeedsDisplay = true;
    }

		//- (BOOL)statusItemView:(BCStatusItemView *)view performDragOperation:(id <NSDraggingInfo>)info
//{
//	NSPasteboard *pb = [info draggingPasteboard];
//	if(![pb availableTypeFromArray:[NSArray arrayWithObjects:@"public.file-url", nil]])
//		return NSDragOperationNone;
//
//	NSString *urlString = [[info draggingPasteboard] stringForType:@"public.file-url"];
//	NSURL *url = [NSURL URLWithString:urlString];
//	if([url isFileURL])
//	{
//		[log debug:@"Enqueing dropped file: %@", [url path]];
//		[self processScreenshotAtPath:[url path] modifiedAtDate:[NSDate date]]; // we could use the file's modified date?
//		return YES;
//	}
//	return NO;
//}
	}
}