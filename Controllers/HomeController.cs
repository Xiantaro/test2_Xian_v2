using Microsoft.AspNetCore.Mvc;

namespace test2.Controllers
{
    public class HomeController : Controller
    {
        #region view
        public IActionResult Index() { return View(); }
        #endregion
    }
}
