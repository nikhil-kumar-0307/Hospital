using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class DoctorInShift
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [Required]
        [MaxLength(20)]
        public string Shift { get; set; }

        [Required]
        public DateTime ShiftDate { get; set; } = DateTime.Today;
    }
}