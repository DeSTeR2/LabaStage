using HospitalDomain.Model;

namespace HospitalMVC.HospitalInfrastructure.Services.PdfService
{
    public class DataContainer
    {
        public string[] names;
        public string[] descriptions;
        public Doctor doctor;
        public Patient patient;
        public DateTime createTime;
    }
}
