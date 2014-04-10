using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace PMTool.Repository
{
    public class EmailProcessor
    {
        public string SendEmail(string mailfrom, string mailto, string subject, string body)
        {

            var message = new MailMessage();
            var client = new SmtpClient();

            //MembershipUser user = Membership.GetUser(userName); //User Name = User Email
            //string confirmationGuid = user.ProviderUserKey.ToString();
            //string verifyUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/account/verify?ID=" + confirmationGuid;

            client.UseDefaultCredentials = false;
            try
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailFrom"]) && !String.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailFromPass"]))
                {
                    message = new MailMessage(ConfigurationManager.AppSettings["EmailFrom"].ToString(), mailto)
                    {
                        Subject = subject
                        //Body = "<b>Dear</b>" + "<b>" + userName + "</b>" + "," + "Your user name:" + userName + " and password:" + "Please confirm your account by clicking the following URL. " + verifyUrl

                    };
                    string Body = body;
                    //"<b>Dear</b>" + " " + "<b>" + firstName + "</b>" + " " + "<b>" + lastName + "</b>" + ",<br>" + "Your user name:" + " " + "<b>" + userName + "</b>" + " " + "and password:" + " " + "<b>" + password + "</b>." + "Please confirm your account by clicking on accept.<br><br><a href='" + verifyUrl + "'>" + "<img src='cid:imageId' align=baseline border=0 />" + "</a>";
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");
                    //LinkedResource imagelink = new LinkedResource(Server.MapPath("~/UploadedDocument/accept.png"));
                    //imagelink.ContentId = "imageId";
                    //imagelink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                    //htmlView.LinkedResources.Add(imagelink);
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
            catch (Exception exp)
            {
                //....//
            }
            return "";
        }
    }
}