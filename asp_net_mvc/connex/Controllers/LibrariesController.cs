using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using connex.Auth;
using connex.Models;
using connex.ViewModels;

namespace connex.Controllers
{
    public class LibrariesController : Controller
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();
        private static int MAX_DOC_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings["DocumentMaxSize"]);
        private static string DOC_BASE_ROOT = ConfigurationManager.AppSettings["DocumentUploadBaseRoot"];

        // GET: /Libraries/
        public ActionResult Index()
        {
            return View(db.Libraries.Include(d => d.Documents).ToList());
        }

        // GET: /Libraries/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Library library = db.Libraries.Find(id);
        //    if (library == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(library);
        //}

        // GET: /Libraries/Download/5
        public ActionResult Download(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document doc= db.Documents.Find(id);
            if (doc == null)
            {
                return HttpNotFound();
            }
            
            return View(doc);
        }

        // GET: /Libraries/Create?Page_ID=8
        [AuthContentManagers]
        public ActionResult Create(int Page_ID)
        {
            LibraryCreateViewModel libMod = new LibraryCreateViewModel();
            libMod.Page_ID = Page_ID;
            return View(libMod);
        }

        // POST: /Libraries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Create(string Title_Text, string Description, int Page_ID)
        {
            string libName = Title_Text.Replace(" ", "");
            List<Library> existingLibs = (List<Library>)(from l in db.Libraries where l.Library_Name == libName select l).ToList();
            if (!(existingLibs.Count() > 0))
            {
                Library library = new Library();
                library.Title_Text = Title_Text;
                library.Library_Name = libName;
                library.Description = Description;
                library.Date_Added = DateTime.Now;
                library.Added_By = User.Identity.Name.ToString();///TODO: auth needed
                library.Archived = false;
                db.Libraries.Add(library);
                db.SaveChanges();

                PageLibraryAssign libAss = new PageLibraryAssign();
                libAss.Library = library;
                libAss.Page_ID = Page_ID;
                db.PageLibraryAssigns.Add(libAss);
                db.SaveChanges();

                return RedirectToAction("Edit", new { Page_ID = Page_ID, Library_ID = library.Library_ID });
            }
            else
            {
                ModelState.AddModelError("", "A Library with the name '"+Title_Text+"' already exists");
                LibraryCreateViewModel libMod = new LibraryCreateViewModel();
                libMod.Page_ID = Page_ID;
                libMod.Description = Description;
                libMod.Title_Text = Title_Text;
                return View(libMod);
            }
            

            
        }

        // GET: /Libraries/Edit?Page_ID=5&Library_ID=1
        [AuthContentManagers]
        public ActionResult Edit(int Page_ID, int Library_ID)
        {
            
            Library library = db.Libraries.Find(Library_ID);
            
            if (library == null)
            {
                return HttpNotFound();
            }
            LibraryEditViewModel libViewMod = new LibraryEditViewModel(library, Page_ID);
            ViewBag.Success_Uploads = "";
            ViewBag.Failed_Uploads = "";
            ViewBag.Upload_Counts = "";
            return View(libViewMod);
        }

        // POST: /Libraries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Edit(string Title_Text, string Description, bool Archived, int Library_ID, int Page_ID)
        {
            if (ModelState.IsValid)
            {
                Library library = db.Libraries.Find(Library_ID);
                library.Library_Name = Title_Text;
                library.Title_Text = Title_Text;
                library.Description = Description;
                if (library.Archived != Archived)
                {
                    library.Archived = Archived;
                    library.Date_Archive = DateTime.Now;
                    library.Archive_By = User.Identity.Name.ToString();///TODO: auth
                }
                db.SaveChanges();
                return RedirectToAction("Edit", new { Page_ID = Page_ID, Library_ID = Library_ID });
            }
            else
            {
                Library library = db.Libraries.Find(Library_ID);
                ModelState.AddModelError("", "Invalid Model State. Changes not saved.");
                LibraryEditViewModel libViewMod = new LibraryEditViewModel(library, Page_ID);
                libViewMod.Title_Text = Title_Text;
                libViewMod.Description = Description;
                libViewMod.Archived = Archived;
                return View(libViewMod);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult RemoveDocument(int Page_ID, int Library_ID, int Document_ID)
        {
            Document doc = db.Documents.Find(Document_ID);
            if (doc != null)
            {
                db.Documents.Remove(doc);
                db.SaveChanges();
            }
            return RedirectToAction("Edit", new { Page_ID = Page_ID, Library_ID = Library_ID});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult AddDocuments(HttpPostedFileBase[] files, int Page_ID, int Library_ID, string Library_Name)
        {
            string successUploads = "<h3>Successful Uploads</h3><hr /><table role=\"presentation\" class=\"table table-striped\"><tbody class=\"files\">";
            string failedUploads = "<h3>Failed Uploads</h3><hr /><table role=\"presentation\" class=\"table table-striped\"><tbody class=\"files\">";
            int success = 0;
            int failed = 0;
            foreach (HttpPostedFileBase DocFile in files)
            {
                if (DocFile != null)
                {
                    try
                    {
                        Regex r = new Regex("[^a-zA-Z0-9]");
                        string dirName = r.Replace(Library_Name, "");
                        string pathRoot = "~/" + DOC_BASE_ROOT + dirName + "/";
                        string dbRoot = DOC_BASE_ROOT + dirName + "/";
                        string displayRoot = "/" + DOC_BASE_ROOT + dirName + "/";
                        string fileNameNoExt = System.IO.Path.GetFileNameWithoutExtension(DocFile.FileName);
                        string ext = System.IO.Path.GetExtension(DocFile.FileName).ToLower();
                        DocumentType type = (DocumentType)db.DocumentTypes.Where(x => x.Extension == ext).FirstOrDefault();
                        if (type != null)
                        {
                            if (DocFile.ContentLength <= MAX_DOC_SIZE)
                            {
                                if (!System.IO.Directory.Exists(Server.MapPath(pathRoot)))
                                {
                                    System.IO.Directory.CreateDirectory(Server.MapPath(pathRoot));
                                }

                                string filePath = Server.MapPath(pathRoot) + DocFile.FileName;
                                string filePathTemp = Server.MapPath(pathRoot) + DocFile.FileName;


                                try
                                {
                                    fileNameNoExt = UploadFile(DocFile, pathRoot, fileNameNoExt, ext);
                                }
                                catch (Exception ex)
                                {
                                    return Json(new { error = "Upload Failed", message = ex.Message });
                                }



                                Document doc = new Document();
                                doc.Name = fileNameNoExt.Replace("_", " ").Replace("-", " ");
                                doc.Path = dbRoot + fileNameNoExt + ext;
                                doc.Library_ID = Library_ID;
                                doc.Date_Added = DateTime.Now;
                                doc.Added_By = User.Identity.Name.ToString();///TODO: auth
                                doc.Archived = false;
                                doc.DocumentType = type;
                                db.Documents.Add(doc);


                                string displayPath = "/Libraries/Download/" + doc.Document_ID;
                                successUploads += "<tr class=\"table table-striped\"><td class=\"col-md-6\"><span class=\"preview\"><div class=\"col-sm-6 col-md-4\"><div class=\"thumbnail\"><img class=\"img-responsive\" alt=\"No Image Found\" src=\"/images/document-icon.png\"\"></img></a></div></div></span></td><td class=\"col-md-2\"><p class=\"name\">" + DocFile.FileName + "</p></td><td class=\"col-md-2\"><img class=\"img-responsive col-md-6\" src=\"../images/green_tick.png\" alt\"No Image Found\"></img></td></tr>";
                                success++;
                                //var files = new List<object>();
                                //files.Add(new { url = displayPath.Replace("\\", "\\/"), thumbnailUrl = "/images/document-icon.png", name = DocFile.FileName, type = doc.DocumentType.Content_Type, size = DocFile.ContentLength, deleteUrl = displayPath.Replace("\\", "\\/"), deleteType = "DELETE" });
                                //return Json(files, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                failedUploads += "<tr class=\"table table-striped\"><td class=\"col-md-6\"><span class=\"label label-danger\">Error:</span>&nbsp;File must be less than 30 MB</td><td class=\"col-md-2\"><p class=\"name\">" + DocFile.FileName + "</p></td><td class=\"col-md-2\"><img class=\"img-responsive col-md-6\" src=\"../images/red_x.png\" alt\"No Image Found\"></img></td></tr>";
                                failed++;
                            }
                        }
                        else
                        {
                            failedUploads += "<tr class=\"table table-striped\"><td class=\"col-md-6\"><span class=\"label label-danger\">Error:</span>&nbsp;" + ext + " is not a supported document format</td><td class=\"col-md-2\"><p class=\"name\">" + DocFile.FileName + "</p></td><td class=\"col-md-2\"><img class=\"img-responsive col-md-6\" src=\"../images/red_x.png\" alt\"No Image Found\"></img></td></tr>";
                            failed++;
                        }
                    }
                    catch (Exception EX)
                    {

                        failedUploads += "<tr class=\"table table-striped\"><td class=\"col-md-6\"><span class=\"label label-danger\">Error:</span>&nbsp;" + EX.Message + "</td><td class=\"col-md-2\"><p class=\"name\">" + DocFile.FileName + "</p></td><td class=\"col-md-2\"><img class=\"img-responsive col-md-6\" src=\"../images/red_x.png\" alt\"No Image Found\"></img></td></tr>";
                        failed++;
                    }
                }
                
            }
            db.SaveChanges();
            if (success > 0)
            {
                successUploads += "</tbody></table>";
            }
            else
            {
                successUploads = "";
            }
            if (failed > 0)
            {
                failedUploads += "</tbody></table>";
            }
            else
            {
                failedUploads = "";
            }

            ViewBag.Success_Uploads = successUploads;
            ViewBag.Failed_Uploads = failedUploads;
            string cssClass = "warning";
            if (failed == 0)
            {
                cssClass = "success";
            }
            else if (success == 0)
            {
                cssClass = "danger";
            }
            if (success > 0 || failed > 0)
            {
                ViewBag.Upload_Counts = "<div class=\"alert alert-" + cssClass + " alert-dismissible\" role=\"alert\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button><strong>Upload Complete:</strong> " + success + " file(s) uploaded successfully, " + failed + " failed.</div>";
            }
            else
            {
                ViewBag.Upload_Counts = "";
            }
            Library lib = db.Libraries.Find(Library_ID);
            LibraryEditViewModel vm = new LibraryEditViewModel(lib, Page_ID);
            return View("Edit", vm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult AddDocument(int Page_ID, int Library_ID, string Name)
        {
            Library lib = db.Libraries.Find(Library_ID);
            string pathRoot = "~/"+DOC_BASE_ROOT+ lib.Library_Name + "/";
            string dbRoot = DOC_BASE_ROOT + lib.Library_Name + "/";
            if (!System.IO.Directory.Exists(Server.MapPath(pathRoot)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(pathRoot));
            }
            if (Request.Files.Count > 0)
            {

                HttpPostedFileBase file = Request.Files["Document"];
                if (file != null)
                {
                    if (file.ContentLength > 0)
                    {
                        if (file.ContentLength < MAX_DOC_SIZE)
                        {
                            string fileName = file.FileName;
                            string fileNameNoEx = Path.GetFileNameWithoutExtension(file.FileName);
                            string ext = Path.GetExtension(file.FileName).ToLower();
                            DocumentType type = (DocumentType)db.DocumentTypes.Where(x => x.Extension == ext).FirstOrDefault();
                            if (type != null)
                            {
                                fileNameNoEx = UploadFile(file, pathRoot, fileName, fileNameNoEx, ext);
                                fileName = fileNameNoEx + ext;
                                Document doc = new Document();
                                doc.Name = Name;
                                doc.Path = dbRoot + fileName;
                                doc.Library_ID = Library_ID;
                                doc.Date_Added = DateTime.Now;
                                doc.Added_By = User.Identity.Name.ToString();///TODO: auth
                                doc.Archived = false;
                                doc.DocumentType = type;
                                db.Documents.Add(doc);
                                db.SaveChanges();
                                return RedirectToAction("Edit", new { Page_ID = Page_ID, Library_ID = Library_ID });

                            }
                            else
                            {
                                ModelState.AddModelError("", "The '" + ext + "' document type is not supported.");
                                LibraryEditViewModel libViewMod = new LibraryEditViewModel(lib, Page_ID);
                                return View("Edit", libViewMod);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Documents must be less than 30 MB");
                            LibraryEditViewModel libViewMod = new LibraryEditViewModel(lib, Page_ID);
                            return View("Edit", libViewMod);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "You must select a 'Document' to upload");
                        LibraryEditViewModel libViewMod = new LibraryEditViewModel(lib, Page_ID);
                        return View("Edit", libViewMod);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "You must select a 'Document' to upload");
                    LibraryEditViewModel libViewMod = new LibraryEditViewModel(lib, Page_ID);
                    return View("Edit", libViewMod);
                }
            }
            else
            {
                ModelState.AddModelError("", "You must select a 'Document' to upload");
                LibraryEditViewModel libViewMod = new LibraryEditViewModel(lib, Page_ID);
                return View("Edit", libViewMod);
            }
            
        }
        private string UploadFile(HttpPostedFileBase file, string pathRoot, string fileNameNoEx, string ext)
        {
            string retName = fileNameNoEx;
            try
            {
                byte[] imageSize = new byte[file.ContentLength];
                file.InputStream.Read(imageSize, 0, (int)file.ContentLength);
                string filePath = Server.MapPath(pathRoot) + fileNameNoEx+ext;

                if (System.IO.File.Exists(filePath))
                {
                    int i = 1;
                    filePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                    retName = fileNameNoEx + i.ToString();
                    while (System.IO.File.Exists(filePath))
                    {
                        i++;
                        filePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                        retName = fileNameNoEx + i.ToString();
                    }

                }

                file.SaveAs(filePath);

            }
            catch (Exception ex)
            {

                throw;
            }
            return retName;

        }
        private string UploadFile(HttpPostedFileBase file, string pathRoot, string fileName, string fileNameNoEx,string ext)
        {
            string retFileName = fileNameNoEx;
            byte[] imageSize = new byte[file.ContentLength];
            file.InputStream.Read(imageSize, 0, (int)file.ContentLength);
            string filePath = Server.MapPath(pathRoot) + fileNameNoEx+ext;
            
            if (System.IO.File.Exists(filePath))
            {
                int i = 1;
                filePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                retFileName = fileNameNoEx + i.ToString();
                while (System.IO.File.Exists(filePath))
                {
                    i++;
                    filePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                    fileName = fileNameNoEx + i.ToString() + ext;
                    retFileName = fileNameNoEx + i.ToString();
                }

            }

            file.SaveAs(filePath);
            return retFileName;
        }
        // GET: /Libraries/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Library library = db.Libraries.Find(id);
        //    if (library == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(library);
        //}

        //// POST: /Libraries/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Library library = db.Libraries.Find(id);
        //    db.Libraries.Remove(library);
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
