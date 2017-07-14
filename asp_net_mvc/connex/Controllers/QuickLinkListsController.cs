using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using connex.Auth;
using connex.Models;
using connex.ViewModels;

namespace connex.Controllers
{
    public class QuickLinkListsController : Controller
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();

        // GET: /QuickLinkLists/
        public ActionResult Index()
        {
            return View(db.QuickLinkLists.ToList());
        }

        // GET: /QuickLinkLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuickLinkList quicklinklist = db.QuickLinkLists.Find(id);
            if (quicklinklist == null)
            {
                return HttpNotFound();
            }
            return View(quicklinklist);
        }

        // GET: /QuickLinkLists/Create
        [AuthContentManagers]
        public ActionResult Create(int Page_ID)
        {
            QuickLinkListCreateViewModel viewMod = new QuickLinkListCreateViewModel();
            viewMod.Page_ID = Page_ID;
            return View(viewMod);
        }

        // POST: /QuickLinkLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Create(string Name, int Page_ID)
        {
            QuickLinkList newList = new QuickLinkList();
            newList.Name = Name;
            newList.Archived = false;
            newList.Date_Created = DateTime.Now;
            newList.Created_By = User.Identity.Name;
            db.QuickLinkLists.Add(newList);

            PageQuickLinkListsAssign qlAss = new PageQuickLinkListsAssign();
            qlAss.Link_List_ID = newList.Link_List_ID;
            qlAss.Page_ID = Page_ID;
            qlAss.Date_Added = DateTime.Now;
            qlAss.Added_By = User.Identity.Name;
            qlAss.Archived = false;
            db.PageQuickLinkListsAssigns.Add(qlAss);

            db.SaveChanges();
            
            return RedirectToAction("Edit", new { Link_List_ID = newList.Link_List_ID, Page_ID = Page_ID });
        }

        // GET: /QuickLinkLists/Edit/?Page_ID=5&Link_List_ID=5
        [AuthContentManagers]
        public ActionResult Edit(int Page_ID, int Link_List_ID)
        {
            QuickLinkList list = db.QuickLinkLists.Find(Link_List_ID);
            if (list == null)
            {
                return HttpNotFound();
            }
            QuickLinkListEditViewModel viewMod = new QuickLinkListEditViewModel(list, Page_ID);
            return View(viewMod);
        }

        // POST: /QuickLinkLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Edit([Bind(Include="Link_List_ID,Page_ID,Name,Archived")] QuickLinkListEditViewModel viewMod)
        {
            if (ModelState.IsValid)
            {
                QuickLinkList qList = db.QuickLinkLists.Find(viewMod.Link_List_ID);
                if (qList != null)
                {
                    qList.Name = viewMod.Name;
                    if (viewMod.Archived && !qList.Archived)
                    {
                        qList.Date_Archived = DateTime.Now;
                        qList.Archived_By = User.Identity.Name;
                    }
                    qList.Archived = viewMod.Archived;
                    db.SaveChanges();
                    return View(viewMod);
                }
                else
                {
                    ModelState.AddModelError("", "No Quick Link List found with the ID '" + viewMod.Link_List_ID + "'");
                    return View(viewMod);
                }
            }
            return View(viewMod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult RemoveQuickLink(int Page_ID, int Link_List_ID, int Quick_Link_ID)
        {
            QuickLink link = db.QuickLinks.Find(Quick_Link_ID);
            if (link != null)
            {
                link.Archived = true;
                link.Date_Added = DateTime.Now;
                link.Archived_By = User.Identity.Name;
                db.SaveChanges();
                return RedirectToAction("Edit", new { Page_ID = Page_ID, Link_List_ID = Link_List_ID });
            }
            else
            {
                QuickLinkList qList = db.QuickLinkLists.Find(Link_List_ID);
                QuickLinkListEditViewModel viewMod = new QuickLinkListEditViewModel(qList, Page_ID);
                ModelState.AddModelError("", "No Quick Link Found with the ID '"+Quick_Link_ID+"'");
                return View("Edit", viewMod);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult AddQuickLink(int Page_ID, int Link_List_ID, string Display_Text, string URL)
        {
            QuickLinkList qList = db.QuickLinkLists.Find(Link_List_ID);
            QuickLinkListEditViewModel viewMod = new QuickLinkListEditViewModel(qList, Page_ID);
            if (Display_Text.Trim() == "")
            {
                ModelState.AddModelError("", "'Display Text' must be filled out");
                return View("Edit", viewMod);
            }
            else if (URL.Trim() == "")
            {
                ModelState.AddModelError("", "'URL' must be filled out");
                return View("Edit", viewMod);
            }
            else
            {
                QuickLink link = new QuickLink();
                link.URL = URL;
                link.Display_Text = Display_Text;
                link.Archived = false;
                link.Date_Added = DateTime.Now;
                link.Added_By = User.Identity.Name;
                link.Link_List_ID = Link_List_ID;
                db.QuickLinks.Add(link);
                db.SaveChanges();

                return RedirectToAction("Edit", new { Page_ID = Page_ID, Link_List_ID = Link_List_ID });
            }
            
        }

        // GET: /QuickLinkLists/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    QuickLinkList quicklinklist = db.QuickLinkLists.Find(id);
        //    if (quicklinklist == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(quicklinklist);
        //}

        //// POST: /QuickLinkLists/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    QuickLinkList quicklinklist = db.QuickLinkLists.Find(id);
        //    db.QuickLinkLists.Remove(quicklinklist);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
