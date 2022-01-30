using System;
using System.ComponentModel.DataAnnotations;

namespace InventorySystemCore
{
    public class DamageReport
    {
        [Key]
        public int DamageReportId { get; private set; }
        public string Reported { get; private set; }
        public string Description { get; set; }
        public virtual Inventory Inventory { get; private set; }
        public virtual int InventoryId { get; private set; }
        DamageReport() { }
        public DamageReport(Inventory inventory, string report)
        {
            Reported = DateTime.Now.ToString();
            Description = report;
            Inventory = inventory;
            Inventory.Status = status.Bojāts;
        }
    }
}
