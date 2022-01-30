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
    /// Interaction logic for EditSenderEmail.xaml
    /// </summary>
    public partial class EditSenderEmail : Window
    {
        public EditSenderEmail()
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
                if (inputEmail.Text == "")
                {
                    notifications.E15();
                    return;
                }
                Tools tools = new Tools();
                if (tools.CheckEmail(inputEmail.Text.ToLower()))
                {
                    Emailing emailing = new Emailing(inputPassword.Password, inputEmail.Text.ToLower());
                    emailing.Register(inputEmail.Text.ToLower());
                    EmailCheck emailCheck = new EmailCheck();
                    emailCheck.ShowDialog();
                    if (emailing.IsRegistrated(emailCheck.inputCode.Text))
                    {
                        emailing.Resave();
                        notifications.I13();
                    }
                    else
                    {
                        notifications.E17();
                        return;
                    }
                    ///return;
                }
                else
                {
                    notifications.E16();
                    return;
                }

            }
            catch (Exception)
            {
                //MessageBox.Show(i.Message);
                return;
            }
            Close();
        }
    }
}
