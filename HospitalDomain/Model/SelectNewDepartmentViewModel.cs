using HospitalDomain.Model;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

public class SelectNewDepartmentViewModel
{
    public List<DoctorSelectionViewModel> DoctorSelections { get; set; }
    public List<Department> Departments { get; set; }

    [Required(ErrorMessage = "Please select a new department.")]
    public int NewDepartmentId { get; set; }

    public static List<DoctorSelectionViewModel> Convert(List<Doctor> doctors)
    {
        List<DoctorSelectionViewModel> list = new();
        foreach (var doc in doctors)
        {
            list.Add(new DoctorSelectionViewModel
            {
                Id = doc.Id,
                Name = doc.Name,
                IsSelected = true
            });
        }

        return list;
    }
}

public class DoctorSelectionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsSelected { get; set; } // Property to mark if the doctor is selected for department update
}

