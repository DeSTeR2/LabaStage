using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HospitalDomain.Model
{
    public partial class AppointmentChangeHistoryModel
    {
        public int AppointmentId { get; set; }
        public DateTime ChangeTime { get; set; }
        public string? ChangeInfo { get; set; }
        public string? ChangedBy { get; set; }

        public Appointment AppointmentNavigation { get; set; } = null!;

        public AppointmentChangeHistoryModel() { }

        public AppointmentChangeHistoryModel(int appointmentId, string? changeInfo, string? changedBy, Appointment appointmentNavigation)
        {
            AppointmentId = appointmentId;
            ChangeTime = DateTime.Now;
            ChangeInfo = changeInfo;
            ChangedBy = changedBy;
            AppointmentNavigation = appointmentNavigation;
        }

        public AppointmentChangeHistoryModel(Appointment appointment, HospitalContext hospitalContext ,string? changeInfo, string? changedBy)
        {
            AppointmentId = appointment.Id;
            ChangeTime = DateTime.Now;
            ChangeInfo = changeInfo;
            ChangedBy = changedBy;

            hospitalContext.AppointmentChanges.Add(this);
            hospitalContext.SaveChanges();
        }
    }
}
