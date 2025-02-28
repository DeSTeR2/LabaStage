using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalDomain.Model;

public partial class Patient
{
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Patient ID")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Patient's name is required")]
    [Display(Name = "Patient Name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Date of birth is required")]
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Contact information is required")]
    [Display(Name = "Contact Information")]
    public string Contacts { get; set; } = null!;

    [Display(Name = "Appointments")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
