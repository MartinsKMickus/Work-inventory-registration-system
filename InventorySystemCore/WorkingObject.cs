using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace InventorySystemCore
{
    public class WorkingObject
    {
        [Key]
        public int WorkingObjectId { get; private set; }
        public string Name { get; set; }
        public bool Deleted { get; set; } = false;
        public virtual ICollection<InventoryUsage>
            InventoryUsages
        { get; private set; } =
            new ObservableCollection<InventoryUsage>();
        public override string ToString()
        {
            return Name;
        }
    }
}
