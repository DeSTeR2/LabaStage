using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalDomain.Model;

public partial class Doctor
{
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Doctor ID")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Doctor's name is required")]
    [Display(Name = "Doctor Name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Specialty is required")]
    [Display(Name = "Specialty")]
    public string Speciality { get; set; } = null!;

    [Required(ErrorMessage = "Contact information is required")]
    [Display(Name = "Contact Information")]
    public string Contact { get; set; } = null!;

    [Required(ErrorMessage = "Please select a department")]
    [Display(Name = "Department")]
    public int Department { get; set; }

    [Display(Name = "Appointments")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [Display(Name = "Doctor's Department")]
    public virtual Department DepartmentNavigation { get; set; } = null!;
}
