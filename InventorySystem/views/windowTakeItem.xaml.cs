using InventorySystemCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
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
    /// Interaction logic for windowTakeItem.xaml
    /// </summary>
    public partial class windowTakeItem : Window
    {
        InventorySystemContext Context;
        Inventory Inventory;
        Employee Employee;
        string Scanned;
        public windowTakeItem(InventorySystemContext context, Employee employee)
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

        void RefreshData()
        {
            Context.WorkingObjects.Load();
            Context.Inventories.Load();
            var ObjectSelection = from workingobject in Context.WorkingObjects
                                  where workingobject.Deleted != true
                                  select workingobject;
            MainData.ItemsSource = ObjectSelection.ToList();//Context.WorkingObjects.ToList();
            var InventorySelection = from inventory in Context.Inventories
                                 where inventory.Status == status.Aktīvs
                                 where inventory.InventoryUsages.All(a => a.Returned != null)
                                 select inventory;
            MainDataInventory.ItemsSource = InventorySelection.ToList();
            var UsageSelection = from usage in Context.InventoryUsage
                                 where usage.Returned == null
                                 select new
                                 {
                                     usage.Taken,
                                     usage.Employee,
                                     usage.WorkingObject,
                                     usage.Inventory.Model,
                                     usage.Inventory.InventoryId,
                                     usage.Inventory.Manufacturer,
                                     usage.Inventory.Name
                                 };
            MainDataUsages.ItemsSource = UsageSelection.ToList();
        }

        private void Take()
        {
            if (MainData.SelectedItem == null) return;
            Notifications notifications = new Notifications();

            Scanned = inputCode.Text;
            CleanTextFields();
            int InvID;
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
                //MessageBox.Show("Inventārs ar ID " + InvID + " neeksistē");
                return;
            }
            textInfoLarge.Text = Inventory.ToString();
            if(Inventory.Status == status.Norakstīts)
            {
                textStatus.Foreground = Brushes.Red;
                textStatus.Text = "Norakstīts!";
                return;
            }
            if((from usage in Inventory.InventoryUsages where usage.Returned == null select usage).Count() > 0)
            {
                textStatus.Foreground = Brushes.Yellow;
                textStatus.Text = "Jau paņemts!";
                notifications.E21(InvID);
                return;
            }

            InventoryUsage inventoryUsage = new InventoryUsage();
            Employee.InventoryUsages.Add(inventoryUsage);
            Inventory.InventoryUsages.Add(inventoryUsage);
            WorkingObject workingObject = (WorkingObject)MainData.SelectedItem;
            workingObject.InventoryUsages.Add(inventoryUsage);
            Context.SaveChanges();
            textStatus.Foreground = Brushes.LightGreen;
            textStatus.Text = "Paņemts!";
            //notifications.I15();
            RefreshData();
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
                Take();
            }
        }

        private void MainData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            inputCode.IsEnabled = true;
            inputCode.Focus();
        }
    }
}
