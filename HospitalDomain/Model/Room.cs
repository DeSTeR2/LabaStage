using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalDomain.Model;

public partial class Room
{
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Room ID")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Room type is required")]
    [Display(Name = "Room Type")]
    public string Type { get; set; } = null!;

    [Required(ErrorMessage = "Room capacity is required")]
    [Display(Name = "Capacity")]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
    public int Capacity { get; set; }
}
