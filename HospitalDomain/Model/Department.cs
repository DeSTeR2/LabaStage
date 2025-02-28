using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalDomain.Model;

public partial class Department
{
    [Display(Name = "Department ID")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Department name is required")]
    [Display(Name = "Department Name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Location is required")]
    [Display(Name = "Location")]
    public string Location { get; set; } = null!;

    [Display(Name = "Doctors in Department")]
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
