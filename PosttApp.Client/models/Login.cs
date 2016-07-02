using System.Runtime.Serialization;

namespace io.postt {
  [DataContract]
  public class Login {
    [DataMember(Name = "apikey")]
    public string ApiKey {
      get;
      set;
    }

    [DataMember(Name = "email")]
    public string Email {
      get;
      set;
    }

    [DataMember(Name = "password")]
    public string Password {
      get;
      set;
    }

    public override string ToString() {
      return string.Format("[ApiKey={0}, Email={1}, Password={2}]", ApiKey, Email, Password);
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
