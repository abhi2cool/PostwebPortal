using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostwebPortal.Models
{
    [SerializePropertyNamesAsCamelCase]
    public class PassageData
    {
        
        [System.ComponentModel.DataAnnotations.Key]
        [IsSearchable, IsFilterable]
        public string PassageId { get; set; }
        public string FinalTitle { get; set; }
        public string HtmlHeadTitle { get; set; }
        public string RubatoAnswer { get; set; }
        public string RawAnswer { get; set; }
        public string FinalTitleQASKey { get; set; }
        public string Url { get; set; }
        [IsSearchable, IsFilterable]
        public string Urlcategory { get; set; }
        [IsSearchable, IsFilterable]
        public string Urlmatchfield { get; set; }

        [IsSearchable, IsFilterable]
        [Analyzer(AnalyzerName.AsString.EnMicrosoft)]
        public string RootQueries { get; set; }

        public int selectedcount { get; set; }

    }

}
