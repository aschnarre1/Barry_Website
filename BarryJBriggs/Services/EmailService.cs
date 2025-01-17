using System;
using System.Net.Mail;
using System.Net;

namespace BarryJBriggs.Services
{
    public class EmailService
    {
        public void SendEmail(string fromEmail, string name, string website, string message)
        {
            string smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER");
            string port = Environment.GetEnvironmentVariable("SMTP_PORT");
            string username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
            string password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");

            try
            {
                if (string.IsNullOrEmpty(smtpServer) ||
                    string.IsNullOrEmpty(port) ||
                    string.IsNullOrEmpty(username) ||
                    string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(adminEmail))
                {
                    throw new InvalidOperationException("Missing required environment variables for email configuration.");
                }

                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = int.Parse(port),
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = $"New Message from {name}",
                    Body = $"\nEmail:\n{fromEmail}\nWebsite:\n{website}\nMessage:\n{message}",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(adminEmail);
                smtpClient.Send(mailMessage);
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                throw;
            }
        }
    }
}
