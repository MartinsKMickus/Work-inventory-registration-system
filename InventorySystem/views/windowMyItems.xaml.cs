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

namespace InventorySystem.views
{
    /// <summary>
    /// Interaction logic for windowMyItems.xaml
    /// </summary>
    public partial class windowMyItems : Window
    {
        public windowMyItems(InventorySystemContext context, Employee employee)
        {
            InitializeComponent();
            context.WorkingObjects.Load();
            context.Inventories.Load();
            var UsageSelection = from usage in employee.InventoryUsages
                                 where usage.Returned == null
                                 select new
                                 {
                                     usage.Inventory.InventoryId,
                                     usage.Inventory.Name,
                                     usage.Inventory.Manufacturer,
                                     usage.Inventory.Model,
                                     usage.InventoryUsageId,
                                     usage.WorkingObject,
                                     usage.Taken,
                                     usage.Returned
                                 };

            DataMain.ItemsSource = UsageSelection.ToList();
            DataMain.Items.Refresh();
        }
    }
}
