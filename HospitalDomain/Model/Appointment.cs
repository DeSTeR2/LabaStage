using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalDomain.Model;

public partial class Appointment
{
    [Required(ErrorMessage = "Поле є обов'язковим")]
    [Display(Name = "Ідентифікатор")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Дата є обов'язковою")]
    [Display(Name = "Дата прийому")]
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Час є обов'язковим")]
    [Display(Name = "Час прийому")]
    public TimeOnly Time { get; set; }

    [Display(Name = "Причина прийому")]
    public string? Reason { get; set; }

    [Required(ErrorMessage = "Оберіть лікаря")]
    [Display(Name = "Лікар")]
    public int Doctor { get; set; }

    [Required(ErrorMessage = "Оберіть пацієнта")]
    [Display(Name = "Пацієнт")]
    public int Patient { get; set; }

    [Display(Name = "Дані лікаря")]
    public virtual Doctor DoctorNavigation { get; set; } = null!;

    [Display(Name = "Дані пацієнта")]
    public virtual Patient PatientNavigation { get; set; } = null!;

    [Display(Name = "Кімнати для прийому")]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
