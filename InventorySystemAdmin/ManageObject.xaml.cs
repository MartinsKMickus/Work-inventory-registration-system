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
    /// Interaction logic for ManageObject.xaml
    /// </summary>
    public partial class ManageObject : Window
    {
        InventorySystemContext Context;
        public ManageObject(InventorySystemContext context)
        {
            InitializeComponent();
            Context = context;
            Context.WorkingObjects.Load();
            RefreshObjectlist();
        }

        private void RefreshObjectlist()
        {
            Context.SaveChanges();
            var ObjectSelection = from workObject in Context.WorkingObjects
                                    where workObject.Deleted != true
                                    select workObject;
            listObjects.ItemsSource = ObjectSelection.ToList();
            //listObjects.ItemsSource = Context.WorkingObjects.ToList();
            //listObjects.Items.Refresh();
        }

        private void listObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonEdit.IsEnabled = true;
            ButtonInfo.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
        }
        private void DisableButtons()
        {
            ButtonEdit.IsEnabled = false;
            ButtonDelete.IsEnabled = false;
            ButtonInfo.IsEnabled = false;
        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listObjects.SelectedItem == null)
            {
                DisableButtons();
                return;
            }
            Notifications notifications = new Notifications();
            WorkingObject workingObject = (WorkingObject)listObjects.SelectedItem;
            if (workingObject.InventoryUsages.Any(a => a.Returned == null))
            {
                if (notifications.A2() == MessageBoxResult.Yes)
                {

                    foreach (InventoryUsage inventoryUsage in workingObject.InventoryUsages) // All working object items are returned.
                    {
                        if (inventoryUsage.Returned == null)
                        {
                            inventoryUsage.Return();
                        }
                    }
                }
                else return;
            }
            else if (notifications.A1() == MessageBoxResult.No)
            {
                return;
            }
            workingObject.Name = "Dzēsts";
            workingObject.Deleted = true;
            RefreshObjectlist();
            DisableButtons();
            notifications.I3();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listObjects.SelectedItem == null)
            {
                DisableButtons();
                return;
            }
            EditObject editObject = new EditObject((WorkingObject)listObjects.SelectedItem);
            editObject.ShowDialog();
            RefreshObjectlist();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            EditObject editObject = new EditObject(null);
            editObject.ShowDialog();
            if (editObject.WorkingObject != null)
            {
                Context.WorkingObjects.Add(editObject.WorkingObject);
                Notifications notifications = new Notifications();
                RefreshObjectlist();
            }
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            if (listObjects.SelectedItem == null)
            {
                DisableButtons();
                return;
            }
            ShowWorkingObjectInventory showWorkingObjectInventory = new ShowWorkingObjectInventory(Context, (WorkingObject)listObjects.SelectedItem);
            showWorkingObjectInventory.ShowDialog();
        }
    }
}
