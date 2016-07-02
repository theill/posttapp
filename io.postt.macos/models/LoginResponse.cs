using System.Runtime.Serialization;

namespace io.postt.macos {
  [DataContract]
  public class LoginResponse {
    [DataMember(Name = "accesstoken")]
    public string AccessToken {
      get;
      set;
    }

    [DataMember(Name = "refreshtoken")]
    public string RefreshToken {
      get;
      set;
    }

    [DataMember(Name = "expires")]
    public long Expires {
      get;
      set;
    }

    [DataMember(Name = "user")]
    public User User {
      get;
      set;
    }

    public override string ToString() {
      return string.Format("[AccessToken={0}, RefreshToken={1}, Expires={2}, User={3}]", AccessToken, RefreshToken, Expires, User);
    }
  }
}
/*
    public string ToJson() {
     JsonObject w = new JsonObject();
     if (this.ID > 0) {
       w.Add("id", new JsonPrimitive(this.ID));
     }
     if (this.LanguageID > 0) {
       w.Add("language_id", new JsonPrimitive(this.LanguageID));
     }
     if (this.Name != null) {
       w.Add("name", new JsonPrimitive(this.Name));
     }
     w.Add("current_level", new JsonPrimitive(this.CurrentLevel));
     w.Add("height", new JsonPrimitive(this.Height));
     w.Add("weight", new JsonPrimitive(this.Weight));
     w.Add("birth_year", new JsonPrimitive(this.BirthYear));
     w.Add("gender", new JsonPrimitive(this.Gender));
     w.Add("points", new JsonPrimitive(this.Points));
     if (FacebookAccessToken != null) {
       w.Add("provider", new JsonPrimitive("facebook"));
       w.Add("uid", new JsonPrimitive(FacebookUid));
       w.Add("access_token", new JsonPrimitive(FacebookAccessToken));
     }

     JsonObject root = new JsonObject();
     root.Add("user", w);
     return root.ToString();
   }
*/
