using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalDomain.Model;

public partial class Appointment
{
    [Required(ErrorMessage = "Field is required")]
    [Display(Name = "Appointment ID")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Date is required")]
    [Display(Name = "Appointment Date")]
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Time is required")]
    [Display(Name = "Appointment Time")]
    public TimeOnly Time { get; set; }

    [Display(Name = "Reason for Appointment")]
    public string? Reason { get; set; }

    [Required(ErrorMessage = "Please select a doctor")]
    [Display(Name = "Doctor")]
    public int Doctor { get; set; }

    [Required(ErrorMessage = "Please select a patient")]
    [Display(Name = "Patient")]
    public int Patient { get; set; }

    [Required(ErrorMessage = "Please select a room")]
    [Display(Name = "Room")]
    public int Room { get; set; }

    [Display(Name = "Doctor Information")]
    public virtual Doctor DoctorNavigation { get; set; } = null!;

    [Display(Name = "Patient Information")]
    public virtual Patient PatientNavigation { get; set; } = null!;
    [Display(Name = "Room Information")]
    public virtual Patient RoomNavigation { get; set; } = null!;

    [Display(Name = "Appointment Rooms")]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
