using BarryJBriggs.Models;
using BarryJBriggs.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BarryJBriggs.Controllers
{
    public class MessageController : Controller
    {
        //Add regions to help organize your file, as it grows it makes it easier
        //to find things if they go these designated sections
        #region Fields

        private readonly EmailService _emailService;

        #endregion Fields

        #region Constructors

        public MessageController(EmailService emailService)
        {
            _emailService = emailService;
        }

        #endregion Constructors

        #region Private Methods

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            return emailRegex.IsMatch(email);
        }

        private string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            var sanitized = input.Replace("<", "&lt;")
                                 .Replace(">", "&gt;")
                                 .Replace("\"", "&quot;")
                                 .Replace("'", "&#39;");
            return sanitized;
        }

        #endregion Private Methods

        #region Public Methods

        //Making email service call asynchronous so now this method will be asynchronous, performance improvement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(Message model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Contact.cshtml", model);
            }

            //Brackets by itself will be very confusing to read when another developer comes in to read code.
            //I would just add brackets only for try/catch, if/else if/else, foreach, functions, etc.
            model.Name = SanitizeInput(model.Name);
            model.Email = SanitizeInput(model.Email);
            model.Website = SanitizeInput(model.Website);
            model.MessageText = SanitizeInput(model.MessageText);

            //Moved this outside of the try/catch since it won't throw anything
            if (!IsValidEmail(model.Email))
            {
                ModelState.AddModelError("Email", "Invalid email address.");

                return View("~/Views/Home/Contact.cshtml", model);
            }

            try
            {
                await _emailService.SendEmail(model.Email, model.Name, model.Website, model.MessageText);

                TempData["Message"] = "Your message has been sent!";

                return RedirectToAction("Contact", "Home");
            }
            catch (Exception ex)
            {
                //An empty model error doesn't appear to do anything. Maybe add a TempData["Error"] = "We are unable to send your message at this time, please try again." ?
                ModelState.AddModelError("", $"Error sending email: {ex.Message}");
            }

            return View("~/Views/Home/Contact.cshtml", model);
        }

        #endregion Public Methods
    }
}