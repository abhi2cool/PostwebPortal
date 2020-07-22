using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PostwebPortal.Controllers
{

    class CategoryMatchField
    {
        public string category;
        public string matchfield;
        public CategoryMatchField()
        {

        }
        public CategoryMatchField(CategoryMatchField c)
        {
            category = c.category;
            matchfield = c.matchfield;
        }
    }
    class UrlJoinHelper
    {
        public static CategoryMatchField geturlmatchfields(string url)
        {
            CategoryMatchField categoryMatchField = new CategoryMatchField();

            string category = GetUrlCategory(url);
            categoryMatchField.category = category;
            categoryMatchField.matchfield = GetMatchField(url, category);

            return categoryMatchField;
        }

        public static string GetUrlCategory(string url)
        {
            string category = "generic";
            if (url.Contains(@"://support.microsoft.com/"))
                category = "soc";
            else if (url.Contains(@"://support.office.com/"))
                category = "soc";
            else if (url.Contains(@"://support.google.com/"))
                category = "sgc";
            else if (url.Contains(@"://helpx.adobe.com/"))
                category = "adobe";
            else if (url.Contains(@"://support.apple.com/") && url.Contains(@"/guide/"))
                category = "sac_guide";
            else if (url.Contains(@"://support.apple.com/"))
                category = "sac";
            return category;

        }
        public static string GetMatchField(string url, string category)
        {
            string matchfield = "";
            if (category != "generic")
            {
                if (category == "sgc")
                    matchfield = Regex.Replace(url, "\\?.*", "").Split('/').Last();
                else if (category == "soc")
                    matchfield = GetSOCGuid(url);
                else if (category == "sac_guide")
                {
                    int parts = url.Split('/').Count();
                    if (parts >= 2)
                        matchfield = url.Split('/').ElementAt(parts - 2) + "/" + url.Split('/').Last();
                    else
                        matchfield = url;
                }
                else
                    matchfield = url.Split('/').Last();
            }
            if (matchfield == "")
                matchfield = url;

            return matchfield;

        }

        public static string GetSOCGuid(string url)
        {
            Regex reg = new Regex(@"[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}");
            Match match = reg.Match(url);
            if (match.Success)
            {
                return match.Value;
            }
            return "";
        }


    }
}