using System.ComponentModel.DataAnnotations;

namespace Hospital.Models
{
    public class DoctorTelemedicine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string DoctorName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Specialist { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoomNo { get; set; }

        public bool Forenoon { get; set; }

        public bool Afternoon { get; set; }
    }
}