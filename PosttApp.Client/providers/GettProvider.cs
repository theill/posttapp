using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Net;
using System.Configuration;

namespace io.postt.providers {
  public class GettProvider {
    const string API_URL = "https://open.ge.tt/1";
    static GettProvider _instance;

    public static GettProvider Instance {
      get {
        if (_instance == null) {
          var gettApiToken = ConfigurationSettings.AppSettings["GettApiToken"];
          _instance = new GettProvider(gettApiToken);
        }

        return _instance;
      }
    }

    string apiKey;

    GettProvider(string apiKey) {
      this.apiKey = apiKey;
    }

    public void Authenticate(string username, string password, Action<Account> loggedIn, Action<string> loginFailed) {
      Login login = new Login { ApiKey = apiKey, Email = username, Password = password };

      string jsonPayload = string.Empty;
      using (MemoryStream ms = new MemoryStream()) {
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Login));
        jsonSerializer.WriteObject(ms, login);
        ms.Flush();
        jsonPayload = Encoding.UTF8.GetString(ms.ToArray());
      }

      _post<LoginResponse>(string.Format("{0}/users/login", API_URL), jsonPayload, (success) => {
        Console.WriteLine("Logged in and got {0}", success);

        if (loggedIn != null) {
          loggedIn.Invoke(new Account() {
            Email = username,
            Password = password,
            Fullname = success.User.Fullname,
            AccessToken = success.AccessToken
          }
          );
        }
        else {
          loginFailed.Invoke("Unable to find account with specified username/password");
        }
      }, (error) => {
        Console.WriteLine("Failed to login: {0}", error);
        loginFailed.Invoke(error);
      }
      );
    }

    public void CreateShare(string title, string accessToken, string filename, Action<string> urlCallback) {
      Console.WriteLine("CreateShare(title={0},accessToken={1},filename={2})", title, accessToken, filename);

      // TODO: consider 'title' (as payload)
      _post<Share>(string.Format("{0}/shares/create?accesstoken={1}", API_URL, accessToken), string.Empty, (success) => {
        Console.WriteLine("Shared created with {0}", success);

        CreateFile((success as Share).Name, filename, accessToken, urlCallback);
      }, (error) => {
        Console.WriteLine("Failed to create share: ", error);
      }
      );
    }

    public void CreateFile(string sharename, string filename, string accessToken, Action<string> urlCallback) {
      Console.WriteLine("CreateFile(sharename={0},filename={1},accessToken={2})", sharename, filename, accessToken);

      string jsonPayload = "{\"filename\": \"" + Path.GetFileName(filename).ToLower() + "\"}";
      _post<ShareFile>(string.Format("{0}/files/{1}/create?accesstoken={2}", API_URL, sharename, accessToken), jsonPayload, (success) => {
        Console.WriteLine("File sucessfully created: {0}", success);

        ShareFile sf = (success as ShareFile);

        if (urlCallback != null) {
          urlCallback(sf.GetUrl);
        }

        PushFile(sf.Upload.PutUrl, new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4 * 1024, false));

      }, (error) => {
        Console.WriteLine("Failed to create file: {0}", error);

      }
      );
    }

    public void PushFile(string url, FileStream fs) {
      Console.WriteLine("PushFile(url={0},fs={1})", url, fs.Length);

      _put(url, fs, (success) => {
        Console.WriteLine("File successfully pushed: {0}", success);

      }, (error) => {
        Console.WriteLine("Failed to push file: {0}", error);
      }
      );
    }

    void _get<T>(string uri, Action<T> completed, Action<string> failed) {
      HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
      req.Method = "GET";
      req.Accept = "application/json";

      Console.WriteLine("<= {0}", uri);
      req.BeginGetResponse(ar => {
        try {
          var rsp = (HttpWebResponse)req.EndGetResponse(ar);
          using (Stream s = rsp.GetResponseStream()) {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            completed.Invoke((T)jsonSerializer.ReadObject(s));
          }
        }
        catch (Exception x) {
          Console.WriteLine("Unable to GET from {0}", uri);
          Console.WriteLine(x);

          failed.Invoke(x.Message);
        }
      }, null);
    }

    void _post<T>(string uri, string jsonPayload, Action<T> completed, Action<string> failed) {
      Console.WriteLine("POST {0} => {1} ", uri, jsonPayload);

      // important :)
      System.Net.ServicePointManager.Expect100Continue = false;

      HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
      req.Method = "POST";
      req.Accept = "application/json";
      req.ContentType = "application/json; charset=utf-8";
      req.ContentLength = jsonPayload.Length;

      req.ReadWriteTimeout = int.MaxValue;
      req.Timeout = int.MaxValue;
      req.AllowWriteStreamBuffering = false;
      req.KeepAlive = false;

      req.BeginGetRequestStream((ar) => {
        HttpWebRequest request = (HttpWebRequest)ar.AsyncState;

        using (Stream s = request.EndGetRequestStream(ar)) {
          s.WriteTimeout = int.MaxValue;
          s.ReadTimeout = int.MaxValue;

          try {
            using (var sw = new StreamWriter(s)) {
              sw.Write(jsonPayload);
              sw.Flush();
            }
            s.Close();

            Console.WriteLine("<= {0}", uri);
            request.BeginGetResponse(ar2 => {
              try {
                Console.WriteLine("Trying to read response");
                var rsp = (HttpWebResponse)request.EndGetResponse(ar2);

                Console.WriteLine("Got back response: {0}", rsp.StatusCode);

                if (rsp.StatusCode == HttpStatusCode.OK) {
                  using (Stream responseStream = rsp.GetResponseStream()) {
                    Console.WriteLine("Got a response stream");
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T), new[] {
                      typeof(User),
                      typeof(Storage)
                    }
                    );
                    completed.Invoke((T)jsonSerializer.ReadObject(responseStream));
                  }
                }
                else {
                  Console.WriteLine("Det var ikke godt: {0}: {1}", rsp.StatusCode, rsp.StatusDescription);
                  failed.Invoke(rsp.StatusDescription);
                }
              }
              catch (WebException x) {
                Console.WriteLine("Unable to POST to {0}: {1}", uri, x.Message);
                Console.WriteLine(x);

                string failedMessage = "Failed";
                //                using (StreamReader sr = new StreamReader(x.Response.GetResponseStream())) {
                //                  Console.WriteLine("Got response (from bad request)");
                //                  failedMessage = sr.ReadToEnd();
                //                }

                failed.Invoke(failedMessage);
              }
            }, null);
          }
          catch (WebException x) {
            Console.WriteLine("Nah: x = {0}", x);

            //            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T), new [] { typeof(User), typeof(Storage) });
            //            completed.Invoke((T)jsonSerializer.ReadObject(x.Response.GetResponseStream()));
          }
        }
      }, req);
    }

    void _put(string uri, FileStream fs, Action<string> completed, Action<string> failed) {
      HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
      req.Method = "PUT";
      req.Accept = "application/json";

      req.ContentLength = fs.Length;

      Console.WriteLine("Writing entire file to stream, 2k at a time");
      req.ReadWriteTimeout = int.MaxValue;
      req.Timeout = int.MaxValue;
      //     req.SendChunked = true;
      req.AllowWriteStreamBuffering = false;
      req.KeepAlive = true;

      req.BeginGetRequestStream((ar) => {
        HttpWebRequest request = (HttpWebRequest)ar.AsyncState;

        using (Stream s = request.EndGetRequestStream(ar)) {
          s.WriteTimeout = int.MaxValue;
          s.ReadTimeout = int.MaxValue;

          try {
            Console.WriteLine("Seeking to begining");
            //            fs.Seek(0, SeekOrigin.Begin);
            //            Console.WriteLine("Copying...");
            //            fs.CopyTo(s);
            //            Console.WriteLine("Closing");
            //            fs.Close();
            //            Console.WriteLine("Closed!");

            int length = 1024 * 128;
            byte[] buffer = new byte[length];
            int bytesRead = 0;
            int totalBytes = 0;

            fs.Seek(0, SeekOrigin.Begin);
            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0) {
              Console.WriteLine("{0} of {1} bytes written (i.e. {2:0.00} percent)", totalBytes, req.ContentLength, (double)totalBytes / req.ContentLength * 100.0);
              s.Write(buffer, 0, bytesRead);
              s.Flush();
              totalBytes += bytesRead;

              // TODO: execute callbacks for partial uploads
            }
            fs.Close();

            Console.WriteLine("Got a total bytes of {0}", totalBytes);
          }
          catch (Exception x) {
            Console.WriteLine("Failed: {0}", x.Message);
            Console.WriteLine(x);

            failed.Invoke(x.Message);
            return;
          }

          Console.WriteLine("File written to stream");

          Console.WriteLine("<= {0}", uri);
          request.BeginGetResponse(ar2 => {
            try {
              var rsp = (HttpWebResponse)request.EndGetResponse(ar2);
              using (StreamReader sr = new StreamReader(rsp.GetResponseStream())) {
                Console.WriteLine("Got response");
                completed.Invoke(sr.ReadToEnd());
              }
            }
            catch (Exception x) {
              Console.WriteLine("Unable to PUT from {0}", uri);
              Console.WriteLine(x);

              failed.Invoke(x.Message);
            }
          }, null);
        }
      }, req);
    }
  }
}