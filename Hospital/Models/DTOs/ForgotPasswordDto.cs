using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.DTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Employee Number is required")]
        public string EmployeeNumber { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}