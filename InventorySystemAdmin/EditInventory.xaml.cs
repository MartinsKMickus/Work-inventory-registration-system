using InventorySystemCore;
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
    /// Interaction logic for EditInventory.xaml
    /// </summary>
    public partial class EditInventory : Window
    {
        public Inventory Inventory;
        public EditInventory(Inventory inventory)
        {
            InitializeComponent();
            inputStatus.ItemsSource = Enum.GetValues(typeof(status)).Cast<status>();
            inputStatus.SelectedItem = status.Aktīvs;
            Inventory = inventory;
            if (Inventory != null)
            {
                inputManufacturer.Text = Inventory.Manufacturer;
                inputModel.Text = Inventory.Model;
                inputDescription.Text = Inventory.Name;
                inputStatus.SelectedItem = Inventory.Status;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Notifications notifications = new Notifications();
            if (inputDescription.Text == "")
            {
                notifications.E8();
                return;
            }
            else if (inputDescription.Text.Length > 35)
            {
                notifications.E9();
                return;
            }
            else if (inputModel.Text == "")
            {
                notifications.E10();
                return;
            }
            else if (inputModel.Text.Length > 35)
            {
                notifications.E11();
                return;
            }
            else if (inputManufacturer.Text == "")
            {
                notifications.E12();
                return;
            }
            else if (inputManufacturer.Text.Length > 35)
            {
                notifications.E13();
                return;
            }

            if (Inventory == null)
            {
                Inventory = new Inventory()
                {
                    Manufacturer = inputManufacturer.Text,
                    Model = inputModel.Text,
                    Name = inputDescription.Text
                };
                notifications.I8();
            }
            else
            {
                Inventory.Manufacturer = inputManufacturer.Text;
                Inventory.Model = inputModel.Text;
                Inventory.Name = inputDescription.Text;
                Inventory.Status = (status)inputStatus.SelectedItem;
                notifications.I9();
            }
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
