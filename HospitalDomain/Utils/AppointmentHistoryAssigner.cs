using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalDomain.Utils
{
    public static class AppointmentHistoryAssigner
    {
        public static string GetTransformedString<T>(string originalString, T oldValue, T newValue)
        {
            string newString = "";
            newString += originalString;
            newString = newString.Replace("{old}", oldValue?.ToString());
            newString = newString.Replace("{new}", newValue?.ToString());
            return newString;
        }
    }
}
