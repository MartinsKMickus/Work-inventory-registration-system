using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace InventorySystemCore
{
    public enum status
    {
        Aktīvs,
        Bojāts,
        Remontā,
        Norakstīts
    }
    public class Inventory
    {
        [Key]
        public int InventoryId { get; private set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public status Status { get; set; }
        public Inventory()
        {
            Status = status.Aktīvs;
        }
        public virtual DamageReport DamageReport { get; set; }
        public virtual ICollection<InventoryUsage>
            InventoryUsages
        { get; private set; } =
            new ObservableCollection<InventoryUsage>();
        public void ReportDamage(string report)
        {
            DamageReport = new DamageReport(this, report);
            Status = status.Bojāts;
        }
        public override string ToString()
        {
            return "Inventāra ID: " + InventoryId + "\n" + Name + " " + Manufacturer + " " + Model;
        }
    }
}
