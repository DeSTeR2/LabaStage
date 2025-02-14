using System.Collections.Generic;
using HospitalDomain.Model;

namespace HospitalMVC.ViewModels // Make sure this matches your project's default namespace
{
    public class HomeIndexViewModel
    {
        public List<Appointment>? Appointments { get; set; }
        public List<Department>? Departments { get; set; }
        public List<Doctor>? Doctors { get; set; }
        public List<Patient>? Patients { get; set; }
        public List<Room>? Rooms { get; set; }
    }
}
