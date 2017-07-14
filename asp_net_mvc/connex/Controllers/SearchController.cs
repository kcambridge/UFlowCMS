using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using connex.ViewModels;

namespace connex.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/Advanced/
        public ActionResult Advanced(string id, bool Inc_Posts, bool Inc_Documents, bool Inc_Galleries, bool Inc_Events, bool Inc_Videos)
        {
            if (id == null)
            {
                id = "";
            }
            SearchViewModel viewMod = new SearchViewModel(id, Inc_Posts,  Inc_Galleries, Inc_Events, Inc_Documents, Inc_Videos);
            return View("Index",viewMod);
        }
        //GET /Search
        public ActionResult Index(string id)
        {
            if (id == null)
            {
                id = "";
            }
            SearchViewModel viewMod = new SearchViewModel(id, true, true, true, true, true);
            return View(viewMod);
        }
	}
}