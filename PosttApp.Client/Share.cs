using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace com.posttapp {
	[DataContract]
	public class Share {
		[DataMember(Name="sharename")]
		public string Name {
			get;
			set;
		}

		[DataMember(Name="created")]
		public int Created {
			get;
			set;
		}

		[DataMember(Name="files")]
		public List<string> Files {
			get;
			set;
		}

		[DataMember(Name="live")]
		public bool Live {
			get;
			set;
		}

		public override string ToString() {
			return string.Format("[Name={0}, Created={1}, Files(count)={2}, Live={3}]", Name, Created, Files.Count, Live);
		}
	}
}