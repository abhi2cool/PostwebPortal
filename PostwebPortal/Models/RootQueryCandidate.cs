using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PostwebPortal.Models
{
    public class RootQueryCandidate
    {
		[Key]
		public string queryid { get; set; }
		public string url { get; set; }
		public string pwquery { get; set; }
		public long impression { get; set; }
		public Nullable<DateTime> lastmodifiedon { get; set; }
		public string lastmodifiedby { get; set; }
		public string selectedanswerid { get; set; }





	}
}
