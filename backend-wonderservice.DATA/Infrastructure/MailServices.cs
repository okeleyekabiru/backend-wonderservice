using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using backend_wonderservice.DATA.Abstration;
using backend_wonderservice.DATA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace backend_wonderservice.DATA.Infrastructure
{
    public class EmailService : IMailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private readonly IHttpContextAccessor _accessor;

        public EmailService(IConfiguration config, ILogger<EmailService> logger, IHttpContextAccessor accessor)
        {
            _config = config;
            _logger = logger;
            _accessor = accessor;
        }
        public void SendMail(string email, string message, string subject)
        {
            if (string.IsNullOrEmpty(email))
            {
                email = _config.GetSection("EmailDeveloper").Value;
            }
            try
            {
                // Credentials
                var credentials = new NetworkCredential(_config.GetSection("EmailAddress").Value,
                    _config.GetSection("EmailPassword").Value);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress("<noreply@wonderservice.com>"),
                    Subject = subject,
                    Body = message
                };

                mail.IsBodyHtml = true;

                mail.To.Add(new MailAddress(email));

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials
                };

                client.Send(mail);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Something went wrong, unable to send email to {email} on {DateTime.Now} {e.InnerException?.ToString() ?? e.Message}");
            }
        }




        public string ErrorMessage(string message)
        {
            var builder = new StringBuilder();
            builder.Append(
                "<!DOCTYPE html>" +
                "<html lang='en'>" +
                "<head>  " +
                " <meta charset='UTF-8'> " +
                "   <meta name='viewport' content='width=device-width', initial-scale=1.0>" +
                "   <title>Document</title>" +
                "</head>" +
                "<body>" +
                " <h1 style='color: red;text-decoration: underline;'>Error Alert</h1> " +
                $"  <p>{message}</p>" +
                "</body>" +
                "</html>");
            return builder.ToString();
        }

        public void VerifyEmail(string email, string message)
        {
            string host = _accessor.HttpContext.Request.Host.Value;
            SendMail(email, $"Please confirm your account by clicking <a href='{"http://" + host + "/api/user/confirmation?Token=" + message}'>here</a>", "Confirm your account");
        }

        public string OrderDetails(Customer customer, string serviceType)
        {
            var builder = new StringBuilder();
            builder.Append($"<!DOCTYPE html>" + "" +
                           "<html lang='en'>" + ""
                           + "<head>" + "  <meta charset='UTF - 8'>" + ""
                           + " <meta name='viewport' content='width = device - width, initial - scale = 1.0'>"
                           + " <title>wonder service</title>" +
                           "</head>" +
                           " < body > "
                           + " <h1 style='color: blue; text - decoration: underline; display: flex; justify - content: center; '>Booking</h1>" +
                           "" + "  <div style='text - align: center; '>"
                           + "" +
                           "< div style = 'text-align: center;' >" +
                           $" <input type='text' disabled value={customer.FirstName}   style='margin - bottom: 2em; border - radius: 4px; background - color:rgba(192, 192, 192, 0.5); color: black; height: 2.5em; width: 25em; '  ><br>"
                           + $"       <input type='text' disabled value={customer.LastName} style='margin - bottom: 2em; border - radius: 4px; background - color:rgba(192, 192, 192, 0.5); color: black; height: 2.5em; width: 25em; '><br>"
                           + $"       <input type='text' disabled value={customer.Email} style='margin - bottom: 2em; border - radius: 4px; background - color:rgba(192, 192, 192, 0.5); color: black; height: 2.5em; width: 25em; '><br>"
                           + $"       <input type='text' disabled value={customer.PhoneNumber} style='margin - bottom: 2em; border - radius: 4px; background - color:rgba(192, 192, 192, 0.5); color: black; height: 2.5em; width: 25em; '><br>"
                           + $"       <input type='text' disabled value={customer.Address} style='margin - bottom: 2em; border - radius: 4px; background - color:rgba(192, 192, 192, 0.5); color: black; height: 2.5em; width: 25em; '><br>"
                           + $"       <input type='text' disabled value={serviceType} style='margin - bottom: 2em; border - radius: 4px; background - color:rgba(192, 192, 192, 0.5); color: black; height: 2.5em; width: 25em; '><br>");
            return builder.ToString();
        }
    }
}
