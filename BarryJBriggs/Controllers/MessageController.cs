using BarryJBriggs.Models;
using BarryJBriggs.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BarryJBriggs.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailService _emailService;
        private static readonly HttpClient client = new HttpClient();
        private static readonly string RECAPTCHA_SECRET = Environment.GetEnvironmentVariable("CAPTCHA_KEY");

        static MessageController()
        {
            if (string.IsNullOrEmpty(RECAPTCHA_SECRET))
            {
                Console.WriteLine("⚠️ WARNING: CAPTCHA_KEY environment variable is not set!");
            }
        }

        public MessageController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(Message model, string gRecaptchaResponse)
        {
            // ✅ reCAPTCHA Validation
            if (string.IsNullOrEmpty(gRecaptchaResponse) || !await VerifyRecaptcha(gRecaptchaResponse))
            {
                ModelState.AddModelError("", "reCAPTCHA verification failed. Please try again.");
                return View("~/Views/Home/Contact.cshtml", model);
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Contact.cshtml", model);
            }

            model.Name = SanitizeInput(model.Name);
            model.Email = SanitizeInput(model.Email);
            model.Website = SanitizeInput(model.Website);
            model.MessageText = SanitizeInput(model.MessageText);

            try
            {
                if (!IsValidEmail(model.Email))
                {
                    ModelState.AddModelError("Email", "Invalid email address.");
                    return View("~/Views/Home/Contact.cshtml", model);
                }

                _emailService.SendEmail(model.Email, model.Name, model.Website, model.MessageText);
                TempData["Message"] = "Your message has been sent!";
                return RedirectToAction("Contact", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending email: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while sending your message. Please try again later.");
            }

            return View("~/Views/Home/Contact.cshtml", model);
        }

        private async Task<bool> VerifyRecaptcha(string recaptchaResponse)
        {
            if (string.IsNullOrEmpty(RECAPTCHA_SECRET))
            {
                Console.WriteLine("⚠️ reCAPTCHA Secret Key is missing!");
                return false; // Prevent API call if secret key is missing
            }

            var response = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={RECAPTCHA_SECRET}&response={recaptchaResponse}",
                null
            );

            var jsonResponse = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(jsonResponse);
            return result.success == true;
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        private string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return input.Replace("<", "&lt;")
                        .Replace(">", "&gt;")
                        .Replace("\"", "&quot;")
                        .Replace("'", "&#39;");
        }
    }
}
