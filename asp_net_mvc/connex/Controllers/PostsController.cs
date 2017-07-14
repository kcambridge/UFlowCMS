using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using connex.Auth;
using connex.Models;
using connex.ViewModels;

namespace connex.Controllers
{
    public class PostsController : Controller
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();
        private static int MAX_IMG_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings["ImageMaxSize"]);
        private static int MAX_VID_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings["VideoMaxSize"]);

        // GET: /Posts/
        public ActionResult Index()
        {
            var posts = db.Posts.Include(p => p.Collection);
            return View(posts.ToList());
        }

        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var postID = (from res in db.Posts where res.Post_ID == id select res.Post_ID).FirstOrDefault();
            if (postID == 0)
            {
                return HttpNotFound();
            }
            Post post = db.Posts.Find(postID);
            return View(post);
        }
        // GET: /Posts/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Post post = db.Posts.Find(id);
        //    if (post == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(post);
        //}

        // GET: /Posts/Create
        [AuthContentManagers]
        public ActionResult Create(int Collection_ID, int Page_ID)
        {
            //ViewBag.Collection_ID = new SelectList(db.Collections, "Collection_ID", "Collection_Title");
            PostCreateViewModel PostViewMod = new PostCreateViewModel();
            PostViewMod.Collection_ID = Collection_ID;
            PostViewMod.Page_ID = Page_ID;
            return View(PostViewMod);
        }

        // POST: /Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Create(string Header, string Summary, int Collection_ID, int Page_ID)
        {
            string pathRoot = "~/images/posts/thumbs/";
            string dbPathRoot = "images/posts/thumbs/";
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files["Thumbnail"];
                if (file.ContentLength > 0)
                {
                    if (file.ContentLength < MAX_IMG_SIZE)
                    {
                        byte[] imageSize = new byte[file.ContentLength];
                        file.InputStream.Read(imageSize, 0, (int)file.ContentLength);
                        string filePath = Server.MapPath(pathRoot) + file.FileName;
                        string fileName = file.FileName;
                        string fileNameNoEx = Path.GetFileNameWithoutExtension(file.FileName);
                        string ext = Path.GetExtension(file.FileName).ToLower();
                        if (System.IO.File.Exists(filePath))
                        {
                            int i = 1;
                            filePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                            while (System.IO.File.Exists(filePath))
                            {
                                i++;
                                filePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                                fileName = fileNameNoEx + i.ToString() + ext;
                            }

                        }
                        if (ext == ".jpg" || ext == ".png")
                        {
                            file.SaveAs(filePath);
                            Post post = new Post();
                            post.Header = Header;
                            post.Summary = Summary;
                            post.Collection_ID = Collection_ID;
                            post.Date_Added = DateTime.Now;
                            post.Added_By = User.Identity.Name.ToString(); ///TODO: Need Auth here
                            post.Archived = false;
                            post.Thumb_Path = dbPathRoot + fileName;
                            db.Posts.Add(post);
                            db.SaveChanges();

                            return RedirectToAction("Edit", new { Post_ID = post.Post_ID, Page_ID = Page_ID, Collection_ID = Collection_ID });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Only .jpg and .png images are supported");
                            PostCreateViewModel PostViewMod = new PostCreateViewModel();
                            PostViewMod.Collection_ID = Collection_ID;
                            PostViewMod.Page_ID = Page_ID;
                            return View(PostViewMod);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thumbnails must be less than 10 MB");
                        PostCreateViewModel PostViewMod = new PostCreateViewModel();
                        PostViewMod.Collection_ID = Collection_ID;
                        PostViewMod.Page_ID = Page_ID;
                        return View(PostViewMod);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "You must selecta 'Thumbnail'");
                    PostCreateViewModel PostViewMod = new PostCreateViewModel();
                    PostViewMod.Collection_ID = Collection_ID;
                    PostViewMod.Page_ID = Page_ID;
                    return View(PostViewMod);
                }
            }
            else
            {
                ModelState.AddModelError("", "You must select a 'Thumbnail'");
                PostCreateViewModel PostViewMod = new PostCreateViewModel();
                PostViewMod.Collection_ID = Collection_ID;
                PostViewMod.Page_ID = Page_ID;
                return View(PostViewMod);
            }

            
            
        }

        // GET: /Posts/Edit/5
        [AuthContentManagers]
        public ActionResult Edit(int Post_ID, int Page_ID, int Collection_ID)
        {
            Post post = db.Posts.Find(Post_ID);
            if (post == null)
            {
                return HttpNotFound();
            }
            PostEditViewModel postViewMod = new PostEditViewModel(post, Page_ID);
            
           return View(postViewMod);
        }

        // POST: /Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Edit(int Post_ID, string Header, string Summary, int Page_ID, bool Archived, int Collection_ID, bool Allow_Comments)
        {
            Post post = db.Posts.Find(Post_ID);

            if (post != null)
            {
                string filePath = "";
                string fileName = "";
                string fileNameNoEx = "";

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files["Thumbnail"];
                    if (file != null)
                    {
                        if (file.ContentLength > 0 && file.FileName != "")
                        {
                            if (file.ContentLength < MAX_IMG_SIZE)
                            {
                                byte[] imageSize = new byte[file.ContentLength];
                                file.InputStream.Read(imageSize, 0, (int)file.ContentLength);
                                filePath = Server.MapPath("~/images/posts/thumbs/") + file.FileName;
                                fileName = file.FileName;
                                fileNameNoEx = Path.GetFileNameWithoutExtension(file.FileName);
                                string ext = Path.GetExtension(file.FileName).ToLower();
                                if (ext.ToLower() == ".jpg" || ext.ToLower() == ".png")
                                {
                                    if (System.IO.File.Exists(filePath))
                                    {
                                        int i = 1;
                                        filePath = Server.MapPath("~/images/posts/thumbs/") + fileNameNoEx + i.ToString() + ext;
                                        while (System.IO.File.Exists(filePath))
                                        {
                                            i++;
                                            filePath = Server.MapPath("~/images/posts/thumbs/") + fileNameNoEx + i.ToString() + ext;
                                            fileName = fileNameNoEx + i.ToString() + ext;
                                        }

                                    }
                                    file.SaveAs(filePath);

                                    post.Thumb_Path = "images/posts/thumbs/" + fileName;
                                    
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Only .png and .jpg files are supported for 'Thumbnails'");
                                    PostEditViewModel postViewMod = new PostEditViewModel(post, Page_ID);
                                    return View(postViewMod);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Thumbnails must be less than 10 MB");
                                PostEditViewModel postViewMod = new PostEditViewModel(post, Page_ID);
                                return View(postViewMod);
                            }
                        }
                        //else
                        //{
                        //    ModelState.AddModelError("", "You must select a 'Thumbnail'");
                        //    PostEditViewModel postViewMod = new PostEditViewModel(post, Page_ID);
                        //    return View(postViewMod);
                        //}
                    }
                    //else
                    //{
                    //    ModelState.AddModelError("", "You must select a 'Thumbnail'");
                    //    PostEditViewModel postViewMod = new PostEditViewModel(post, Page_ID);
                    //    return View(postViewMod);
                    //}
                }
                //else
                //{
                //    ModelState.AddModelError("", "You must select a 'Thumbnail'");
                //    PostEditViewModel postViewMod = new PostEditViewModel(post, Page_ID);
                //    return View(postViewMod);
                //}

                post.Header = Header;
                post.Summary = Summary;
                if (!post.Archived && Archived)
                {
                    post.Archive_By = User.Identity.Name.ToString();///TODD authentication
                    post.Date_Archive = DateTime.Now;
                }
                post.Archived = Archived;
                post.Last_Updated = DateTime.Now;
                post.Updated_By = User.Identity.Name.ToString();///TODO: auth
                post.Allow_Comments = Allow_Comments;

                db.SaveChanges();
                return RedirectToAction("Edit", new { Post_ID = Post_ID, Page_ID = Page_ID, Collection_ID = Collection_ID });
            }
            else
            {
                return HttpNotFound();
            }
            
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult AddSection(int Post_ID, int Page_ID, int Collection_ID)
        {
            Section newSect = new Section();
            Post post = db.Posts.Find(Post_ID);
            if (post != null)
            {
                newSect.Post = post;
                db.Sections.Add(newSect);
                db.SaveChanges();
            }
            return RedirectToAction("Edit", new { Post_ID = Post_ID, Page_ID = Page_ID, Collection_ID = Collection_ID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult RemoveSection(int Post_ID, int Page_ID, int Collection_ID, int Section_ID)
        {
            Section sect = db.Sections.Find(Section_ID);
            if (sect != null)
            {
                db.Sections.Remove(sect);
                db.SaveChanges();
            }
            return RedirectToAction("Edit", new { Post_ID = Post_ID, Page_ID = Page_ID, Collection_ID = Collection_ID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult EditSection(int Post_ID, int Collection_ID, int Page_ID, int Section_ID, bool Has_Heading, bool Has_Media, bool Has_Text, bool Has_Hyperlink, string Sub_Header, int? Media_Type_ID,int? Width_Mode_ID, string Text_Block, string Hyperlink, string HyperlinkText)
        {
            Section sect = db.Sections.Find(Section_ID);
            if (sect != null)
            {
                if (Has_Heading)
                {
                    sect.Sub_Header = Convert.ToString(Sub_Header);
                }
                else
                {
                    sect.Sub_Header = null;
                }
                if (Has_Media)
                {
                    sect.Media_Type_ID = Convert.ToInt32(Media_Type_ID);
                    sect.Media_Width_Mode_ID = Convert.ToInt32(Width_Mode_ID);
                    string pathRoot = "";
                    if (sect.Media_Type_ID == 1)
                    {
                        pathRoot = "~/images/posts/";
                    }
                    else if (sect.Media_Type_ID == 2)
                    {
                        pathRoot = "~/video/posts/";
                    }

                    if (Request.Files.Count > 0)
                    {
                        
                        HttpPostedFileBase file = Request.Files["Media_Path"];
                        if (file != null)
                        {
                            byte[] imageSize = new byte[file.ContentLength];
                            if (file.ContentLength > 0)
                            {
                                file.InputStream.Read(imageSize, 0, (int)file.ContentLength);
                                string filePath = Server.MapPath(pathRoot) + file.FileName;
                                string fileName = file.FileName;
                                string fileNameNoEx = Path.GetFileNameWithoutExtension(file.FileName);
                                string ext = Path.GetExtension(file.FileName).ToLower();
                                ///TODO: Handle video uploads. You will need to accept two files in this case, unless you implement ffmpeg converions service
                                if ((sect.Media_Type_ID == 1 && (ext.ToLower() == ".jpg" || ext.ToLower() == ".png")))
                                {
                                    if (file.ContentLength < MAX_IMG_SIZE)
                                    {
                                        if (System.IO.File.Exists(filePath))
                                        {
                                            int i = 1;
                                            filePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                                            while (System.IO.File.Exists(filePath))
                                            {
                                                i++;
                                                filePath = Server.MapPath(pathRoot) + fileNameNoEx + i.ToString() + ext;
                                                fileName = fileNameNoEx + i.ToString() + ext;
                                            }

                                        }
                                        file.SaveAs(filePath);
                                        string dbPath = "";
                                        if (sect.Media_Type_ID == 1)
                                        {
                                            dbPath = "images/posts/" + fileName;
                                        }
                                        ///TODO: Need to handle video files
                                        //else if (sect.Media_Type_ID == 2)
                                        //{
                                        //    dbPath = "video/posts/" + fileNameNoEx;
                                        //}
                                        sect.Media_Path = dbPath;
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "Image Files must be less than 10 MB");
                                        Post post = db.Posts.Find(Post_ID);
                                        if (post == null)
                                        {
                                            return HttpNotFound();
                                        }
                                        PostEditViewModel postViewMod = new PostEditViewModel(post, Page_ID);
                                        return View("Edit", postViewMod);
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Only .jpg and .png Image Files are supported");
                                    Post post = db.Posts.Find(Post_ID);
                                    if (post == null)
                                    {
                                        return HttpNotFound();
                                    }
                                    PostEditViewModel postViewMod = new PostEditViewModel(post, Page_ID);
                                    return View("Edit", postViewMod);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "You must select a Media File");
                            Post post = db.Posts.Find(Post_ID);
                            if (post == null)
                            {
                                return HttpNotFound();
                            }
                            PostEditViewModel postViewMod = new PostEditViewModel(post, Page_ID);
                            return View("Edit", postViewMod);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "You must select a Media File");
                        Post post = db.Posts.Find(Post_ID);
                        if (post == null)
                        {
                            return HttpNotFound();
                        }
                        PostEditViewModel postViewMod = new PostEditViewModel(post, Page_ID);
                        return View("Edit", postViewMod);
                    }

                }
                else
                {
                    sect.Media_Type_ID = null;
                    sect.Media_Width_Mode_ID = null;
                    sect.Media_Path = null;
                }

                if (Has_Text)
                {
                    sect.Text_Block = Convert.ToString(Text_Block);
                }
                else
                {
                    sect.Text_Block = null;
                }

                if (Has_Hyperlink)
                {
                    sect.Hyperlink = Convert.ToString(Hyperlink);
                    sect.Hyperlink_Text = Convert.ToString(HyperlinkText);
                }
                else
                {
                    sect.Hyperlink = null;
                    sect.Hyperlink_Text = null;
                }
                db.SaveChanges();
                return RedirectToAction("Edit", new { Post_ID = Post_ID, Page_ID = Page_ID, Collection_ID = Collection_ID });
            }
            else
            {
                return HttpNotFound();
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthContentManagers]
        public ActionResult Archive(int Collection_ID, int Page_ID, int Post_ID)
        {
            Post post = db.Posts.Find(Post_ID);
            post.Archived = true;
            post.Archive_By = User.Identity.Name.ToString();
            post.Date_Archive = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Edit", "Collections", new { Page_ID = Page_ID, Collection_ID = Collection_ID});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult AddComment(int Post_ID, string Comment)
        {
            Post post = db.Posts.Find(Post_ID);
            if (post != null)
            {
                if (post.Allow_Comments)
                {
                    Comment comment = new Comment();
                    comment.Archived = false;
                    comment.Comment1 = Comment;
                    comment.Date_Submitted = DateTime.Now;
                    comment.Submitted_By = User.Identity.Name;
                    comment.Post = post;
                    db.Comments.Add(comment);
                    db.SaveChanges();
                    return RedirectToAction("View", new { id = Post_ID });
                }
                else
                {
                    ModelState.AddModelError("", "Comments have been disabled for this post");
                    return View("View",post);

                }
            }
            else
            {
                return HttpNotFound();
            }
            
            
        }

        // GET: /Posts/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Post post = db.Posts.Find(id);
        //    if (post == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(post);
        //}

        //// POST: /Posts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Post post = db.Posts.Find(id);
        //    db.Posts.Remove(post);
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
