using InventorySystemCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InventorySystemAdmin
{
    /// <summary>
    /// Interaction logic for ShowWorkingObjectInventory.xaml
    /// </summary>
    public partial class ShowWorkingObjectInventory : Window
    {
        public ShowWorkingObjectInventory(InventorySystemContext context, WorkingObject workingObject)
        {
            InitializeComponent();
            context.Employees.Load();
            context.Inventories.Load();

            var UsageSelection = from usage in workingObject.InventoryUsages
                                 //where usage.Returned == null
                                 select new
                                 {
                                     usage.Inventory.InventoryId,
                                     usage.Inventory.Name,
                                     usage.Inventory.Manufacturer,
                                     usage.Inventory.Model,
                                     usage.InventoryUsageId,
                                     usage.Employee,
                                     usage.Taken,
                                     usage.Returned 
                                 };

            DataMain.ItemsSource = UsageSelection.ToList();
            DataMain.Items.Refresh();
        }
    }
}
