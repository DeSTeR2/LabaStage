﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

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

    [DisplayName("Doctor name")]
    public virtual Doctor DoctorNavigation { get; set; } = null!;

    [DisplayName("Patient name")]
    public virtual Patient PatientNavigation { get; set; } = null!;

    [DisplayName("Room type")]
    public virtual Room RoomNavigation { get; set; } = null!;
}
