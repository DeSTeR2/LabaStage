using System;
using System.Collections.Generic;

namespace HospitalDomain.Model;

public partial class Room
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public int Capacity { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public string AppointmentId;
}
