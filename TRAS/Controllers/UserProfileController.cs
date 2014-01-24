using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace TRAS.Controllers
{
    public class UserProfileController : Controller
    {
        //
        // GET: /UserProfile/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void UpdateProfile(AgentViewModel agentVM) 
        { 
            
        }
	}
}