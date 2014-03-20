using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using PMTool.Filters;
using PMTool.Models;
using PMTool.Repository;
using System.Net.Mail;
using System.Configuration;

namespace PMTool.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : BaseController
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        // [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            Session.Abandon();

            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    string msg = WebSecurity.CreateAccount(model.UserName, model.UserName, model.Password, model.FirstName, model.LastName, false);

                    if (msg == String.Empty && !String.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableEmailNotification"]))
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableEmailNotification"])) //Check Enable Email Notification
                        {
                            try
                            {
                                //Send confirmation email added by Mahedee @ 23-01-14
                                SendConfirmationEmail(model.UserName,model.FirstName,model.LastName,model.Password);  
                            }
                            catch (Exception exp)
                            {
                                //throw (exp);
                            }
                        }
                    }

                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult verify(string Id)
        {
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            UnitOfWork unitofwork = new UnitOfWork();


            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            User user = unitofwork.UserRepository.GetUserByUserName(User.Identity.Name);
            if (user != null)
                ViewBag.HasLocalPassword = true;
            else
                ViewBag.HasLocalPassword = false;

            //OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount;
            UnitOfWork unitofwork = new UnitOfWork();
            User user = unitofwork.UserRepository.GetUserByUserName(User.Identity.Name);
            if (user != null)
                hasLocalAccount = true;
            else
                hasLocalAccount = false;
            //= OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (PMToolContext db = new PMToolContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }


        /// <summary>
        /// Invite people to the project 
        /// added by Mahedee @ 23-01-14
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult InvitePeople(string email)
        {
            //Just for test purpose
            if (!String.IsNullOrEmpty(email))
            {
                try
                {
                    if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableEmailNotification"]))
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableEmailNotification"])) //Check Enable Email Notification
                        {
                            SendInvitationEmail(email);
                        }
                    }
                }
                catch (Exception exp)
                {
                    throw (exp);
                }
                ViewBag.Message = "Invitation sent!";
            }
            return View(ViewBag);
        }


        [Authorize]
        public ActionResult EditProfile()
        {
            UnitOfWork unitofWork = new UnitOfWork();
            User user = unitofWork.UserRepository.GetUserByUserName(User.Identity.Name);
            return View(user);
        }


        [Authorize]
        [HttpPost]
        public ActionResult EditProfile(User user)
        {
            UnitOfWork unitofWork = new UnitOfWork();
            unitofWork.UserRepository.Update(user);
            unitofWork.Save();
            return View();
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //[Authorize]
        //[HttpPost]
        //public ActionResult ChangePassword(RegisterModel registerModel)
        //{
        //       if (ModelState.IsValid)
        //    {
        //    UnitOfWork unitofWork = new UnitOfWork();
        //    User user = unitofWork.UserRepository.GetUserByUserName(User.Identity.Name);

        //    //WebSecurity.User.Identity.Name
        //    MembershipUser membershipUser = Membership.GetUser(user.Username);
        //    membershipUser.ChangePassword(membershipUser.GetPassword(), registerModel.Password); //oldpassword, new password
        //    return View();
        //}

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        #region Invitation & Verification

        /// <summary>
        /// Send confirmation email added by Mahedee @ 23-01-14
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="password"></param>
        public void SendConfirmationEmail(string userName,string firstName,string lastName,string password)
        {
            var message = new MailMessage();
            var client = new SmtpClient();

            MembershipUser user = Membership.GetUser(userName); //User Name = User Email
            string confirmationGuid = user.ProviderUserKey.ToString();
            //string verifyUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/account/verify?ID=" + confirmationGuid;

            string verifyUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~") + "/account/verify?ID=" + confirmationGuid; 
            client.UseDefaultCredentials = false;
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailFrom"]) && !String.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailFromPass"]))
            {
                message = new MailMessage(ConfigurationManager.AppSettings["EmailFrom"].ToString(), userName)
                {
                    Subject = "Hi " + " "+ "" + firstName + "" + " "+"" + lastName + "" +" "+"Please confirm your email."
                    //Body = "<b>Dear</b>" + "<b>" + userName + "</b>" + "," + "Your user name:" + userName + " and password:" + "Please confirm your account by clicking the following URL. " + verifyUrl

                };
                string Body = "<b>Dear</b>" + " " + "<b>" + firstName + "</b>" + " " + "<b>" + lastName + "</b>" + ",<br>" + "Your user name:"+" "+"<b>"+ userName +"</b>"+" "+"and password:"+" "+"<b>"+ password +"</b>." + "Please confirm your account by clicking on accept.<br><br><a href='" + verifyUrl + "'>" + "<img src='cid:imageId' align=baseline border=0 />" + "</a>";
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");
                LinkedResource imagelink = new LinkedResource(Server.MapPath("~/UploadedDocument/accept.png"));
                imagelink.ContentId = "imageId";
                imagelink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                htmlView.LinkedResources.Add(imagelink);
                message.AlternateViews.Add(htmlView);
                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                message.IsBodyHtml = true;

                client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EmailFrom"].ToString(), ConfigurationManager.AppSettings["EmailFromPass"].ToString());
            }

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableSsl"]))
                client.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
            else
                client.EnableSsl = false;
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTP"]))
                client.Host = ConfigurationManager.AppSettings["SMTP"].ToString();

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTPPort"]))
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            else
                client.Port = 25; //Default port for SMTP

            //client.u
            client.Send(message);
        }


        /// <summary>
        /// Send invitation Mail added by Mahedee @ 23-01-14
        /// </summary>
        /// <param name="userEmail"></param>
        public void SendInvitationEmail(string userEmail)
        {
            
            string confirmationGuid = Guid.NewGuid().ToString();
            var message = new MailMessage();
            var client = new SmtpClient();
        
            //string verifyUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/account/verify?ID=" + confirmationGuid;
           
            string verifyUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/Register";
            client.UseDefaultCredentials = false;

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailFrom"]) && !String.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailFromPass"]))
            {
                message = new MailMessage(ConfigurationManager.AppSettings["EmailFrom"].ToString(), userEmail)
                {
                    Subject = "Invitation from PMTool"

                };
                string Body = "You are invited to PMTool. Please click on accept and signup.<br><br><a href='" + verifyUrl + "'>" + "<img src='cid:imageId' align=baseline border=0 />" + "</a>";
                 AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");
                 LinkedResource imagelink = new LinkedResource(Server.MapPath("~/UploadedDocument/accept.png"));
                 imagelink.ContentId = "imageId";
                 imagelink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                 htmlView.LinkedResources.Add(imagelink);
                 message.AlternateViews.Add(htmlView);
                 SmtpClient smtp = new SmtpClient();
                 smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                 message.IsBodyHtml = true;
                client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["EmailFrom"].ToString(), ConfigurationManager.AppSettings["EmailFromPass"].ToString());
            }


            
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.Credentials = new System.Net.NetworkCredential("testbd2014@gmail.com", "testbd@123");

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableSsl"]))
                client.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
            else
                client.EnableSsl = false;
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTP"]))
                client.Host = ConfigurationManager.AppSettings["SMTP"].ToString();

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTPPort"]))
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            else
                client.Port = 25; //Default port for SMTP

            //client.UseDefaultCredentials = false;
            client.Send(message);
        }
        #endregion
    }
}
