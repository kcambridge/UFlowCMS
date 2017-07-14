using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using connex.Models;

namespace connex.Auth
{
    public class AuthContentManagers : AuthorizeAttribute
    {
        // Custom property
        public string AccessLevel { get; set; }
        

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            CONNEXDBEntities db = new CONNEXDBEntities();
            //var isAuthorized = base.AuthorizeCore(httpContext);
            //if (!isAuthorized)
            //{
            //    return false;
            //}
            string trace = "1-";
            bool authorized = false;
            //get current username
            string userName = httpContext.User.Identity.Name.ToString();
            
            Admin currentUser = (Admin)db.Admins.Where(x => x.Network_ID == userName).FirstOrDefault();
            db.Entry(currentUser).Reload();//forces a refresh of EF data
            if (currentUser != null)
            {
                trace += "1a-";
                //get user roles
                foreach (Role role in currentUser.Roles.OrderBy(x => x.Role_ID))
                {
                    trace += "2-";
                    //allow any admin
                    if (role.Role1 == "Admin")
                    {
                        trace += "2a-";
                        authorized = true;
                        break;
                    }
                    else if (role.Role1 == "ContentManager")
                    {
                        //get current request url and extract Page_ID
                        trace += "3-";
                        string url = httpContext.Request.Url.AbsoluteUri;
                        if (httpContext.Request.QueryString["Page_ID"] != null)
                        {
                            trace += "3-";
                            try
                            {
                                trace += "3a-";
                                int Page_ID = Convert.ToInt32(httpContext.Request.QueryString["Page_ID"]);
                                //lookup access in db
                                if (currentUser.PageAdminAssigns.Any(x => x.Page_ID == Page_ID))
                                {
                                    trace += "4-";
                                    authorized = true;
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                trace += "5-";
                                //do nothing
                            }

                        }
                        else
                        {
                            trace += "6-";
                            int altIndex = url.LastIndexOf("/Pages/Edit/");
                            if (altIndex > -1)
                            {
                                trace += "7-";
                                string[] parts = url.Substring(altIndex + 12, url.Length - (altIndex + 12)).Split('?');
                                try
                                {
                                    trace += "8-";
                                    int Page_ID = Convert.ToInt32(parts[0].Trim());
                                    //lookup access in db
                                    //List<PageAdminAssign> authPages = (List<PageAdminAssign>)db.PageAdminAssigns.Where(x => x.Admin_ID == currentUser.Admin_ID).ToList();
                                    if (currentUser.PageAdminAssigns.Any(x => x.Page_ID == Page_ID))
                                    {
                                        trace += "9-";
                                        authorized = true;
                                        break;
                                    }
                                }
                                catch (Exception)
                                {
                                    trace += "10-";
                                    //do nothing
                                }
                            }
                            else if (httpContext.Request.Form["Page_ID"] != null)
                            {
                                trace += "11-";
                                int Page_ID = Convert.ToInt32(httpContext.Request.Form["Page_ID"]);
                                if (currentUser.PageAdminAssigns.Any(x => x.Page_ID == Page_ID))
                                {
                                    trace += "12-";
                                    authorized = true;
                                    break;
                                }
                            }
                            trace += "13-";
                        }
                        trace += "14-";

                    }
                    trace += "15-";

                }
            }
            trace += "16-";
            if (!authorized)
            {
                if (!System.IO.Directory.Exists("C:/ConnexLogs/"))
                {
                    System.IO.Directory.CreateDirectory("C:/ConnexLogs/");
                }
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("C:/ConnexLogs/AuthLogs.txt", true))
                {
                    string roles = "";
                    string pageIDs = "";
                    foreach (Role role in currentUser.Roles)
                    {
                        roles += role.Role1 + ",";
                    }
                    foreach (PageAdminAssign page in currentUser.PageAdminAssigns)
                    {
                        pageIDs += page.Page.Page_Name + ",";
                    }

                    file.WriteLine("Authorization failed for " + userName + " on the url " + httpContext.Request.Url + " with the roles " + roles + " and the assigned pages " + pageIDs + " at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "\n\nTrace:\n" + trace + "\n\n");
                }
            }
            else
            {
                 if (!System.IO.Directory.Exists("C:/ConnexLogs/"))
                {
                    System.IO.Directory.CreateDirectory("C:/ConnexLogs/");
                }
                 using (System.IO.StreamWriter file = new System.IO.StreamWriter("C:/ConnexLogs/AuthLogs.txt", true))
                 {
                     file.WriteLine("Authorization successful for "+userName+" on the url "+httpContext.Request.Url+" at "+ DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()+"\n\n");
                 }
            }
            db.Dispose();
            return authorized;
        }
    }
}