using System;
using System.IO;
using System.Net;
using System.Text;

namespace io.postt.macos.providers {
	public class FtpProvider : IPosttProvider {
		bool _authenticated;
		string username;
		string password;

		public bool Authenticated {
			get {
				return _authenticated;
			}
		}

		public void Authenticate(string username, string password, Action<Account> loggedIn, Action<string> loginFailed) {
			this.username = username;
			this.password = password;

			try {
				var ftpClient = (FtpWebRequest)WebRequest.Create("ftp://www.theill.com/");
				ftpClient.Credentials = new System.Net.NetworkCredential(username, password);
				ftpClient.Method = System.Net.WebRequestMethods.Ftp.PrintWorkingDirectory;
				var wr = ftpClient.GetResponseAsync().Result;
				Console.WriteLine("Got back: " + wr);

				ftpClient.GetResponseAsync().ContinueWith((y) => {
					try {
						Console.WriteLine("ok, got response async: faulted: {0}, canceled: {1}, completed: {2}", y.IsFaulted, y.IsCanceled, y.IsCompleted);

						if (y.IsFaulted) {
							Console.WriteLine("Failed to log in");
							loginFailed("Failed");
						}

						Console.WriteLine("Success with log in");
						_authenticated = true;
						loggedIn(new Account { Email = username, Password = password });
					}
					catch (Exception ix) {
						Console.WriteLine("got: " + ix.Message);
					}
				});
			}
			catch (Exception x) {
				Console.WriteLine("Not good, we got: " + x.Message);
			}
		}

		public void Upload(string path, string filename, Action<string> urlCallback) {
			FtpWebRequest ftpClient = (FtpWebRequest)WebRequest.Create("ftp://www.theill.com/http/stuff/screenshots/" + filename);
			ftpClient.Credentials = new System.Net.NetworkCredential(this.username, this.password);
			ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
			ftpClient.UseBinary = true;
			//ftpClient.KeepAlive = true;
			System.IO.FileInfo fi = new System.IO.FileInfo(path + filename);
			ftpClient.ContentLength = fi.Length;
			byte[] buffer = new byte[4097];
			int bytes = 0;
			int total_bytes = (int)fi.Length;
			System.IO.FileStream fs = fi.OpenRead();
			System.IO.Stream rs = ftpClient.GetRequestStream();
			while (total_bytes > 0) {
				bytes = fs.Read(buffer, 0, buffer.Length);
				rs.Write(buffer, 0, bytes);
				total_bytes = total_bytes - bytes;
			}
			//fs.Flush();
			fs.Close();
			rs.Close();
			FtpWebResponse uploadResponse = (FtpWebResponse)ftpClient.GetResponse();
			var x = uploadResponse.StatusDescription;
			uploadResponse.Close();

			Console.WriteLine("Got back " + x);

			//var request = (FtpWebRequest)WebRequest.Create("ftp://www.theill.com/stuff/screenshots");
			//request.Method = WebRequestMethods.Ftp.UploadFile;
			//request.UseBinary = true;
			//request.UsePassive = true;

			//// This example assumes the FTP site uses anonymous logon.
			//request.Credentials = new NetworkCredential(this.username, this.password);

			//// Copy the contents of the file to the request stream.
			//var sourceStream = new StreamReader(filename);
			//byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
			//sourceStream.Close();
			//request.ContentLength = fileContents.Length;

			//Console.WriteLine("File read .. starting to upload");
			//var requestStream = request.GetRequestStream();
			//requestStream.Write(fileContents, 0, fileContents.Length);
			//requestStream.Close();

			//var response = (FtpWebResponse)request.GetResponse();

			//Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

			//response.Close();

			if (urlCallback != null) {
				urlCallback("done");
			}
		}
	}
}

