using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace HospitalDomain.Model;

public partial class Patient
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    [DisplayName("Date of birth")]
    public DateTime DateOfBirth { get; set; }

    public string Contacts { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
