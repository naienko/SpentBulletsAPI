using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SpentBulletsAPI.Models
{
    public class Stack
    {
        [Key]
        public int Id { get; set; }

        public int amount { get; set; }

        [Required]
        public int UserId { get; set; }

        public User user { get; set; }

        [Required]
        public int BrandId { get; set; }

        public Brand brand { get; set; }

        [Required]
        public int CaliberId { get; set; }

        public Caliber caliber { get; set; }

        public int grain { get; set; }

        public string notes { get; set; }
    }
}
