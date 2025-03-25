using Azure.Core;

namespace Utils
{
    public static class Constants
    {
        public static string Admin = "admin";
        public static string Manager = "manager";
        public static string Doctor = "doctor";
        public static string User = "user";

        public static string DefaultProfileImage = "/css/images/defaultPicture.png";
        public static string RootProfileImagesPath = "/css/images/profiles/";

        public static int StartWork = 9;
        public static int EndWork = 17;

        public static string CreatedAppointment = "Appointment was created";
        public static string ApprovedAppointment = "Appointment was approved";
        public static string CanceledAppointment = "Appointment was canceled";
        public static string AttendentedAppointment = "Appointment was attendented";
        public static string CompletedAppointment = "Appointment was completed";
        public static string SetRoom = "Set room {new}{old}";
        public static string ChangeTime = "Changed time from {old} to {new}";
        public static string ChangedReason = "Changed reason from {old} to {new}";
        public static string ChangeDate = "Changed date from {old} to {new}";
        public static string ChangeDoctor = "Changed doctor from {old} to {new}";
        public static string ChangedRoom = "Changed room from {old} to {new}";
        public static string ChangedPatient = "Changed patient from {old} to {new}";

        public static int RefreshAppointmentStateInMinutes = 30;

        public static string HospitalName = "Hospital";
        public static string HospitalAddress = "123 Hospital St, City";
        public static string HospitalStampImagePath = "/css/images/hospitalStamp.png";
    }
}