using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hospital.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string EmployeeNumber { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        [MaxLength(20)]
        public string Role { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}