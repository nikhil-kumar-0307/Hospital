using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.DTOs
{
    public class VisitingDoctorDto
    {
        [Required(ErrorMessage = "Doctor's name is required")]
        [MaxLength(150)]
        [Display(Name = "Doctor's Name")]
        public string DoctorName { get; set; }

        [Required(ErrorMessage = "Specialist is required")]
        [MaxLength(100)]
        [Display(Name = "Specialist")]
        public string Specialist { get; set; }

        [Required(ErrorMessage = "Room No is required")]
        [MaxLength(20)]
        [Display(Name = "Room No")]
        public string RoomNo { get; set; }

        [Display(Name = "Forenoon")]
        public bool Forenoon { get; set; }

        [Display(Name = "Afternoon")]
        public bool Afternoon { get; set; }

        [Required]
        [Display(Name = "Visit Date")]
        [DataType(DataType.Date)]
        public System.DateTime VisitDate { get; set; } = System.DateTime.Today;
    }
}