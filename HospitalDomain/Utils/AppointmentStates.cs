using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalDomain.Utils
{
    public static class AppointmentStates
    {
        public static string Pending = "Pending";
        public static string Approved = "Approved";
        public static string Completed = "Completed";
        public static string Attending = "Attending";
        public static string Canceled = "Canceled";

        public static List<Tuple<int, string>> States = new List<Tuple<int, string>>
        {
            new Tuple<int, string>(1, Pending),
            new Tuple<int, string>(2, Approved),
            new Tuple<int, string>(3, Attending),
            new Tuple<int, string>(4, Completed),
            new Tuple<int, string>(5, Canceled),
        };
    }
}
