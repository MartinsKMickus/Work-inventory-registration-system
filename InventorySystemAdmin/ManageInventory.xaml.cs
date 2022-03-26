using InventorySystemCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ManageInventory.xaml
    /// </summary>
    public partial class ManageInventory : Window
    {
        int ShowingType = 0;
        InventorySystemContext Context;
        public ManageInventory(InventorySystemContext context)
        {
            InitializeComponent();
            Context = context;
            Context.Employees.Load();
            Status.ItemsSource = Enum.GetValues(typeof(status)).Cast<status>();
            RefreshInventorylist();
            DisableButtons();
        }

        private void RefreshInventorylist()
        {
            Context.SaveChanges();
            switch (ShowingType)
            {
                case 0:
                    {
                        var UsageSelection = from inventory in Context.Inventories
                                             where inventory.Status != status.Norakstīts
                                             select inventory;
                        listInventories.ItemsSource = UsageSelection.ToList();
                        break;
                    }
                case 1:
                    {
                        var UsageSelection = from inventory in Context.Inventories
                                             select inventory;
                        listInventories.ItemsSource = UsageSelection.ToList();
                        break;
                    }
                case 2:
                    {
                        var UsageSelection = from inventory in Context.Inventories
                                             where inventory.InventoryUsages.Any(a => a.Returned == null)
                                             select inventory;
                        listInventories.ItemsSource = UsageSelection.ToList();
                        break;
                    }
                case 3:
                    {
                        var UsageSelection = from inventory in Context.Inventories
                                             where inventory.InventoryUsages.All(a => a.Returned != null)
                                             select inventory;
                        listInventories.ItemsSource = UsageSelection.ToList();
                        break;
                    }
            }
            
            //listInventories.Items.Refresh();
            //listInventories.ItemsSource = Context.Inventories.ToList();
            //listInventories.Items.Refresh();
        }
        private void listInventories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
            ButtonInfo.IsEnabled = true;
        }
        private void DisableButtons()
        {
            ButtonEdit.IsEnabled = false;
            ButtonDelete.IsEnabled = false;
            ButtonInfo.IsEnabled = false;
        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listInventories.SelectedItem == null)
            {
                DisableButtons();
                return;
            }
            Notifications notifications = new Notifications();
            if (notifications.A5() == MessageBoxResult.Yes)
            {
                ((Inventory)listInventories.SelectedItem).Status = status.Norakstīts;
                notifications.I10();
                RefreshInventorylist();
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listInventories.SelectedItem == null)
            {
                DisableButtons();
                return;
            }
            EditInventory editInventory = new EditInventory((Inventory)listInventories.SelectedItem);
            editInventory.ShowDialog();
            RefreshInventorylist();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            EditInventory editInventory = new EditInventory(null);
            editInventory.ShowDialog();
            if (editInventory.Inventory != null)
            {
                Context.Inventories.Add(editInventory.Inventory);
                RefreshInventorylist();
            }
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            if(listInventories.SelectedItem == null)
            {
                DisableButtons();
                return;
            }
            ShowInventory showInventory = new ShowInventory(Context, (Inventory)listInventories.SelectedItem);
            showInventory.ShowDialog();
        }

        private void ButtonGenBarcodes_Click(object sender, RoutedEventArgs e)
        {
            if (listInventories.SelectedItems.Count == 0) return;
            string[] barcodes = new string[listInventories.SelectedItems.Count];
            int i = 0;
            foreach(Inventory item in listInventories.SelectedItems)
            {
                try
                {
                    if (item.InventoryId < 10)
                    {
                        barcodes[i++] = "00000" + item.InventoryId.ToString();
                    }
                    else if (item.InventoryId < 100)
                    {
                        barcodes[i++] = "0000" + item.InventoryId.ToString();
                    }
                    else if (item.InventoryId < 1000)
                    {
                        barcodes[i++] = "000" + item.InventoryId.ToString();
                    }
                    else if (item.InventoryId < 10000)
                    {
                        barcodes[i++] = "00" + item.InventoryId.ToString();
                    }
                    else if (item.InventoryId < 100000)
                    {
                        barcodes[i++] = "0" + item.InventoryId.ToString();
                    }
                    else
                    {
                        barcodes[i++] = item.InventoryId.ToString();
                    }
                }
                catch (Exception)
                {
                    barcodes[i++] = item.InventoryId.ToString();
                }
                
            }
            Notifications notifications = new Notifications();
            notifications.I11();
            Tools tools = new Tools();
            if ((bool)CheckboxPrint.IsChecked)
            {
                tools.GenerateBarcodes(barcodes, true);
            }
            else
            {
                tools.GenerateBarcodes(barcodes);
            }
            
        }

        private void CheckboxPrint_Checked(object sender, RoutedEventArgs e)
        {
            ButtonGenBarcodes.Content = "Printēt Svītrkodus";
        }

        private void CheckboxPrint_Unchecked(object sender, RoutedEventArgs e)
        {
            ButtonGenBarcodes.Content = "Ģenerēt Svītrkodus";
        }

        private void CheckboxShowDeleted_Checked(object sender, RoutedEventArgs e)
        {
            ShowingType = 1;
            RefreshInventorylist();
        }

        private void CheckboxShowDeleted_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowingType = 0;
            RefreshInventorylist();
        }

        private void CheckboxShowOnlyUnreturned_Checked(object sender, RoutedEventArgs e)
        {
            CheckboxShowOnlyOnPlace.IsChecked = false;
            CheckboxShowDeleted.IsEnabled = false;
            CheckboxShowDeleted.IsChecked = true;
            ShowingType = 2;
            RefreshInventorylist();
        }

        private void CheckboxShowOnlyUnreturned_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowingType = 0;
            CheckboxShowDeleted.IsEnabled = true;
            CheckboxShowDeleted.IsChecked = false;
            RefreshInventorylist();
        }

        private void CheckboxShowOnlyOnPlace_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckboxShowDeleted.IsEnabled = true;
            CheckboxShowDeleted.IsChecked = false;
            ShowingType = 0;
            RefreshInventorylist();
        }

        private void CheckboxShowOnlyOnPlace_Checked(object sender, RoutedEventArgs e)
        {
            CheckboxShowOnlyUnreturned.IsChecked = false;
            CheckboxShowDeleted.IsEnabled = false;
            CheckboxShowDeleted.IsChecked = true;
            ShowingType = 3;
            RefreshInventorylist();
        }
    }
}
