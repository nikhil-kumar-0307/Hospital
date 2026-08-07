using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.DTOs
{
    public class AddDoctorDto
    {
        [Required(ErrorMessage = "Doctor's name is required")]
        [MaxLength(100)]
        [Display(Name = "Doctor's Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Specialist is required")]
        [MaxLength(50)]
        [Display(Name = "Specialist")]
        public string Specialist { get; set; }
    }
}