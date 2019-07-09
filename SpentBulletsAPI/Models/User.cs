using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SpentBulletsAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        public string display_name { get; set; }

        public string role { get; set; }
    }
}
