using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notes_MarketPlace.Controllers
{
    public class SettingsController : Controller
    {
        // GET: Settings
        public ActionResult ManageSystemConfiguration()
        {
            return View();
        }

        public ActionResult AddAdministrator()
        {
            return View();
        }
        public ActionResult ManageAdministrator()
        {
            return View();
        }

        public ActionResult AddCategory()
        {
            return View();
        }
        public ActionResult ManageCategory()
        {
            return View();
        }

        public ActionResult AddType()
        {
            return View();
        }
        public ActionResult ManageType()
        {
            return View();
        }

        public ActionResult AddCountry()
        {
            return View();
        }
        public ActionResult ManageCountries()
        {
            return View();
        }

    }
}