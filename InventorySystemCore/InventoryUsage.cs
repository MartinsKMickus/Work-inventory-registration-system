using System;
using System.ComponentModel.DataAnnotations;

namespace InventorySystemCore
{
    public class InventoryUsage
    {
        [Key]
        public int InventoryUsageId { get; private set; }
        public string Taken { get; private set; }
        public string Returned { get; private set; }
        public virtual Employee Employee { get; private set; }
        public virtual Inventory Inventory { get; private set; }
        public virtual WorkingObject WorkingObject { get; private set; }
        public InventoryUsage()
        {
            Taken = DateTime.Now.ToString();
        }
        public void Return()
        {
            Returned = DateTime.Now.ToString();
        }
        public override string ToString()
        {
            string toReturn = $"Lietošanas ID: {InventoryUsageId}\nPaņemts: {Taken}\nAtgriezts: {Returned}\nPaņēma: {Employee}\nUz objektu: {WorkingObject}";
            return toReturn;
        }
    }
}
