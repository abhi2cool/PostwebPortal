using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Extensions.Logging;
using PostwebPortal.Models;
using X.PagedList;

namespace PostwebPortal.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public RootQueryCandidateDbContext rqcContext; 
        private readonly RootQueryCandidateDbContext _context;
        public HomeController(ILogger<HomeController> logger, RootQueryCandidateDbContext context)
        {
            ResponseData response = new ResponseData();
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [Route("Home/SearchHandle")]
        public IActionResult SearchHandle()
        {
            string Query = Request.Form["Query"];
            ViewBag.Query = Query;
            int noAlgos = 3;
            var client = new WebSearchClient(new ApiKeyServiceClientCredentials("8d3129ea335440da904641c7cc055494"));
            var response = BingSearchHelper.WebResults(client, Query, noAlgos);

            //-------------test
            List<RootQueryCandidate> rqCandidates = _context.getQueryCandidates();
            //----------------
            return View(response);
        }


        public IActionResult Index()
        {
            ResponseData response = new ResponseData();
            return View(response);
        }
        [HttpPost]
        public IActionResult Index(string Query)
        {
            ViewBag.Query = Query;
            int noAlgos = 10;
            var client = new WebSearchClient(new ApiKeyServiceClientCredentials("8d3129ea335440da904641c7cc055494"));
            var response = BingSearchHelper.WebResults(client, Query, noAlgos);

            //-------------test
            List<RootQueryCandidate> rqCandidates = _context.getQueryCandidates();
            //_context.answerid_pwqueries_selection.Add(new RootQueryCandidate { answerid = "a", impression = 1, judgedetails = "", pwquery = "testa1", selected = "false" });
            //_context.answerid_pwqueries_selection.Add(new RootQueryCandidate { answerid = "a", impression = 2, judgedetails = "", pwquery = "testa2", selected = "false" });
            //_context.answerid_pwqueries_selection.Add(new RootQueryCandidate { answerid = "a", impression = 3, judgedetails = "", pwquery = "testa3", selected = "false" });
            //_context.answerid_pwqueries_selection.Add(new RootQueryCandidate { answerid = "b", impression = 2, judgedetails = "", pwquery = "testb2", selected = "false" });
            //_context.SaveChanges();
            //----------------
            var results = _context.url_pwqueries_selection.Where(q => q.selectedanswerid == "a").OrderByDescending(i => i.impression);

            return View(response.Result);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult QueryExpansion()
        {
            QEResponseData qeResponseData = new QEResponseData();
            return View(qeResponseData);
        }

        [HttpPost]
        public IActionResult QueryExpansion(string QEQuery)
        {
            QEResponseData qeResponseData = new QEResponseData();
            string check = Request.Form["check"];
            ViewBag.QEQuery = QEQuery;
            qeResponseData.passageData = AzureSearchHelper.getPassagefromAnswerId(QEQuery);
            if(!string.IsNullOrEmpty( qeResponseData.passageData.PassageId))
                qeResponseData.allCandidateQueries = _context.getQueryCandidatesByAnswerId(qeResponseData.passageData.PassageId);
            
            return View(qeResponseData);
            
        }


        public IActionResult UrlExpansion(int? page, string url, string? judgename)
        {
            if (url != null)
            {

                CategoryMatchField urlfields = new CategoryMatchField(UrlJoinHelper.geturlmatchfields(url.ToLower()));
                string key = urlfields.category + ":" + urlfields.matchfield;
                List<RootQueryCandidate> qeResponseData = new List<RootQueryCandidate>();
                //string check = Request.Form["check"];
                ViewBag.url = url;
                HashSet<string> Urls = new HashSet<string> { url };

                var Passages = AzureSearchHelper.getPassagesfromUrls(Urls);
                //List<String> PassageIds = new List<string>();
                ViewBag.Passages = Passages;
                foreach (var Passage in Passages)
                {
                    Passage.RootQueries = Passage.RootQueries.Split(";").First();
                }
                ViewBag.Url = Passages.FirstOrDefault().Url;
                ViewBag.PassageTitle = Passages.FirstOrDefault().HtmlHeadTitle;
                ViewBag.PassageCount = Passages.Count();
                ViewBag.JudgeName = judgename;
                if (Passages.Count != 0)
                {

                    qeResponseData = _context.getQueryCandidatesByUrl(key);

                }
                //ViewBag.PassageIds = PassageIds;
                var pageNumber = page ?? 1; // if no page is specified, default to the first page (1)
                int pageSize = 100; // Get 25 students for each requested page.
                IPagedList onePageOfQueries = qeResponseData.ToPagedList(pageNumber, pageSize);
                ViewBag.Page = pageNumber;
                return View(onePageOfQueries);
            }
            else
            {
                IPagedList<RootQueryCandidate> qeResponseData = new List<RootQueryCandidate>().ToPagedList(1, 100);
                ViewBag.Page = 1;
                return View(qeResponseData);
            }
        }
       
        [HttpPost]
        public IActionResult UrlExpansion(string url, int? page, string? judgename)
        {
            CategoryMatchField urlfields = new CategoryMatchField(UrlJoinHelper.geturlmatchfields(url.ToLower()));
            string key = urlfields.category + ":" + urlfields.matchfield;
            List<RootQueryCandidate> qeResponseData = new List<RootQueryCandidate>();
            string check = Request.Form["check"];
            ViewBag.url = url;
            HashSet<string> Urls = new HashSet<string> { url };
            
            var Passages = AzureSearchHelper.getPassagesfromUrls(Urls);
            //List<String> PassageIds = new List<string>();
            foreach (var Passage in Passages)
            {
                Passage.RootQueries = Passage.RootQueries.Split(";").First();
            }
            ViewBag.Passages = Passages;
            ViewBag.Url = Passages.FirstOrDefault().Url;
            ViewBag.PassageTitle = Passages.FirstOrDefault().HtmlHeadTitle;
            ViewBag.PassageCount = Passages.Count();
            ViewBag.Judgename = judgename;
            if (Passages.Count != 0)
            {
              
                    qeResponseData = _context.getQueryCandidatesByUrl(key);
                   
            }
            //ViewBag.PassageIds = PassageIds;
            var pageNumber = page ?? 1; // if no page is specified, default to the first page (1)
            int pageSize = 100; // Get 25 students for each requested page.
            IPagedList onePageOfQueries = qeResponseData.ToPagedList(pageNumber, pageSize);
            ViewBag.page = pageNumber;
            return View(onePageOfQueries);

       
        }

        [HttpPost]
        public IActionResult QueryUpdate( List<QueryPassageFormData> update, string url, string judgename,int page )
        {
            CategoryMatchField urlfields = new CategoryMatchField(UrlJoinHelper.geturlmatchfields(url.ToLower()));
            string key = urlfields.category + ":" + urlfields.matchfield;
            //key = "sac:ht204974";
            //var headers = Request.Headers;
            //string UEUrl = "xyz";
            //QEResponseData qeResponseData = new QEResponseData();
            string check = Request.Form["check"];
            //ViewBag.UEUrl = url;
            //HashSet<string> Urls = new HashSet<string> { url };
            //ViewBag.page = page;
            //var Passages = AzureSearchHelper.getPassagesfromUrls(Urls);
            // List<String> PassageIds = new List<string>();
            //foreach (var Passage in Passages)
            //{
                //Passage.RootQueries = Passage.RootQueries.Split(";").First();   
            //}
            //ViewBag.Passages = Passages;
            //ViewBag.Url = url;
            //ViewBag.PassageTitle = Passages.FirstOrDefault().HtmlHeadTitle;
            //ViewBag.PassageCount = Passages.Count();
            List<RootQueryCandidate> updatedata = new List<RootQueryCandidate>();
            
            if (update.Count != 0)
            {
                
                    updatedata.AddRange(_context.getQueryCandidatesByUrl(key));
                
                foreach(var item in update)
                {
                    item.selectedanswerid = item.selectedanswerid ?? "none";
                    var i = updatedata.Where(a=> (a.queryid== item.queryid)
                                                  &&(a.selectedanswerid != item.selectedanswerid )).FirstOrDefault();
                    if (i != null)
                    {
                        i.selectedanswerid = item.selectedanswerid ;
                        i.lastmodifiedby = judgename;
                        i.lastmodifiedon = DateTime.Now;
                    }
                }

                _context.SaveChanges();

             
            }

            //ViewBag.PassageIds = PassageIds;

            return RedirectToAction("UrlExpansion", new { url = url, page= page, judgename= judgename });


        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
