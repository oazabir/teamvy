using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace PMTool.Repository
{
    public class EmailProcessor
    {
        public string SendEmail(string mailfrom, List<string> mailto, List<string> ccto, List<string> bccto, string subject, string body)
        {

            var message = new MailMessage();
            var client = new SmtpClient();
            MailAddress fromAddress = null;

            //MembershipUser user = Membership.GetUser(userName); //User Name = User Email
            //string confirmationGuid = user.ProviderUserKey.ToString();
            //string verifyUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/account/verify?ID=" + confirmationGuid;

            client.UseDefaultCredentials = false;
            try
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailFrom"]) && !String.IsNullOrEmpty(ConfigurationManager.AppSettings["EmailFromPass"]))
                {
                    fromAddress = new MailAddress(ConfigurationManager.AppSettings["EmailFrom"]);
                    message.From = fromAddress;
                    foreach (string emailto in mailto)
                    {
                        message.To.Add(emailto);
                    }

                    message.Subject = subject;
                 
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
  
                    message.AlternateViews.Add(htmlView);
                    //message.BodyEncoding = Encoding.UTF8;
                    //message.AlternateViews.Add(plainview);
                    //SmtpClient smtp = new SmtpClient();
                    //smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;

  
                    //message.IsBodyHtml = true;
                    //string html = RegisterMessageBodyHtml(recvrName, verCode,NewUserID);
                    //string plain = RegisterMessageBodyPlaintext(recvrName, verCode, NewUserID);
                    //message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, new ContentType("text/html"));
                    //message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plain, new ContentType("text/plain"));

                    //message.BodyFormat = MailFormat.Html;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    message.Headers.Add("Reply-To", "mahedee.hasan@bs-23.com");
                    //message.ReplyToList.Add(new MailAddress(fromAddress));

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
            finally
            {
                message.Dispose();
                client.Dispose();
            }
            return "";
        }
    }
}