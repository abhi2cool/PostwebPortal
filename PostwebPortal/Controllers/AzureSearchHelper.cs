using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using PostwebPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PostwebPortal.Controllers
{
    public class AzureSearchHelper
    {
        public static List<PassageData> getPassagesfromUrls(HashSet<string> urls)
        {
            List<PassageData> allPD = new List<PassageData>();

           
            ISearchServiceClient serviceClient = new SearchServiceClient("passageranker", new SearchCredentials("61438B42BF9E15FB1FBAAA6963C3FCB8"));
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("techpassagesdata");

            HashSet<string> aids = new HashSet<string>();
            foreach (var item in urls)
            {
                CategoryMatchField urlfields = new CategoryMatchField(UrlJoinHelper.geturlmatchfields(item.ToLower()));
                string key = urlfields.category + ":" + urlfields.matchfield;
                string filter = "urlcategory eq '" + urlfields.category + "' and urlmatchfield eq '" + urlfields.matchfield + "'";
                SearchParameters parameters;
                DocumentSearchResult<PassageData> results;
                parameters =
                    new SearchParameters()
                    {
                        Filter = filter
                    };

                string test = "";
                results = indexClient.Documents.Search<PassageData>(test, parameters);
                foreach (var item2 in results.Results)
                {
                    PassageData pd = new PassageData();
                    try
                    {
                        pd.Url = item2.Document.Url;
                        pd.FinalTitle = item2.Document.FinalTitle;
                        pd.FinalTitleQASKey = item2.Document.FinalTitleQASKey;
                        pd.HtmlHeadTitle = item2.Document.HtmlHeadTitle;
                        pd.PassageId = item2.Document.PassageId;
                        pd.RawAnswer = item2.Document.RawAnswer;
                        pd.RootQueries = item2.Document.RootQueries;
                        pd.RubatoAnswer = item2.Document.RubatoAnswer;
                        pd.Urlcategory = item2.Document.Urlcategory;
                        pd.Urlmatchfield = item2.Document.Urlmatchfield;

                        allPD.Add(pd);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            return allPD;

        }


        public static PassageData getPassagefromAnswerId(string AnswerId)
        {
            List<PassageData> allPD = new List<PassageData>();

            ISearchServiceClient serviceClient = new SearchServiceClient("passageranker", new SearchCredentials("61438B42BF9E15FB1FBAAA6963C3FCB8"));
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("techpassagesdata");

            string filter = "passageId eq '" + AnswerId + "'";
            SearchParameters parameters;
            DocumentSearchResult<PassageData> results;
            parameters =
                new SearchParameters()
                {
                    Filter = filter
                };

            string test = "";
            results = indexClient.Documents.Search<PassageData>(test, parameters);
            foreach (var item2 in results.Results)
            {
                PassageData pd = new PassageData();
                try
                {
                    pd.Url = item2.Document.Url;
                    pd.FinalTitle = item2.Document.FinalTitle;
                    pd.FinalTitleQASKey = item2.Document.FinalTitleQASKey;
                    pd.HtmlHeadTitle = item2.Document.HtmlHeadTitle;
                    pd.PassageId = item2.Document.PassageId;
                    pd.RawAnswer = item2.Document.RawAnswer;
                    pd.RootQueries = item2.Document.RootQueries;
                    pd.RubatoAnswer = item2.Document.RubatoAnswer;
                    pd.Urlcategory = item2.Document.Urlcategory;
                    pd.Urlmatchfield = item2.Document.Urlmatchfield;

                    allPD.Add(pd);
                }
                catch (Exception e)
                {

                }
            }


            //always returns the first
            return allPD.First();

        }
    }
}
