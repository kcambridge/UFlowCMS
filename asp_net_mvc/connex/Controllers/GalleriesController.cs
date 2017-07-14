using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using connex.Models;
using connex.ViewModels;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Drawing.Imaging;
using connex.Auth;
using System.Text.RegularExpressions;
namespace connex.Controllers
{
    public class GalleriesController : Controller
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();
        private static int GALLERY_IMG_WIDTH = Convert.ToInt32(ConfigurationManager.AppSettings["GalleryImageWidth"]);
        private static int GALLERY_IMG_HEIGHT = Convert.ToInt32(ConfigurationManager.AppSettings["GalleryImageHeight"]);
        private static string IMAGE_BASE_ROOT = ConfigurationManager.AppSettings["ImageUploadBaseRoot"];
        
        

        // GET: /Galleries/
        public ActionResult Index()
        {
            return View(db.Galleries.ToList());
        }
        // GET: /Galleries/View/5
        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gallery gallery = db.Galleries.Find(id);
            if (gallery == null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }
        // GET: /Galleries/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Gallery gallery = db.Galleries.Find(id);
        //    if (gallery == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(gallery);
        //}

      

        // GET: /Galleries/Create
        [AuthContentManagers]
        public ActionResult Create(int Page_ID)
        {
            GalleryCreateViewModel viewMod = new GalleryCreateViewModel(Page_ID);
            return View(viewMod);
        }

        // POST: /Galleries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Create(int Page_ID, string Gallery_Name)
        {
            if (ModelState.IsValid)
            {
                Gallery gal = new Gallery();
                gal.Gallery_Name = Gallery_Name;
                gal.Date_Added = DateTime.Now;
                gal.Added_By = User.Identity.Name.ToString();///TODO: auth
                gal.Archived = false;
                db.Galleries.Add(gal);
                db.SaveChanges();

                PageGalleryAssign galAss = new PageGalleryAssign();
                galAss.Gallery = gal;
                galAss.Page_ID = Page_ID;
                galAss.Date_Added = DateTime.Now;
                galAss.Added_By = User.Identity.Name.ToString();///TODO: auth
                galAss.Archived = false;
                db.PageGalleryAssigns.Add(galAss);

                db.SaveChanges();
                return RedirectToAction("Edit", new { Page_ID = Page_ID, Gallery_ID = gal.Gallery_ID});
            }
            else
            {
                ModelState.AddModelError("", "Error. Invalid Model State. Please try again.");
                GalleryCreateViewModel viewMod = new GalleryCreateViewModel(Page_ID);
                viewMod.Gallery_Name = Gallery_Name;
                return View(viewMod);
            }

            
        }

        // GET: /Galleries/Edit/5
        [AuthContentManagers]
        public ActionResult Edit(int Page_ID,int Gallery_ID)
        {
            
            Gallery gallery = db.Galleries.Find(Gallery_ID);
            if (gallery == null)
            {
                return HttpNotFound();
            }
            GalleryEditViewModel galViewMod = new GalleryEditViewModel(Page_ID,gallery);
            cleanUpUploads(gallery);
            ViewBag.Success_Uploads = "";
            ViewBag.Failed_Uploads = "";
            ViewBag.Upload_Counts = "";
            return View(galViewMod);
        }
        //deletes the temp folder in the respective gallery folder. This folder is used to store uploaded images before they are re-sized.
        //unable to delete while uploading since they are in use by UI. 
        private void cleanUpUploads(Gallery gal)
        {///TODO: needs to be rewrittedn
            try
            {
                Regex r = new Regex("[^a-zA-Z0-9]");
                string dirName = r.Replace(gal.Gallery_Name, "");
                string pathRootTemp = "~/" + IMAGE_BASE_ROOT + dirName + "/uploadTemp/";
                if (System.IO.Directory.Exists(Server.MapPath(pathRootTemp)))
                {
                    DirectoryInfo directory = new DirectoryInfo(Server.MapPath(pathRootTemp));
                    foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
                    System.IO.Directory.Delete(Server.MapPath(pathRootTemp));
                }
            }
            catch (Exception)
            {
                
                
            }
            

        }

