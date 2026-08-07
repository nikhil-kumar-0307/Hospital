using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class ScheduledDoctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; } = DateTime.Today;

        [Required]
        [MaxLength(20)]
        public string RoomNo { get; set; }

        public bool Forenoon { get; set; }

        public bool Afternoon { get; set; }
    }
}