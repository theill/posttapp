#region Using directives
using System;
using System.Runtime.Serialization;

#endregion

namespace com.posttapp {
	[DataContract]
	public class ShareFile {
		[DataMember(Name="sharename")]
		public string ShareName {
			get;
			set;
		}

		[DataMember(Name="filename")]
		public string Filename {
			get;
			set;
		}

		[DataMember(Name="getturl")]
		public string GetUrl {
      get;
      set;
		}

    [DataMember(Name="upload")]
    public Upload Upload {
      get;
      set;
    }

		[DataMember(Name="fileid")]
		public string FileID {
			get;
			set;
		}

		[DataMember(Name="readystate")]
		public string ReadyState {
			get;
			set;
		}

		[DataMember(Name="created")]
		public int Created {
			get;
			set;
		}

		[DataMember(Name="downloads")]
		public int Downloads {
			get;
			set;
		}

		public override string ToString() {
			return string.Format("[ShareName={0}, Filename={1}, Upload={2}, GetUrl={3}, FileID={4}, ReadyState={5}, Created={6}, Downloaded={7}]", ShareName, Filename, Upload, GetUrl, FileID, ReadyState, Created, Downloads);
		}
	}
}