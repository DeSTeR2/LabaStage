﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace HospitalDomain.Model;

public partial class Appointment
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public string? Reason { get; set; }

    public int Doctor { get; set; }

    public int Patient { get; set; }

    public int? Room { get; set; }

    public int? ReceiptId { get; set; }

    public int AppointmentState { get; set; } = 1;

    [DisplayName("Doctor name")]
    public virtual Doctor DoctorNavigation { get; set; } = null!;

    [DisplayName("Patient name")]
    public virtual Patient PatientNavigation { get; set; } = null!;

    [DisplayName("Receipt")]
    public virtual ReceiptModel? ReceiptNavigation { get; set; } = null!;

    [DisplayName("Room type")]
    [AllowNull]
    public virtual Room RoomNavigation { get; set; }

    public override string ToString()
    {
        string result = "";
        result = $"Appointment on date {Date} at {Time}. Reason - {Reason}\n";
        return result;
    }
}
