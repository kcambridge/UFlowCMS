using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using connex.Models;
using connex.ViewModels;

namespace connex
{
    public class FileUploadController : Controller
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();
        private static int GALLERY_IMG_WIDTH = Convert.ToInt32(ConfigurationManager.AppSettings["GalleryImageWidth"]);
        private static int GALLERY_IMG_HEIGHT = Convert.ToInt32(ConfigurationManager.AppSettings["GalleryImageHeight"]);
        private static string IMAGE_BASE_ROOT = ConfigurationManager.AppSettings["ImageUploadBaseRoot"];

        // GET: /FileUpload/
        public ActionResult Index()
        {
            var photos = db.Photos.Include(p => p.Gallery);
            return View(photos.ToList());
        }

        // GET: /FileUpload/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase[] files, string Gallery_Name, int Gallery_ID, int Page_ID)
        {
            Gallery gal = db.Galleries.Find(Gallery_ID);
            try
            {
                /*Lopp for multiple files*/
                string retView = "";
                foreach (HttpPostedFileBase ImgFile in files)
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
                        db.SaveChanges();

                        //var files = new List<object>();
                        //files.Add(new { url = displayPath.Replace("\\", "\\/"), thumbnailUrl = displayPath.Replace("\\", "\\/"), name = ImgFile.FileName, type = "image/jpeg", size = ImgFile.ContentLength, deleteUrl = displayPath.Replace("\\", "\\/"), deleteType = "DELETE" });
                        //return Json(files, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { error = "Upload Failed", message = "Not a  supported image format" });
                    }
                    
                    ///*Geting the file name*/
                    //string filename = System.IO.Path.GetFileName(file.FileName);
                    ///*Saving the file in server folder*/
                    //retView += "<img src=\"images/"+filename+"\" alt=\"Image Not Found\"/>";
                    //file.SaveAs(Server.MapPath("~/images/" + filename));
                    //string filepathtosave = "Images/" + filename;
                    ///*HERE WILL BE YOUR CODE TO SAVE THE FILE DETAIL IN DATA BASE*/
                    retView += "<img src=\"/" + dbRoot + fileNameNoExt + ext + "\" alt=\"Image Not Found\"/>";
                }

                ViewBag.Message = retView;
            }
            catch
            {
                ViewBag.Message = "Error while uploading the files.";
            }
            GalleryEditViewModel vm = new GalleryEditViewModel(Page_ID, gal);
            return View();
        }

        private string UploadFile(HttpPostedFileBase file, string pathRoot, string fileNameNoEx, string ext)
        {
            string retFileName = fileNameNoEx;
            try
            {
                byte[] imageSize = new byte[file.ContentLength];
                file.InputStream.Read(imageSize, 0, (int)file.ContentLength);
                string filePath = Server.MapPath(pathRoot) + fileNameNoEx + ext;

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

        // GET: /FileUpload/Create
        public ActionResult Create()
        {
            ViewBag.Gallery_ID = new SelectList(db.Galleries, "Gallery_ID", "Gallery_Name");
            return View();
        }

        // POST: /FileUpload/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Photo_ID,File_Path,Thumb_Path,Date_Added,Added_By,Date_Archive,Archive_By,Archived,Gallery_ID")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Photos.Add(photo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Gallery_ID = new SelectList(db.Galleries, "Gallery_ID", "Gallery_Name", photo.Gallery_ID);
            return View(photo);
        }

        // GET: /FileUpload/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            ViewBag.Gallery_ID = new SelectList(db.Galleries, "Gallery_ID", "Gallery_Name", photo.Gallery_ID);
            return View(photo);
        }

        // POST: /FileUpload/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Photo_ID,File_Path,Thumb_Path,Date_Added,Added_By,Date_Archive,Archive_By,Archived,Gallery_ID")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Gallery_ID = new SelectList(db.Galleries, "Gallery_ID", "Gallery_Name", photo.Gallery_ID);
            return View(photo);
        }

        // GET: /FileUpload/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: /FileUpload/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Photo photo = db.Photos.Find(id);
            db.Photos.Remove(photo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
