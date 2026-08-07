using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Hospital.Models.DTOs
{
    public class ScheduledDoctorDto
    {
        [Required(ErrorMessage = "Please select a doctor")]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Required]
        [Display(Name = "Scheduled Date")]
        [DataType(DataType.Date)]
        public DateTime ScheduledDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Room No is required")]
        [MaxLength(20)]
        [Display(Name = "Room No")]
        public string RoomNo { get; set; }

        [Display(Name = "Forenoon")]
        public bool Forenoon { get; set; }

        [Display(Name = "Afternoon")]
        public bool Afternoon { get; set; }

        // Populated in the controller, not bound from the form
        public SelectList DoctorList { get; set; }
    }
}