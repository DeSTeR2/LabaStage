using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace HospitalMVC
{
    public class AccountViewModel
    {
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public string? UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
