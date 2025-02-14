using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalDomain.Model;

public partial class Doctor
{
    [Required(ErrorMessage = "Поле є обов'язковим")]
    [Display(Name = "Ідентифікатор")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Ім'я лікаря є обов'язковим")]
    [Display(Name = "Ім'я лікаря")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Спеціальність є обов'язковою")]
    [Display(Name = "Спеціальність")]
    public string Speciality { get; set; } = null!;

    [Required(ErrorMessage = "Контактна інформація є обов'язковою")]
    [Display(Name = "Контактна інформація")]
    public string Contact { get; set; } = null!;

    [Required(ErrorMessage = "Оберіть департамент")]
    [Display(Name = "Департамент")]
    public int Department { get; set; }

    [Display(Name = "Записи прийомів")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [Display(Name = "Департамент лікаря")]
    public virtual Department DepartmentNavigation { get; set; } = null!;
}
