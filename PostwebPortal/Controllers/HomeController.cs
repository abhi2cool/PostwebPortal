﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Extensions.Logging;
using PostwebPortal.Models;
using X.PagedList;

namespace PostwebPortal.Controllers
{

    [Authorize]
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

        [AllowAnonymous]
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


        public IActionResult UrlExpansion(int? page, string url, string? keys, string? isselected)
        {
            var identity = (ClaimsIdentity)HttpContext.User.Identity;
            var judgename = identity.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var email = identity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            ViewBag.JudgeName = judgename;
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
                
                ViewBag.Url = Passages.FirstOrDefault().Url;
                ViewBag.PassageTitle = Passages.FirstOrDefault().HtmlHeadTitle;
                ViewBag.PassageCount = Passages.Count();
                
                ViewBag.Keys = keys;
                ViewBag.IsSelected = isselected;
                if (Passages.Count != 0)
                {
                    if (isselected == "selected")

                        qeResponseData = _context.getQueryCandidatesByUrl(key).Where(m => m.selectedanswerid != "none").ToList();
                    else if(isselected == "none")
                        qeResponseData = _context.getQueryCandidatesByUrl(key).Where(m => m.selectedanswerid == "none").ToList();
                    else
                    qeResponseData = _context.getQueryCandidatesByUrl(key);
                }



                if (keys != null && qeResponseData.Count > 0)
                {
                    string[] terms = keys.Split("||");
                    foreach (var term in terms)
                    {
                        qeResponseData = qeResponseData.Where(q => q.pwquery.Contains(term)).ToList();
                    }

                }

                foreach (var Passage in Passages)
                {
                    Passage.selectedcount = qeResponseData.Where(x => x.selectedanswerid == Passage.PassageId).Count();
                    Passage.RootQueries = string.Join(";", Passage.RootQueries.Split(";").Take(5));
                }
                ViewBag.Passages = Passages;

                
                //ViewBag.PassageIds = PassageIds;
                var pageNumber = page ?? 1; // if no page is specified, default to the first page (1)
                int pageSize = 100; // Get 25 students for each requested page.
                IPagedList onePageOfQueries = qeResponseData.ToPagedList(pageNumber, pageSize);
                ViewBag.Page = pageNumber;
                return View(onePageOfQueries);
            }
            else
            {
                IPagedList<RootQueryCandidate> qeResponseData = new List<RootQueryCandidate>().ToPagedList(1, 500);
                ViewBag.Page = 1;
                return View(qeResponseData);
            }
        }
       
        [HttpPost]
        public IActionResult UrlExpansion(string url, int? page, string? keys, string? isselected)
        {
            var identity = (ClaimsIdentity)HttpContext.User.Identity;
            var judgename = identity.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var email = identity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            CategoryMatchField urlfields = new CategoryMatchField(UrlJoinHelper.geturlmatchfields(url.ToLower()));
            string key = urlfields.category + ":" + urlfields.matchfield;
            List<RootQueryCandidate> qeResponseData = new List<RootQueryCandidate>();
            string check = Request.Form["check"];
            ViewBag.url = url;
            HashSet<string> Urls = new HashSet<string> { url };
            
            var Passages = AzureSearchHelper.getPassagesfromUrls(Urls);
            //List<String> PassageIds = new List<string>();
            
            ViewBag.Url = Passages.FirstOrDefault().Url;
            ViewBag.PassageTitle = Passages.FirstOrDefault().HtmlHeadTitle;
            ViewBag.PassageCount = Passages.Count();
            ViewBag.Judgename = judgename;
            ViewBag.Keys = keys;

            ViewBag.IsSelected = isselected;
            if (Passages.Count != 0)
            {
                if (isselected == "selected")

                    qeResponseData = _context.getQueryCandidatesByUrl(key).Where(m => m.selectedanswerid != "none").ToList();
                else if (isselected == "none")
                    qeResponseData = _context.getQueryCandidatesByUrl(key).Where(m => m.selectedanswerid == "none").ToList();
                else
                    qeResponseData = _context.getQueryCandidatesByUrl(key);
            }



            if (keys != null && qeResponseData.Count > 0)
            {
                string[] terms = keys.Split("||");
                foreach (var term in terms)
                {
                    qeResponseData = qeResponseData.Where(q => q.pwquery.Contains(term)).ToList();
                }

            }

            foreach (var Passage in Passages)
            {
                Passage.selectedcount = qeResponseData.Where(x => x.selectedanswerid == Passage.PassageId).Count();
                Passage.RootQueries = string.Join(";", Passage.RootQueries.Split(";").Take(5));
            }
            ViewBag.Passages = Passages;


            //ViewBag.PassageIds = PassageIds;
            var pageNumber = page ?? 1; // if no page is specified, default to the first page (1)
            int pageSize = 500; // Get 25 students for each requested page.
            IPagedList onePageOfQueries = qeResponseData.ToPagedList(pageNumber, pageSize);
            ViewBag.page = pageNumber;
            return View(onePageOfQueries);

       
        }

        [HttpPost]
        public IActionResult QueryUpdate( List<QueryPassageFormData> update, string url, int page , string? keys, string? isselected )
        {
            var identity = (ClaimsIdentity)HttpContext.User.Identity;
            var judgename = identity.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var email = identity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
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
                        i.lastmodifiedby = email;
                        i.lastmodifiedon = DateTime.Now;
                    }
                }

                _context.SaveChanges();

             
            }

            //ViewBag.PassageIds = PassageIds;

            return RedirectToAction("UrlExpansion", new { url = url, page= page, judgename= judgename, keys= keys, isselected = isselected });


        }

        public IActionResult AddQuery(string queries,string selectedanswerid, string url, int page, string? keys)
        {
            var identity = (ClaimsIdentity)HttpContext.User.Identity;
            var judgename = identity.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var email = identity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

            CategoryMatchField urlfields = new CategoryMatchField(UrlJoinHelper.geturlmatchfields(url.ToLower()));
            string key = urlfields.category + ":" + urlfields.matchfield;
            string[] allqueries = queries.Split("||");

            //key = "sac:ht204974";
            //var headers = Request.Headers;
            //string UEUrl = "xyz";
            //QEResponseData qeResponseData = new QEResponseData();
            //string check = Request.Form["check"];
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

            if (allqueries.Count() != 0)
            {

                updatedata.AddRange(_context.getQueryCandidatesByUrl(key));

                foreach (var query in allqueries)
                {
                    var contains = _context.getQueryCandidatesByUrl(key).Where(x=>x.pwquery == query);
                    RootQueryCandidate i = new RootQueryCandidate();
                    if (contains.Count() == 0)
                    {
                        i.queryid = Guid.NewGuid().ToString();   //todo check for guid presence in db
                        i.pwquery = query;
                        i.url = key;
                        i.selectedanswerid = selectedanswerid;
                        i.lastmodifiedby = email;
                        i.lastmodifiedon = DateTime.Now;
                        i.impression = -1;
                        _context.AddAsync(i);
                        _context.SaveChangesAsync();

                    }

                    else
                    {
                       var temp = updatedata.Where(a => (a.pwquery == query)
                                                  && (a.selectedanswerid != selectedanswerid)).FirstOrDefault();
                        if (temp != null)
                        {
                            temp.selectedanswerid = selectedanswerid;
                            temp.lastmodifiedby = email;
                            temp.lastmodifiedon = DateTime.Now;
                        }
                        _context.SaveChangesAsync();
                    }

                    _context.SaveChangesAsync();

                }

                


            }

            //ViewBag.PassageIds = PassageIds;

            return RedirectToAction("UrlExpansion", new { url = url, page = page, judgename = judgename, keys = keys });


        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public IActionResult SignOut()
        {
            return SignOut(new AuthenticationProperties()
            { RedirectUri = "/Home/SignOutSuccess" },
        AzureADDefaults.AuthenticationScheme,
        AzureADDefaults.CookieScheme,
        AzureADDefaults.OpenIdScheme);
        }

        public IActionResult SignOutSuccess()
        {
            return RedirectToAction("Index");
        }

    }
}
