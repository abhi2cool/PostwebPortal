using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PostwebPortal.Models
{
    public class ResponseData
    {

        public List<UrlData> allUrlDetails;

        public ResponseData()
        {
            allUrlDetails = new List<UrlData>();
        }
    }
}
