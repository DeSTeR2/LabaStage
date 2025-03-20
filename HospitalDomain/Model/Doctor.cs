using HospitalMVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalDomain.Model;

public partial class Doctor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Speciality { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? Department { get; set; }

    public string? ProfilePictureUrl { get; set; }
    public string? SignaturePictireUrl { get; set; }


    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [Display(Name = "Department")]
    public virtual Department DepartmentNavigation { get; set; } = null!;

    // Parameterless constructor required by EF Core
    public Doctor()
    {
    }

    // Custom constructor for creating Doctor from User
    public Doctor(User user, int id)
    {
        Id = id;
        Name = user.FullName;
        Contact = user.PhoneNumber;
        Email = user.Email;
        ProfilePictureUrl = user.ProfilePictureUrl;
    }

    // Custom constructor for another use case
    public Doctor(string doctorName, int appCount)
    {
        Name = doctorName;
    }
}