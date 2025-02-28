using System;
using System.Collections.Generic;

namespace HospitalDomain.Model;

public partial class Patient
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Contacts { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
