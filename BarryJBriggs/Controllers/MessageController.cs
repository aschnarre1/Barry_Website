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



        public MessageController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(Message model, string gRecaptchaResponse)
        {
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
                ModelState.AddModelError("", $"Error sending email: {ex.Message}");
            }

            return View("~/Views/Home/Contact.cshtml", model);
        }

        private async Task<bool> VerifyRecaptcha(string recaptchaResponse)
        {
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
