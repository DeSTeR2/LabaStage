using System.ComponentModel.DataAnnotations.Schema;
using Utils;

namespace HospitalDomain.Model
{
    public partial class ReceiptModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int Id { get; set; }
        public ICollection<ReceiptRecord>? ReceiptRecords { get; set; }

        public ReceiptModel() { }

        public ReceiptModel(Appointment appointment, string[] names, string[] descriptions, HospitalContext hospitalContext)
        {
            ReceiptRecords = new List<ReceiptRecord>();
            var modelIds = hospitalContext.Receipts
                .Select(r => r.Id)
                .OrderBy(id => id)
                .ToList();

            this.Id = Util.GetId(modelIds);
            
            hospitalContext.Receipts.Add(this);
            hospitalContext.SaveChanges();

            appointment.ReceiptId = Id;
            appointment.ReceiptNavigation = this;

            var recordIds = hospitalContext.ReceiptRecords
                .Select(r => r.Id)
                .OrderBy(id => id)
                .ToList();

            for (int i=0; i<names.Length;i++)
            {
                ReceiptRecord receiptRecord = new ReceiptRecord()
                {
                    Id = Util.GetId(recordIds),
                    Name = names[i],
                    Description = descriptions[i],
                    ReceiptId = this.Id,
                    ReceiptNavigation = this
                };

                //ReceiptRecords.Add(receiptRecord);
                recordIds.Add(receiptRecord.Id);
                hospitalContext.ReceiptRecords.Add(receiptRecord);
            }
            hospitalContext.Appointments.Update(appointment);
            hospitalContext.SaveChanges();
        }
        public static void CreateReceipt(Appointment appointment, string[] names, string[] descriptions, HospitalContext hospitalContext)
        {
            if (appointment.ReceiptId == null || appointment.ReceiptId == 0)
            {
                ReceiptModel receiptModel = new ReceiptModel(appointment, names, descriptions, hospitalContext);
            } else {
                var recordIds = hospitalContext.ReceiptRecords
                    .Select(r => r.Id)
                    .OrderBy(id => id)
                    .ToList();

                ReceiptModel receipt = hospitalContext.Receipts.First(a => a.Id == appointment.ReceiptId);
                ReceiptRecord[] records = hospitalContext.ReceiptRecords.Where(a => a.ReceiptId == appointment.ReceiptId).ToArray();

                for (int i = 0; i < records.Length; i++)
                {
                    records[i].Name = names[i];
                    records[i].Description = descriptions[i];
                    hospitalContext.ReceiptRecords.Update(records[i]);
                }

                for (int i = records.Length; i < names.Length; i++)
                {
                    ReceiptRecord receiptRecord = new ReceiptRecord()
                    {
                        Id = Util.GetId(recordIds),
                        Name = names[i],
                        Description = descriptions[i],
                        ReceiptId = receipt.Id,
                        ReceiptNavigation = receipt
                    };

                    recordIds.Add(receiptRecord.Id);
                    hospitalContext.ReceiptRecords.Add(receiptRecord);
                }

                hospitalContext.Appointments.Update(appointment);
                hospitalContext.SaveChanges();
            }
        }
    }
}
