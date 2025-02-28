using System;
using System.Collections.Generic;

namespace HospitalDomain.Model;

public partial class Appointment
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public string? Reason { get; set; }

    public int Doctor { get; set; }

    public int Patient { get; set; }

    public int Room { get; set; }

    public virtual Doctor DoctorNavigation { get; set; } = null!;

    public virtual Patient PatientNavigation { get; set; } = null!;

    public virtual Room RoomNavigation { get; set; } = null!;
}
