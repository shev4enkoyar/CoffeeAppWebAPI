using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace CoffeeAppWebApi.Services
{
    public class EmailService
    {
        private IConfiguration Configuration { get; }

        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public bool SendEmail(string addressee, string subject, string bodyHtml)
        {
            using SmtpClient client = new("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential credentials = new(Configuration["MailSettings:DefaultMail"], Configuration["MailSettings:AppKey"]);
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;
            using MailMessage mail = new();
            mail.From = new MailAddress(Configuration["MailSettings:DefaultMail"]);
            mail.To.Add(addressee);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = string.Format(bodyHtml);
            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
