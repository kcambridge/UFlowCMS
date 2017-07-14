using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using connex.Models;

namespace connex.ajax
{
    /// <summary>
    /// Retrieves content ID's based on the content type selected
    /// </summary>
    public class GetContentIds : IHttpHandler
    {
        private CONNEXDBEntities db = new CONNEXDBEntities();
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.QueryString["Content_Type_ID"] != null)
            {
                int id = 0;
                try
                {
                    id = Convert.ToInt32(context.Request.QueryString["Content_Type_ID"]);
                    string resp = "";
                    if (id == 1)//Post
                    {
                        foreach (var post in db.Posts.Where(x => x.Archived == false).OrderByDescending(x => x.Date_Added).ToList())
                        {
                            resp += "<option value=\"" + post.Post_ID + "\">" + post.Header + "</option>";
                        }
                    }
                    else if (id == 2)//Gallery
                    {
                        foreach (var gal in db.Galleries.Where(x => x.Archived == false).OrderByDescending(x => x.Date_Added).ToList())
                        {
                            resp += "<option value=\"" + gal.Gallery_ID + "\">" + gal.Gallery_Name + "</option>";
                        }
                    }
                    else if (id == 3)//Video
                    {
                        foreach (var vid in db.Videos.Where(x => x.Archived == false).OrderByDescending(x => x.Date_Added).ToList())
                        {
                            resp += "<option value=\"" + vid.Video_ID + "\">" + vid.Title_Text + "</option>";
                        }
                    }
                    else if (id == 4)//Event
                    {
                        foreach (var ev in db.Events.Where(x => x.Archived == false).OrderByDescending(x => x.Date_Added).ToList())
                        {
                            resp += "<option value=\"" + ev.Event_ID + "\">" + ev.Event_Name + "</option>";
                        }
                    }
                    else if (id == 5)//Page
                    {
                        foreach (var page in db.Pages.Where(x => x.Archived == false).OrderBy(x => x.Title_Text).ToList())
                        {
                            resp += "<option value=\"" + page.Page_ID + "\">" + page.Title_Text + "</option>";
                        }
                    }
                    else
                    {
                        resp = "Invalid content ID";
                    }
                    context.Response.Write(resp);
                }
                catch (Exception)
                {

                    context.Response.Write("Error loading content");
                }

            }
            else
            {
                context.Response.Write("Invalid content ID");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}