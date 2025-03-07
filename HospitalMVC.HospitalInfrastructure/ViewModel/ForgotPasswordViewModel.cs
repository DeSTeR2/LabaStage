﻿using System.ComponentModel.DataAnnotations;

namespace HospitalDomain.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
