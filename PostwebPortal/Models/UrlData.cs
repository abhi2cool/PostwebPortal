using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostwebPortal.Models
{
    public class UrlData
    {
        public BingData bingData;
        public List<PassageData> allPassages;

        public UrlData()
        {
            bingData = new BingData();
            allPassages = new List<PassageData>();
        }
    }
}
