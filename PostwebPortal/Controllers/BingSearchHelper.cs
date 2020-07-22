using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Azure.CognitiveServices.Search.WebSearch.Models;
using PostwebPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostwebPortal.Controllers
{
    public class BingSearchHelper
    {

        public static async Task<ResponseData> WebResults(WebSearchClient client,string query,int noAlgos)
        {
            ResponseData response = new ResponseData();
            try
            {
                var webData = await client.Web.SearchAsync(query: query);
                
                if (webData?.WebPages?.Value?.Count > 0)
                {
                    for (int i = 0; i < noAlgos; i++)
                    {
                        var webPagesResult = webData.WebPages.Value[i];

                        if (webPagesResult != null)
                        {
                            UrlData ud = new UrlData();
                            ud.bingData.name = webPagesResult.Name;
                            ud.bingData.url = webPagesResult.Url;
                            ud.bingData.snippet = webPagesResult.Snippet;
                            ud.bingData.displayUrl = webPagesResult.DisplayUrl;

                            ud.allPassages = new List<PassageData>( AzureSearchHelper.getPassagesfromUrls(new HashSet<string>() { ud.bingData.url }));

                            response.allUrlDetails.Add(ud);
                        }
                        else
                        {
                            Console.WriteLine("Didn't find any web pages...");
                        }
                    }
                    
                }
                else
                {
                    Console.WriteLine("Didn't find any web pages...");
                }

                /*
                 * Images
                 * If the search response contains images, the first result's name
                 * and url are printed.
                 */
                //if (webData?.Images?.Value?.Count > 0)
                //{
                //    // find the first image result
                //    var firstImageResult = webData.Images.Value.FirstOrDefault();

                //    if (firstImageResult != null)
                //    {
                //        Console.WriteLine("Image Results # {0}", webData.Images.Value.Count);
                //        Console.WriteLine("First Image result name: {0} ", firstImageResult.Name);
                //        Console.WriteLine("First Image result URL: {0} ", firstImageResult.ContentUrl);
                //    }
                //    else
                //    {
                //        Console.WriteLine("Didn't find any images...");
                //    }
                //}
                //else
                //{
                //    Console.WriteLine("Didn't find any images...");
                //}

                /*
                 * News
                 * If the search response contains news articles, the first result's name
                 * and url are printed.
                 */
                //if (webData?.News?.Value?.Count > 0)
                //{
                //    // find the first news result
                //    var firstNewsResult = webData.News.Value.FirstOrDefault();

                //    if (firstNewsResult != null)
                //    {
                //        Console.WriteLine("\r\nNews Results # {0}", webData.News.Value.Count);
                //        Console.WriteLine("First news result name: {0} ", firstNewsResult.Name);
                //        Console.WriteLine("First news result URL: {0} ", firstNewsResult.Url);
                //    }
                //    else
                //    {
                //        Console.WriteLine("Didn't find any news articles...");
                //    }
                //}
                //else
                //{
                //    Console.WriteLine("Didn't find any news articles...");
                //}

                /*
                 * Videos
                 * If the search response contains videos, the first result's name
                 * and url are printed.
                 */
                //if (webData?.Videos?.Value?.Count > 0)
                //{
                //    // find the first video result
                //    var firstVideoResult = webData.Videos.Value.FirstOrDefault();

                //    if (firstVideoResult != null)
                //    {
                //        Console.WriteLine("\r\nVideo Results # {0}", webData.Videos.Value.Count);
                //        Console.WriteLine("First Video result name: {0} ", firstVideoResult.Name);
                //        Console.WriteLine("First Video result URL: {0} ", firstVideoResult.ContentUrl);
                //    }
                //    else
                //    {
                //        Console.WriteLine("Didn't find any videos...");
                //    }
                //}
                //else
                //{
                //    Console.WriteLine("Didn't find any videos...");
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine("Encountered exception. " + ex.Message);
            }

            return response;
        }
    }
}
