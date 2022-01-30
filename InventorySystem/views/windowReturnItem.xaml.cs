using InventorySystemCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
    /// Interaction logic for windowReturnItem.xaml
    /// </summary>
    public partial class windowReturnItem : Window
    {
        InventorySystemContext Context;
        Inventory Inventory;
        Employee Employee;
        string Scanned;
        public windowReturnItem(InventorySystemContext context, Employee employee)
        {
            InitializeComponent();
            Context = context;
            Employee = employee;
            RefreshData();
            inputCode.Focus();
        }

        void CleanTextFields()
        {
            textInfoLarge.Text = "";
            textStatus.Text = "";
            inputCode.Text = "";
        }

        private void RefreshData()
        {
            Context.Inventories.Load();
            var UsageSelection = from usage in Employee.InventoryUsages
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
            //comboObjects.ItemsSource = Context.WorkingObjects.ToList();
        }

        private void Return()
        {
            Scanned = inputCode.Text;
            CleanTextFields();
            int InvID;
            Notifications notifications = new Notifications();
            try
            {
                InvID = int.Parse(Scanned);
            }
            catch (Exception)
            {
                inputCode.Text = "";
                MessageBox.Show("Nav ievadīts inventāra ID");
                return;
            }
            Inventory = Context.Inventories.Find(InvID);
            if (Inventory == null)
            {
                notifications.E20(InvID);
                return;
            }
            //InventoryUsage inventoryUsageTemp;
            foreach (InventoryUsage inventoryUsage in Inventory.InventoryUsages)
            {
                if (inventoryUsage.Returned == null) // Can be that each of if is true if this is true.
                {
                    if(inventoryUsage.Employee != Employee)
                    {
                        if(notifications.A6() == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                    //if (DateTime.Parse(inventoryUsage.Taken) < DateTime.Now)
                    //{
                    //foreach (InventoryUsage inventoryUsage1 in Employee.InventoryUsages) // Check if found inventory is really taken by employee.
                    //{
                    //if (inventoryUsage1.InventoryUsageId == inventoryUsage.InventoryUsageId)
                    //{
                    inventoryUsage.Return(); // Note returning time.
                    textInfoLarge.Text = Inventory.ToString() + "\n";
                    textInfoLarge.Text += "Paņemts: ";
                    textInfoLarge.Text += inventoryUsage.Taken + "\n";
                    textInfoLarge.Text += "Atgriezts: ";
                    textInfoLarge.Text += inventoryUsage.Returned;
                    Context.SaveChanges();
                    textStatus.Foreground = Brushes.GreenYellow;
                    textStatus.Text = "Atgriezts!";
                    //notifications.I16();
                    //return;
                    //}
                    //}
                    //}
                }
            }
            RefreshData();
            /// Inventory code not found!
            /// 
        }

        private void inputCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inputCode.Text.Length > 5)
            {
                if (inputCode.Text == "")
                {
                    CleanTextFields();
                    return;
                }
                Console.Beep(650, 240);
                Return();
            }
        }

        private void buttonEnd_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
