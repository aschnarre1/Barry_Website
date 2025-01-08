using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BarryJBriggs.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters")]
        public string Name { get; set; }



        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, ErrorMessage = "Email must be less than 100 characters")]
        public string Email { get; set; }



        [StringLength(200, ErrorMessage = "Website must be less than 200 characters")]
        public string Website { get; set; }


        [Required(ErrorMessage = "Message is required")]
        [StringLength(500, ErrorMessage = "Message must be less than 500 characters")]
        public string MessageText { get; set; }


    }
}
