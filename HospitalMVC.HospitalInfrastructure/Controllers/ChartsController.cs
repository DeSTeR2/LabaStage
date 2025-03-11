using HospitalDomain.Model;
using HospitalMVC.HospitalInfrastructure.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaMVC.WebMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChartsController : ControllerBase
{
    private record CountByDoctorNumber(string DepartmentName, int Count);

    private readonly HospitalContext _hospitalContext;

    public ChartsController(HospitalContext hospitalContext)
    {
        _hospitalContext = hospitalContext;
    }

    [HttpGet("countDepartment")]
    public async Task<IActionResult> GetCountByDepartmentAsync(CancellationToken cancellationToken)
    {
        var responseItems = await _hospitalContext
            .Departments
            .Select(dep => new
            {
                DepartmentName = dep.Name,
                DoctorCount = dep.Doctors.Count
            })
            .ToListAsync(cancellationToken);

        var result = responseItems
            .Select(item => new CountByDoctorNumber(item.DepartmentName, item.DoctorCount))
            .ToList();

        return Ok(result); 
    }

    [HttpGet("appointmentsByDoctor")]
    public async Task<JsonResult> GetAppointmentsByDoctorAsync(CancellationToken cancellationToken)
    {
        var doctorAppointmentCounts = await _hospitalContext
            .Doctors
            .Select(doctor => new
            {
                DoctorName = doctor.Name,
                AppointmentCount = doctor.Appointments.Count
            })
            .OrderByDescending(d => d.AppointmentCount) // Optional: Sort by appointment count
            .ToListAsync(cancellationToken);

        // Return the data as JSON
        return new JsonResult(doctorAppointmentCounts);
    }
}
