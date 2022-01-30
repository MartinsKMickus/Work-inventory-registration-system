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
    /// Interaction logic for ManageEmployee.xaml
    /// </summary>
    public partial class ManageEmployee : Window
    {
        InventorySystemContext Context;
        public ManageEmployee(InventorySystemContext context)
        {
            InitializeComponent();
            Context = context;
            Context.Employees.Load();
            RefreshEmployeelist();
        }

        private void RefreshEmployeelist()
        {
            Context.SaveChanges();
            var EmployeeSelection = from employee in Context.Employees
                                 where employee.Deleted != true
                                 select employee;
            listEmployees.ItemsSource = EmployeeSelection.ToList();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {

            EditEmployee editEmployee = new EditEmployee(null);
            editEmployee.ShowDialog();
            if (editEmployee.Employee != null)
            {
                Context.Employees.Add(editEmployee.Employee);
                RefreshEmployeelist();
            }
        }

        private void listEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            if (listEmployees.SelectedItem == null)
            {
                DisableButtons();
                return;
            }
            Notifications notifications = new Notifications();
            Employee employee = (Employee)listEmployees.SelectedItem;
            if (employee.InventoryUsages.Any(a => a.Returned == null))
            {
                if (notifications.A4() == MessageBoxResult.Yes)
                {

                    foreach (InventoryUsage inventoryUsage in employee.InventoryUsages) // All employee items are returned.
                    {
                        if (inventoryUsage.Returned == null)
                        {
                            inventoryUsage.Return();
                        }
                    }
                }
                else return;
            }
            else if (notifications.A3() == MessageBoxResult.No)
            {
                return;
            }
            Context.SaveChanges();
            employee.Deleted = true;
            employee.Name = "Dzēsts";
            employee.Surname = "Dzēsts";
            RefreshEmployeelist();
            DisableButtons();
            notifications.I7();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            EditEmployee editEmployee = new EditEmployee((Employee)listEmployees.SelectedItem);
            editEmployee.ShowDialog();
            RefreshEmployeelist();
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            ShowEmployeeInventory showEmployeeInventory = new ShowEmployeeInventory(Context, (Employee)listEmployees.SelectedItem);
            showEmployeeInventory.ShowDialog();
        }
    }
}
