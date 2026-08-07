using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Hospital.Models.DTOs
{
    public class DoctorOnLeaveDto
    {
        [Required(ErrorMessage = "Please select a doctor")]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Required]
        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; } = DateTime.Today;

        // Populated in the controller, not bound from the form
        public SelectList DoctorList { get; set; }
    }
}