using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostwebPortal.Models
{
    public class QEResponseData
    {
        public PassageData passageData;
        public List<RootQueryCandidate> allCandidateQueries;
        public QEResponseData()
        {
            passageData = new PassageData();
            allCandidateQueries = new List<RootQueryCandidate>();
        }
    }
}
