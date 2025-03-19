using System.ComponentModel.DataAnnotations;

namespace HospitalDomain.Model
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full name")]
        [DataType(DataType.Text)]
        public string FullName { get; set; } = null!;

        [Required]
        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [Required]
        [Display(Name = "Passworld")]
        [DataType(DataType.Password)]
        public string Passworld { get; set; } = null!; 
        
        [Required]
        [Display(Name = "Confirm Passworld")]
        [DataType(DataType.Password)]
        public string ConfirmationPassworld { get; set; } = null!; 
    }
}