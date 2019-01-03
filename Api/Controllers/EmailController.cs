using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Http;
using System.Web.Http.Cors;
using Api.Models;
using System.Globalization;
using System.Net.Http;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EmailController : ApiController
    {

        public static string email = "bambino@entdesign.com";
        public static void Send(MailAddress FromEmailAddress, string ToEmailAddress, string CCAddress, string BccAddress, string Subject, string Message)
        {
            
            //SmtpClient mailClient = new SmtpClient("smtp.office365.com");
            SmtpClient mailClient = new SmtpClient("smtp.gmail.com");

            mailClient.Port = 587;
            mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailClient.UseDefaultCredentials = false;
            //mailClient.UseDefaultCredentials = true;
            mailClient.EnableSsl = true;
            mailClient.Credentials = new NetworkCredential(email, "/nx58r#X%!fZZAUd");
                                               
            MailMessage msg = new MailMessage();
            msg.Bcc.Add(BccAddress);
            msg.From = FromEmailAddress;
            msg.To.Add(ToEmailAddress);
            msg.CC.Add(CCAddress);
            msg.ReplyToList.Add(new MailAddress(email));
            //msg.ReplyToList.Add(new MailAddress("jpberding@gmail.com"));
            msg.Subject = Subject;
            msg.IsBodyHtml = true;
            msg.Body = Message;

            mailClient.ServicePoint.MaxIdleTime = 1;
            //mailClient.SendMailAsync(msg);
            mailClient.Send(msg);

            LogController.Add(Guid.Empty, "Email " + Subject + " Sent to " + ToEmailAddress, "Email", "Send", Guid.Empty, "Emails");

        }
        
        [HttpPost]
        public HttpResponseMessage SendTest([FromBody] EmptyViewModel data)
        {
            
            try
            {
                
                UnitOfWork unitOfWork = new UnitOfWork();

                EmailController.Send(new MailAddress(email),
                        "jpberding@gmail.com",
                        email,
                        email,
                        "Test",
                        "Testing");

                return Request.CreateResponse(HttpStatusCode.OK, new { });

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }
        
        public static EmailViewModel GetEmail(Guid emailId)
        {
            
            UnitOfWork unitOfWork = new UnitOfWork();
                 
            return unitOfWork.EmailRepository
                .GetBy(i => i.emailId == emailId)
                .Select(i => new EmailViewModel
                {
                    emailId = i.emailId,
                    body = i.body,
                    subject = i.subject
                })
                .FirstOrDefault();

        }
        
        public static string GetSignUpEmailText(EmailViewModel email, Member member, int keyCode)
        {
            
            //logo companyName keyCode
			email.body = email.body
                //.Replace("<<logo>>", "https://authentication.ciclops.software/Content/Images/logo_icon.png")
                //.Replace("<<companyName>>", "Ciclops")
                .Replace("<<email>>", member.email)
                .Replace("<<keyCode>>", keyCode.ToString());
            
            return email.body;

        }
        
        public static string GetForgotPasswordEmailText(EmailViewModel email, Member member)
        {
            
			email.body = email.body
                .Replace("<<path>>", "https://authentication.ciclops.software/ResetPassword")
                //.Replace("<<companyName>>", "Ciclops")
                .Replace("<<forgotPasswordToken>>", member.forgotPasswordToken.ToString())
                .Replace("<<email>>", member.email);
            
            return email.body;

        }
        
    }
}
