using System.Net.Mail;
using System.Net;

namespace BarryJBriggs.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string fromEmail, string name, string website, string message)
        {
            try
            {
                var smtpClient = new SmtpClient(_config["EmailSettings:SmtpServer"])
                {
                    Port = int.Parse(_config["EmailSettings:Port"]),
                    Credentials = new NetworkCredential(
                        _config["EmailSettings:Username"],
                        _config["EmailSettings:Password"]),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = $"New Message from {name}",
                    Body = $"\nEmail:\n{fromEmail}\nWebsite:\n{website}\nMessage:\n{message}",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(_config["EmailSettings:AdminEmail"]);
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
