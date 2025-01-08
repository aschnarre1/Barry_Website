using BarryJBriggs.Models;
using BarryJBriggs.Services;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult SendMessage(Message model)
        {
            if (ModelState.IsValid)
            {
                try
                {
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

    }
}
