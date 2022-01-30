using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace InventorySystemCore
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; private set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public bool Deleted { get; set; } = false;
        public bool PasswordChange { get; set; } = true;
        public virtual ICollection<InventoryUsage>
            InventoryUsages
        { get; set; } =
            new ObservableCollection<InventoryUsage>();
        public override string ToString()
        {
            return Name + " " + Surname;
        }
    }
}
