using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using connex.ViewModels;

namespace connex.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        public ActionResult Index()
        {
            return View("Login");
        }

        public ActionResult Login()
        {
            LoginViewModel viewMod = new LoginViewModel();
            return View(viewMod);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel viewMod, string ReturnUrl)
        {
            bool valid = false;
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                ///TODO: remove this. This is for testing of authorization errors for the user below
                //if (viewMod.Username == "mmitchell")
                //{
                  //  valid = true;
                //}
                //else
                //{
                    valid = context.ValidateCredentials(viewMod.Username, viewMod.Password);
                //}
            }
            if (valid)
            {
                FormsAuthentication.SetAuthCookie(viewMod.Username,viewMod.Remember_Me);
               
                if (ReturnUrl != null)
                {
                    if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/")
                   && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Pages");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Pages");
                }
            }
            else
            {
                ModelState.AddModelError("", "The Username or Password provided is incorrect.");
                viewMod.Password = "";
                return View(viewMod);
            }
            
        }

        public ActionResult Logout(string returnUrl)
        {
            FormsAuthentication.SignOut();

            if (returnUrl != null)
            {
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
               && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Pages");
                }
            }
            else
            {
                return RedirectToAction("Index", "Pages");
            }
        }
	}
}