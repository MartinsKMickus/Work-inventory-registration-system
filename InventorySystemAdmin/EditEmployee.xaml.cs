using InventorySystemCore;
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
    /// Interaction logic for EditEmployee.xaml
    /// </summary>
    public partial class EditEmployee : Window
    {
        public Employee Employee;
        public EditEmployee(Employee employee)
        {
            InitializeComponent();
            Employee = employee;
            if (Employee != null)
            {
                inputName.Text = Employee.Name;
                inputSurname.Text = Employee.Surname;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Notifications notifications = new Notifications();
            if (inputName.Text == "")
            {
                notifications.E3();
                return;
            }
            else if (inputName.Text.Length > 20)
            {
                notifications.E4();
                return;
            }
            else if (inputSurname.Text == "")
            {
                notifications.E5();
                return;
            }
            else if (inputSurname.Text.Length > 20)
            {
                notifications.E6();
                return;
            }
            else if ((inputPassword.Password.Length > 10 || inputPassword.Password.Length < 4) && inputPassword.Password != "")
            {
                notifications.E7();
                return;
            }
            if (inputPassword.Password.Length == 0)
            {
                notifications.I4();
                inputPassword.Password = "0000"; // Default password.
            }
            Tools tools = new Tools();
            if (Employee == null)
            {
                Employee = new Employee()
                {
                    Name = inputName.Text,
                    Surname = inputSurname.Text,
                    Password = tools.CreatePassHash(inputPassword.Password)
                };
                notifications.I5(); // Slikti!!!
            }
            else
            {
                Employee.Name = inputName.Text;
                Employee.Surname = inputSurname.Text;
                Employee.Password = tools.CreatePassHash(inputPassword.Password);
                Employee.PasswordChange = true;
                notifications.I6(); // Slikti!!!
            }
            
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
