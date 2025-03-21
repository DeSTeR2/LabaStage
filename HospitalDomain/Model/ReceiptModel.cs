using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalDomain.Model
{
    public partial class ReceiptModel
    {
        public int Id { get; set; }
        public ICollection<ReceiptRecord>? ReceiptRecords { get; set; }
    }
}
