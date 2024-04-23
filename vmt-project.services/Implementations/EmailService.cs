using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using vmt_project.models.Request.Email;
using vmt_project.services.Contracts;
using vmt_project.dal.Models.Entities;

namespace vmt_project.services.Implementations
{
    public class EmailService : IEmailService
    {
        public async Task SendEmail(SendEmailRequest request)
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("EmailUsername"), Environment.GetEnvironmentVariable("EmailPassword"));

                // Create the email message
                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(Environment.GetEnvironmentVariable("From"));
                    mailMessage.To.Add(request.To);
                    mailMessage.Subject = request.Subject;
                    mailMessage.Body = request.Body;

                    // Send the email
                    try
                    {
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send email: {ex.Message}");
                    }
                }
            }
        }
        public async Task SendEmailForgetPassword(User user, string token)
        {
            var request = new SendEmailRequest
            {
                To = user.Email,
                Subject = "Forget password",
                Body = token
            };
            await SendEmail(request);
        }
    }
}