        // POST: /Galleries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Edit([Bind(Include="Gallery_ID,Gallery_Name,Date_Added,Added_By,Last_Updated,Updated_By,Date_Archive,Archive_By,Archived")] Gallery gallery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gallery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gallery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult RemoveImage(int Page_ID, int Gallery_ID, int Photo_ID)
        {
            Photo photo = db.Photos.Find(Photo_ID);
            photo.Archived = true;
            photo.Archive_By = User.Identity.Name.ToString();///TODO:auth;
            photo.Date_Archive = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Edit", new{Page_ID = Page_ID, Gallery_ID = Gallery_ID});
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult AddImages(HttpPostedFileBase[] files, int Page_ID, int Gallery_ID, string Gallery_Name)
        {
            string successUploads = "<h3>Successful Uploads</h3><hr /><table role=\"presentation\" class=\"table table-striped\"><tbody class=\"files\">";
            string failedUploads = "<h3>Failed Uploads</h3><hr /><table role=\"presentation\" class=\"table table-striped\"><tbody class=\"files\">";
            int success = 0;
            int failed = 0;
            Gallery gal = db.Galleries.Find(Gallery_ID);
            foreach (HttpPostedFileBase ImgFile in files)
            {
                if (ImgFile != null)
                {
                    Regex r = new Regex("[^a-zA-Z0-9]");
                    string dirName = r.Replace(Gallery_Name, "");
                    string pathRoot = "~/" + IMAGE_BASE_ROOT + dirName + "/";
                    string pathRootTemp = "~/" + IMAGE_BASE_ROOT + dirName + "/uploadTemp/";
                    string dbRoot = IMAGE_BASE_ROOT + dirName + "/";
                    string displayRoot = "/" + IMAGE_BASE_ROOT + dirName + "/";
                    string fileNameNoExt = System.IO.Path.GetFileNameWithoutExtension(ImgFile.FileName);
                    string ext = System.IO.Path.GetExtension(ImgFile.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".png")
                    {
                        try
                        {
                            if (!System.IO.Directory.Exists(Server.MapPath(pathRoot)))
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath(pathRoot));
                            }
                            if (!System.IO.Directory.Exists(Server.MapPath(pathRootTemp)))
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath(pathRootTemp));
                            }

                            string filePath = Server.MapPath(pathRoot) + ImgFile.FileName;
                            string filePathTemp = Server.MapPath(pathRootTemp) + ImgFile.FileName;
                            string displayPath = displayRoot + ImgFile.FileName;
                            string retName = "";
                            try
                            {
                                retName = UploadFile(ImgFile, pathRootTemp, fileNameNoExt, ext);
                                filePathTemp = Server.MapPath(pathRootTemp) + retName + ext;
                                filePath = Server.MapPath(pathRoot) + retName + ext;
                                displayPath = displayRoot + retName + ext;
                                fileNameNoExt = retName;

                            }
                            catch (Exception ex)
                            {
                                return Json(new { error = "Upload Failed", message = ex.Message });
                            }

                            //scaling image
                            Image origImg = Bitmap.FromFile(filePathTemp);
                            int width = origImg.Width;
                            int height = origImg.Height;

                            if (width > height)
                            {
                                if (width > GALLERY_IMG_WIDTH)
                                {
                                    double origDouble = Convert.ToDouble(width);
                                    double maxDouble = Convert.ToDouble(GALLERY_IMG_WIDTH);
                                    double perc = 1 - ((origDouble - maxDouble) / origDouble);
                                    width = Convert.ToInt32(Convert.ToDouble(width) * perc);
                                    height = Convert.ToInt32(Convert.ToDouble(height) * perc);
                                    Size newSize = new Size(width, height);
                                    Image newImage = (Image)(new Bitmap(origImg, newSize));
                                    //System.IO.File.Delete(filePath);
                                    if (System.IO.File.Exists(filePath))
                                    {
                                        int i = 1;
                                        filePath = Server.MapPath(pathRoot) + fileNameNoExt + i.ToString() + ext;
                                        retName = fileNameNoExt + i.ToString();

                                        while (System.IO.File.Exists(filePath))
                                        {
                                            i++;
                                            filePath = Server.MapPath(pathRoot) + fileNameNoExt + i.ToString() + ext;
                                            retName = fileNameNoExt + i.ToString();
                                        }
                                        fileNameNoExt = retName;
                                        displayPath = displayRoot + retName + ext;

                                    }
                                    if (ext == ".jpg")
                                    {
                                        newImage.Save(filePath, ImageFormat.Jpeg);
                                    }
                                    else if (ext == ".png")
                                    {
                                        newImage.Save(filePath, ImageFormat.Png);
                                    }
                                    newImage.Dispose();

                                }
                                else
                                {
                                    retName = UploadFile(ImgFile, pathRoot, fileNameNoExt, ext);
                                    filePath = Server.MapPath(pathRoot) + retName + ext;
                                    displayPath = displayRoot + retName + ext;
                                    fileNameNoExt = retName;
                                }

                            }
                            else
                            {
                                if (height > GALLERY_IMG_HEIGHT)
                                {
                                    double origDouble = Convert.ToDouble(height);
                                    double maxDouble = Convert.ToDouble(GALLERY_IMG_HEIGHT);
                                    double perc = 1 - ((origDouble - maxDouble) / origDouble);
                                    width = Convert.ToInt32(Convert.ToDouble(width) * perc);
                                    height = Convert.ToInt32(Convert.ToDouble(height) * perc);
                                    Size newSize = new Size(width, height);
                                    Image newImage = (Image)(new Bitmap(origImg, newSize));

                                    if (System.IO.File.Exists(filePath))
                                    {
                                        int i = 1;
                                        filePath = Server.MapPath(pathRoot) + fileNameNoExt + i.ToString() + ext;
                                        retName = fileNameNoExt + i.ToString();
                                        while (System.IO.File.Exists(filePath))
                                        {
                                            i++;
                                            filePath = Server.MapPath(pathRoot) + fileNameNoExt + i.ToString() + ext;
                                            retName = fileNameNoExt + i.ToString();
                                        }
                                        fileNameNoExt = retName;
                                        displayPath = displayRoot + retName + ext;
                                    }

                                    if (ext == ".jpg")
                                    {
                                        newImage.Save(filePath, ImageFormat.Jpeg);
                                    }
                                    else if (ext == ".png")
                                    {
                                        newImage.Save(filePath, ImageFormat.Png);
                                    }
                                    newImage.Dispose();
                                }
                                else
                                {
                                    retName = UploadFile(ImgFile, pathRoot, fileNameNoExt, ext);
                                    filePath = Server.MapPath(pathRoot) + retName + ext;
                                    displayPath = displayRoot + retName + ext;
                                    fileNameNoExt = retName;
                                }
                            }
                            origImg.Dispose();

                            Photo photo = new Photo();
                            photo.File_Path = dbRoot + fileNameNoExt + ext;
                            photo.Thumb_Path = dbRoot + fileNameNoExt + ext;
                            photo.Archived = false;
                            photo.Added_By = User.Identity.Name.ToString();///TODO:auth
                            photo.Date_Added = DateTime.Now;
                            photo.Gallery = gal;
                            db.Photos.Add(photo);

                            successUploads += "<tr class=\"table table-striped\"><td class=\"col-md-6\"><span class=\"preview\"><div class=\"col-sm-6 col-md-4\"><div class=\"thumbnail\"><a rel=\"lightbox-cats\" href=\"" + "/" + dbRoot + fileNameNoExt + ext + "\" title=\"" + ImgFile.FileName + "\" target=\"_blank\"><img class=\"img-responsive\" alt=\"No Image Found\" src=\"" + "/" + dbRoot + fileNameNoExt + ext + "\"></img></a></div></div></span></td><td class=\"col-md-2\"><p class=\"name\">" + ImgFile.FileName + "</p></td><td class=\"col-md-2\"><img class=\"img-responsive col-md-6\" src=\"../images/green_tick.png\" alt\"No Image Found\"></img></td></tr>";
                            success++;
                            //var files = new List<object>();
                            //files.Add(new { url = displayPath.Replace("\\", "\\/"), thumbnailUrl = displayPath.Replace("\\", "\\/"), name = ImgFile.FileName, type = "image/jpeg", size = ImgFile.ContentLength, deleteUrl = displayPath.Replace("\\", "\\/"), deleteType = "DELETE" });
                            //return Json(files, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception EX)
                        {
                            failedUploads += "<tr class=\"table table-striped\"><td class=\"col-md-6\"><span class=\"label label-danger\">Error:</span>&nbsp;" + EX.Message + "</td><td class=\"col-md-2\"><p class=\"name\">" + ImgFile.FileName + "</p></td><td class=\"col-md-2\"><img class=\"img-responsive col-md-6\" src=\"../images/red_x.png\" alt\"No Image Found\"></img></td></tr>";
                            failed++;
                        }

                    }
                    else
                    {
                        failedUploads += "<tr class=\"table table-striped\"><td class=\"col-md-6\"><span class=\"label label-danger\">Error:</span>&nbsp;" + ext + " is not a supported image format</td><td class=\"col-md-2\"><p class=\"name\">" + ImgFile.FileName + "</p></td><td class=\"col-md-2\"><img class=\"img-responsive col-md-6\" src=\"../images/red_x.png\" alt\"No Image Found\"></img></td></tr>";
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
            GalleryEditViewModel vm = new GalleryEditViewModel(Page_ID, gal);
            return View("Edit", vm);
            
        }

        private string UploadFile(HttpPostedFileBase file, string pathRoot, string fileNameNoEx, string ext)
        {
            string retFileName = fileNameNoEx;
            try
            {
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
                        retFileName = fileNameNoEx + i.ToString();
                    }

                }

                file.SaveAs(filePath);
                
            }
            catch (Exception ex)
            {

                throw;
            }
            return retFileName;
            
        }

        // GET: /Galleries/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Gallery gallery = db.Galleries.Find(id);
        //    if (gallery == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(gallery);
        //}

        // POST: /Galleries/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Gallery gallery = db.Galleries.Find(id);
        //    db.Galleries.Remove(gallery);
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
