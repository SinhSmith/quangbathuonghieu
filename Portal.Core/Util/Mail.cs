using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Portal.Core.Util
{
    public class Mail
    {
        public static void Send(string _mail, string title, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("team@zody.vn");
                mail.To.Add(_mail);
                mail.Subject = title;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("team@zody.vn", "Htphuc01t1");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception)
            {
            }
        }
    }
}