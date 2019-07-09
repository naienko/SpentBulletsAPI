using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SpentBulletsAPI.Models
{
    public class Caliber
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string caliber { get; set; }
    }
}
