using InventorySystemCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ShowInventory.xaml
    /// </summary>
    public partial class ShowInventory : Window
    {
        public ShowInventory(InventorySystemContext context, Inventory inventory)
        {
            InitializeComponent();
            context.Employees.Load();
            context.WorkingObjects.Load();
            listInventory.ItemsSource = inventory.InventoryUsages;
        }
    }
}
