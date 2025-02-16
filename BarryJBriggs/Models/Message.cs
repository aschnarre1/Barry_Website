//using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BarryJBriggs.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
        public required string Name { get; set; }



        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, ErrorMessage = "Email must be less than 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public required string Email { get; set; }

        [StringLength(200, ErrorMessage = "Website must be less than 200 characters")]
        [Url(ErrorMessage = "Invalid website URL format")]
        [RegularExpression(@"^(https?:\/\/.*)?$", ErrorMessage = "Invalid website URL format")]

        public string? Website { get; set; }

        [Display(Name = "Message")]
        [Required(ErrorMessage = "Message is required")]
        [MinLength(3, ErrorMessage = "Name must be at least 10 characters long")]
        [StringLength(500, ErrorMessage = "Message must be less than 500 characters")]
        public required string MessageText { get; set; }
    }
}
