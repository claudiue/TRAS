using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TRAS.Models.ViewModels;
using WebServices;

namespace TRAS.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Search(SearchViewModel model)
        {
            var toponym = new GeoNamesAgent().Search(model.Query, "json");
            //var res = new GeoNamesAgent().FindNearBy(toponym);
            return Json(toponym);
        }
	}
}