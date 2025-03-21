using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalDomain.Model
{
    public partial class ReceiptRecord
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public int ReceiptId { get; set; }

        public ReceiptModel ReceiptNavigation { get; set; }
    }
}
