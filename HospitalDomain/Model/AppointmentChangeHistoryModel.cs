﻿using System.Security.Claims;
using Utils;

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

        public AppointmentChangeHistoryModel(Appointment appointment, HospitalContext hospitalContext ,string? changeInfo, ClaimsPrincipal User = null)
        {
            string changedBy = User != null ? User.Identity.Name + $" ({CheckRole.GetUserRole(User)})" : "";

            AppointmentId = appointment.Id;
            AppointmentNavigation = appointment;
            ChangeTime = DateTime.Now;
            ChangeInfo = changeInfo;
            ChangedBy = changedBy;

            hospitalContext.AppointmentChanges.Add(this);
            hospitalContext.SaveChanges();
        }
    }
}
