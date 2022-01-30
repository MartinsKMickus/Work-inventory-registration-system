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
    /// Interaction logic for EditAdministrator.xaml
    /// </summary>
    public partial class EditAdministrator : Window
    {
        public Administrator administrator = null;
        public EditAdministrator()
        {
            InitializeComponent();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Notifications notifications = new Notifications();
            try
            {
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
                else if (inputEmail.Text == "")
                {
                    notifications.E15();
                    return;
                }
                else if (inputPassword.Password.Length > 10 || inputPassword.Password.Length < 4)
                {
                    notifications.E7();
                    return;
                }
                administrator = new Administrator(inputEmail.Text.ToLower(), inputName.Text, inputPassword.Password);

            }
            catch (Exception)
            {
                notifications.E16(); // Slikti!!!
                //MessageBox.Show(i.Message);
                administrator = null;
                return;
            }
            Close();
        }
    }
}
