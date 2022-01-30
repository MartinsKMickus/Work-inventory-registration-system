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
    /// Interaction logic for EditObject.xaml
    /// </summary>
    public partial class EditObject : Window
    {
        public WorkingObject WorkingObject;
        public EditObject(WorkingObject workingObject)
        {
            InitializeComponent();
            WorkingObject = workingObject;
            if (WorkingObject != null)
            {
                inputName.Text = WorkingObject.Name;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Notifications notifications = new Notifications();
            if(inputName.Text == "")
            {
                notifications.E1();
                return;
            }
            else if(inputName.Text.Length > 35)
            {
                notifications.E2();
                return;
            }

            if (WorkingObject == null)
            {
                WorkingObject = new WorkingObject()
                {
                    Name = inputName.Text,
                };
                notifications.I1(); // Slikti!!!
            }
            else
            {
                WorkingObject.Name = inputName.Text;
                notifications.I2(); // Slikti!!!
            }
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
