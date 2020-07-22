using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Extensions.Logging;
using PostwebPortal.Models;

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
            var results = _context.answerid_pwqueries_selection.Where(q => q.answerid == "a").OrderByDescending(i => i.impression);

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
