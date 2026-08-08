using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Hospital.Models.DTOs
{
    public class DoctorInShiftDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a doctor")]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Please select a shift")]
        [Display(Name = "Shift")]
        public string Shift { get; set; }

        // Populated in the controller, not bound from the form
        public SelectList DoctorList { get; set; }

        [Required]
        [Display(Name = "Shift Date")]
        [DataType(DataType.Date)]
        public System.DateTime ShiftDate { get; set; } = System.DateTime.Today;
    }
}