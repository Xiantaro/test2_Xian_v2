using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using System.Xml.Linq;

namespace test2.Areas.Frontend.Controllers
{
    [Area("Frontend")]
    public class HomeController : Controller
    {
        #region field
        public const string sk1 = "query1";
        public const string sk2 = "type1";
        public const string sk3 = "query2";
        public const string sk4 = "year1";
        public const string sk5 = "year2";
        public const string sk6 = "lang";
        public const string sk7 = "type2";
        public const string sk8 = "status";
        #endregion

        #region view
        public IActionResult Index() { return View(); }
        public IActionResult Query(string query1, string type1, string query2, string year1, string year2, string lang, string type2, string status)
        {
            if (string.IsNullOrEmpty(query1)) { query1 = string.Empty; }
            if (string.IsNullOrEmpty(type1)) { type1 = string.Empty; }
            if (string.IsNullOrEmpty(query2)) { query2 = string.Empty; }
            if (string.IsNullOrEmpty(year1)) { year1 = string.Empty; }
            if (string.IsNullOrEmpty(year2)) { year2 = string.Empty; }
            if (string.IsNullOrEmpty(lang)) { lang = string.Empty; }
            if (string.IsNullOrEmpty(type2)) { type2 = string.Empty; }
            if (string.IsNullOrEmpty(status)) { status = string.Empty; }

            HttpContext.Session.SetString(sk1, query1);
            HttpContext.Session.SetString(sk2, type1);
            HttpContext.Session.SetString(sk3, query2);
            HttpContext.Session.SetString(sk4, year1);
            HttpContext.Session.SetString(sk5, year2);
            HttpContext.Session.SetString(sk6, lang);
            HttpContext.Session.SetString(sk7, type2);
            HttpContext.Session.SetString(sk8, status);

            return View();
        }
        public IActionResult Collection() { return View(); }
        public IActionResult LoginC() { return View(); }
        public IActionResult LoginM() { return View(); }
        #endregion
    }
}