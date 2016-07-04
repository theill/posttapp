using System;
using System.Linq;

using MonoMac.Foundation;
using MonoMac.AppKit;

using io.postt.macos.providers;
using System.Configuration;

namespace io.postt.macos {
	public partial class AppDelegate : NSApplicationDelegate {
		MainWindowController mainWindowController;
		NSStatusItem mainItem;

		public Account Account = new Account();

		NSMetadataQuery _query;

		public override void DidFinishLaunching(NSNotification notification) {
			Console.WriteLine("FinishedLaunching(notification={0})", notification);

			mainWindowController = new MainWindowController();

			InitializeStatusBar();
			//InitializeFirebase();

			_query = new NSMetadataQuery();
			_query.Predicate = NSPredicate.FromFormat("kMDItemIsScreenCapture = 1");

			NSNotificationCenter.DefaultCenter.AddObserver(NSMetadataQuery.DidStartGatheringNotification, ScreenCaptureChange, _query);
			NSNotificationCenter.DefaultCenter.AddObserver(NSMetadataQuery.DidUpdateNotification, ScreenCaptureChange, _query);
			NSNotificationCenter.DefaultCenter.AddObserver(NSMetadataQuery.DidFinishGatheringNotification, ScreenCaptureChange, _query);

			_query.StartQuery();
		}

		void ScreenCaptureChange(NSNotification notification) {
			Console.WriteLine("The " + notification.Name + " message was posted");
			if (notification.Name == "NSMetadataQueryDidUpdateNotification") {
				NSMetadataQuery query = notification.Object as NSMetadataQuery;
				var latestItem = query != null ? query.Results.LastOrDefault() : null;
				if (latestItem != null) {
					ItemDropped(latestItem.Path);
				}
			}
		}

		public override void WillTerminate(NSNotification notification) {
			_query.StopQuery();
			_query.Delegate = null;
			_query = null;
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender) {
			return false;
		}

		void InitializeStatusBar() {
			// creating the status item with a length of -2 is equivalent to the call
			// [[NSStatusBar systemStatusBar] statusItemWithLength:NSSquareStatusItemLength]
			mainItem = NSStatusBar.SystemStatusBar.CreateStatusItem(-2);
			mainItem.HighlightMode = true;
			mainItem.Menu = menu;
			mainItem.View = new StatusItemView(mainItem, ItemDropped);
		}

		void InitializeFirebase() {
			var firebase = new Firebase.Xamarin.FirebaseClient<Account>("https://project-2230474106662854153.firebaseio.com/", "OGL4xQePUQxGFfQCMy5bzIEkYvKtIiPnFCjiKzMX");

			Console.WriteLine("write");
			var item = firebase.Child("accounts").PostAsync(new Account { Email = "svend" });
			Console.WriteLine("done");
			//Console.WriteLine("item: " + item.Key);
			//Console.WriteLine("all done");

			// add new item to list of data 
			//var item = await firebase
			//	.Child("yourentity")
			//	.PostAsync(new YourObject());

			//var config = new FirebaseConfig {
			//	AuthSecret = "OGL4xQePUQxGFfQCMy5bzIEkYvKtIiPnFCjiKzMX",
			//	BasePath = "https://project-2230474106662854153.firebaseio.com/"
			//};

			//var client = new FirebaseClient(config);

			//var rsp = client.SetAsync("/x/y/z", new { foo = "bar" });
			//var xx = rsp.Result;
			//Console.WriteLine("we have: " + xx);
		}

		void ItemDropped(string item) {
			Console.WriteLine("Item dropped: {0}", item);

			//var provider = ProviderFactory.GetInstance();
			//if (provider.IsAuthenticated) {

			//}

			var provider = new FtpProvider();
			if (provider.Authenticated) {
				Console.WriteLine("Uploading...");
				provider.Upload(item, "asdf", (url) => {
					Console.WriteLine("very nice => " + url);
				});
			}
			else {
				Console.WriteLine("authenticating...");
				var username = ConfigurationSettings.AppSettings["FtpUsername"];
				var password = ConfigurationSettings.AppSettings["FtpPassword"];
				provider.Authenticate(username, password, account => {
					Console.WriteLine("cool. got: " + account);
					var fileInfo = new System.IO.FileInfo(item);
					provider.Upload(item, fileInfo.Name, (url) => {
						Console.WriteLine("very nice => " + url);
						BeginInvokeOnMainThread(() => {
							// placing link into clipboard
							NSPasteboard pb = NSPasteboard.GeneralPasteboard;
							pb.DeclareTypes(new string[] { NSPasteboard.NSStringType }, null);
							pb.SetStringForType(url, NSPasteboard.NSStringType);
							Console.WriteLine("File will be available for download from {0}", url);

							var sound = NSSound.FromName("Pop");
							sound.Play();
						});
					});
				}, err => {
					Console.WriteLine("not cool -- failed with: " + err);
				});
			}


			//if (Account.IsAuthenticated) {
			//	CreateShare(Account, item.Replace("file://localhost", "").Replace("%20", " "));
			//}
			//else {
			//	GettProvider.Instance.Authenticate(Account.Email, Account.Password, (account) => {
			//		// user is logged in

			//		// store retrieved access token for future requests
			//		Account.AccessToken = account.AccessToken;

			//		CreateShare(account, item.Replace("file://localhost", "").Replace("%20", " "));
			//	}, (error) => {
			//		// TODO: handle error message when uploading
			//		Console.WriteLine("Got an error: {0}", error);
			//	});
			//}
		}

		private void CreateShare(Account account, string path) {
			GettProvider.Instance.CreateShare(
				"test1",
				account.AccessToken,
				path,
				(url) => {
					BeginInvokeOnMainThread(() => {
						// placing link into clipboard
						NSPasteboard pb = NSPasteboard.GeneralPasteboard;
						pb.DeclareTypes(new string[] { NSPasteboard.NSStringType }, null);
						pb.SetStringForType(url, NSPasteboard.NSStringType);
						Console.WriteLine("File will be available for download from {0}", url);
					});
				}
			);
		}

		partial void launchGettWebsiteClicked(MonoMac.Foundation.NSObject sender) {
			Console.WriteLine("Open ge.tt...");
			NSWorkspace.SharedWorkspace.OpenUrl(NSUrl.FromString("http://ge.tt/"));
		}

		partial void preferencesClicked(MonoMac.Foundation.NSObject sender) {
			Console.WriteLine("Open Preferences...");

			NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
			mainWindowController.Window.Activate();
		}

		partial void helpClicked(MonoMac.Foundation.NSObject sender) {
			Console.WriteLine("Open help section...");
			NSWorkspace.SharedWorkspace.OpenUrl(NSUrl.FromString("https://postt.io/help"));
		}

		partial void quitClicked(MonoMac.Foundation.NSObject sender) {
			Console.WriteLine("Quit application");
			NSApplication.SharedApplication.Terminate(this);
		}
	}
}