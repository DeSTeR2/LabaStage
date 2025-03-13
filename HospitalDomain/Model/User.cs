using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HospitalMVC
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? Address { get; set; }

        public string? ProfilePictureUrl { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public void UpdateUser(AccountViewModel model)
        {
            FullName = model.FullName;
            DateOfBirth = model.DateOfBirth;
            PhoneNumber = model.PhoneNumber;
            Address = model.Address;
            ProfilePictureUrl = model.ProfilePictureUrl;
            UpdatedAt = model.UpdatedAt;
        }
    }
}
