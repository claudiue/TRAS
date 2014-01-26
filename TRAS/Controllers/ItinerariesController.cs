using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TripleStore.Stardog;
using ViewModels;

namespace TRAS.Controllers
{
    public class ItinerariesController : Controller
    {
        //
        // GET: /Itineraries/
        public ActionResult Index()
        {
            IList<ItineraryViewModel> itineraries = null;
            var db = StardogDb.GetInstance();
            itineraries = db.GetItineraries();
            return View(itineraries);
        }

        public ActionResult ItineraryDetails(string id)
        {
            IList<ItineraryViewModel> itineraries = null;
            var db = StardogDb.GetInstance();
            itineraries = db.GetItineraries();
            return View(itineraries.Where(it => it.Id == id).FirstOrDefault());
        }

        public ActionResult UserItineraries(string id = null)
        {
            if (id == null)
            {
                id = User.Identity.Name;
            }
            IList<ItineraryViewModel> itineraries = null;
            if (!string.IsNullOrEmpty(id))
            {
                var db = StardogDb.GetInstance();
                itineraries = db.GetItineraries(id);
            }
            return View(itineraries);
        }

        public ActionResult UserItineraryDetails(string id)
        {
            IList<ItineraryViewModel> itineraries = null;
            var db = StardogDb.GetInstance();
            itineraries = db.GetItineraries();
            return View(itineraries.Where(it => it.Id == id).FirstOrDefault());
        }

        

        
	}
}