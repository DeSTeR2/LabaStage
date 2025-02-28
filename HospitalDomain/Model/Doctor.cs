using System;
using System.Collections.Generic;

namespace HospitalDomain.Model;

public partial class Doctor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Speciality { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public int Department { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Department DepartmentNavigation { get; set; } = null!;
}
