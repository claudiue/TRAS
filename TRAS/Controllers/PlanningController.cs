using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using WebServices;

namespace TRAS.Controllers
{
    public class PlanningController : Controller
    {
        //
        // GET: /Planning/
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

        [HttpPost]
        public JsonResult AjaxNearby(NearbyViewModel model)
        {
            //var toponym = new GeoNamesAgent().Search(model., "json");
            var res = new GeoNamesAgent().FindNearBy(model.Lat, model.Lng);
            return Json(res);
        }

        [HttpPost]
        public ActionResult SearchSpots(SearchViewModel model)
        {
            var spots = new GeoNamesAgent().SearchSpots(model.Query, "json");
            return Json(new { spots = spots });
        }

        [HttpPost]
        public void SaveItinerary(ItineraryClientViewModel itinerary) 
        {
            //var jobj = JObject.Parse(itinerary);
        }
	}
}