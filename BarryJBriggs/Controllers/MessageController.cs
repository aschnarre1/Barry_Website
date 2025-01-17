using BarryJBriggs.Models;
using BarryJBriggs.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BarryJBriggs.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailService _emailService;

        public MessageController(EmailService emailService)
        {
            _emailService = emailService;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMessage(Message model)
        {

            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Contact.cshtml", model);
            }

            {

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
            }
            return View("~/Views/Home/Contact.cshtml", model);
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

            var sanitized = input.Replace("<", "&lt;")
                                 .Replace(">", "&gt;")
                                 .Replace("\"", "&quot;")
                                 .Replace("'", "&#39;");
            return sanitized;
        }



    }
}




