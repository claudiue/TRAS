using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TripleStore.Stardog;
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
        public ActionResult SaveItinerary(ItineraryClientViewModel itinClientVM) 
        {
            var creatorId = User.Identity.Name;
            var PersonVM = new PersonViewModel(){ Id = creatorId };

            var db = StardogDb.GetInstance();

            ItineraryViewModel itinVM = new ItineraryViewModel()
            {
                Id = Utils.GetID("itin"),
                Creator = PersonVM,
                Name = itinClientVM.name,
                Budget = itinClientVM.budget,
                NrOfDays = itinClientVM.days,
                Rating = itinClientVM.rating,
            };

            foreach (var location in itinClientVM.locations)
            {
                string placeID = string.Empty;
                if (location.locationData.GeoNameId > 0)
                {
                    placeID = string.Concat("place_", location.locationData.GeoNameId.ToString());
                }
                else
                {
                    placeID = Utils.GetID("place");
                }

                PlaceViewModel placeVM = new PlaceViewModel()
                {
                    Id = placeID,
                    Name = location.locationData.Name,
                    Lat = location.locationData.Latitude,
                    Long = location.locationData.Longitude
                };

                db.CreateOrUpdatePlace(placeVM);
                
                if (location.features != null)
                {
                    foreach (var feature in location.features)
                    {
                        string featureID = string.Empty;
                        if (feature.data.spotData.GeonameId > 0)
                        {
                            featureID = string.Concat("feat_", feature.data.spotData.GeonameId.ToString());
                        }
                        else
                        {
                            featureID = Utils.GetID("feat");
                        }

                        FeatureViewModel featureVM = new FeatureViewModel()
                        {
                            Id = featureID,
                            Name = feature.data.spotData.Name,
                            Code = feature.data.spotData.Fcode,
                            Parent = placeVM
                        };

                        double lat;
                        if (double.TryParse(feature.data.spotData.Lat, out lat))
                        {
                            featureVM.Lat = lat;
                        }

                        double lng;
                        if (double.TryParse(feature.data.spotData.Lng, out lng))
                        {
                            featureVM.Long = lng;
                        }

                        db.CreateOrUpdateFeature(featureVM);

                        itinVM.Features.Add(featureVM);
                    }
                }
                
            }

            db.CreateOrUpdateItinerary(itinVM);

            return RedirectToAction("UserItineraries", "Itineraries");
        }
	}
}