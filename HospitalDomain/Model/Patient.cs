using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalDomain.Model;

public partial class Patient
{
    [Required(ErrorMessage = "Поле є обов'язковим")]
    [Display(Name = "Ідентифікатор")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Ім'я пацієнта є обов'язковим")]
    [Display(Name = "Ім'я пацієнта")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Дата народження є обов'язковою")]
    [Display(Name = "Дата народження")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Контактна інформація є обов'язковою")]
    [Display(Name = "Контактна інформація")]
    public string Contacts { get; set; } = null!;

    [Display(Name = "Записи прийомів")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
