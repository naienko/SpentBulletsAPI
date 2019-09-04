using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SpentBulletsAPI.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string name { get; set; }
        [Required]
        public int TypeId { get; set; }
        [Required]
        public int UserId { get; set; }
        public User user { get; set; }
        [Required]
        public string about { get; set; }
    }
}
