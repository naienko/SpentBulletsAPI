using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SpentBulletsAPI.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string brand { get; set; }
    }
}
