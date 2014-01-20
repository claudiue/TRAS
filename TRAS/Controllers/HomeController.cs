using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TRAS.Models;
using TRAS.Models.ViewModels;
using WebServices;

namespace TRAS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            ViewBag.Email = "User not found.";
            UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (User.Identity.GetUserId() != null)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                    ViewBag.Email = user.Email;
            }
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public JsonResult Search(SearchViewModel model)
        {
            var spots = new GeoNamesAgent().SearchSpots(model.Query, "json");
            return Json(spots);
        }
    }
}