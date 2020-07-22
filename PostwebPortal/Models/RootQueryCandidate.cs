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
		public string answerid { get; set; }
		[Key]
		public string pwquery { get; set; }
		public long impression { get; set; }
		public string selected { get; set; }
		public string judgedetails { get; set; }		


	}
}
