using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalDomain.Model;

public partial class Room
{
    [Required(ErrorMessage = "Поле є обов'язковим")]
    [Display(Name = "Ідентифікатор")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Тип кімнати є обов'язковим")]
    [Display(Name = "Тип кімнати")]
    public string Type { get; set; } = null!;

    [Required(ErrorMessage = "Місткість кімнати є обов'язковою")]
    [Display(Name = "Місткість")]
    [Range(1, int.MaxValue, ErrorMessage = "Місткість повинна бути більше 0")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Поле наявності є обов'язковим")]
    [Display(Name = "Наявність")]
    public byte Availability { get; set; }

    [Required(ErrorMessage = "Оберіть запис прийому")]
    [Display(Name = "Запис прийому")]
    public int Appointment { get; set; }

    [Display(Name = "Деталі запису")]
    public virtual Appointment AppointmentNavigation { get; set; } = null!;
}
