using InventorySystem.views;
using InventorySystemCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InventorySystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly InventorySystemContext _context = new InventorySystemContext();
        Employee LoginEmployee;
        public MainWindow()
        {
            InitializeComponent();
            if (!_context.Database.CanConnect())
            {
                MessageBox.Show("Nepieciešams palaist InventorySystemAdmin pirmo!");
                Application.Current.Shutdown();
                return;
            }
            try
            {
                _context.Employees.Load();
            }
            catch (Exception)
            {
                MessageBox.Show("Nepieciešams palaist InventorySystemAdmin pirmo!");
                Application.Current.Shutdown();
                return;
            }
            //EmailPasswords email = new EmailPasswords();
            //MessageBox.Show(email.GetString());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // clean up database connections
            _context.Dispose();
            base.OnClosing(e);
        }

        private void CheckLogin()
        {
            _context.Employees.Load();
            Tools tools = new Tools();
            try
            {
                LoginEmployee = _context.Employees.Find(int.Parse(inputID.Text));
            }
            catch (Exception)
            {
                NullReferenceException noexist = new NullReferenceException();
                throw noexist;
            }
            if(LoginEmployee.Deleted)
            {
                NullReferenceException noexist = new NullReferenceException();
                throw noexist;
            }
            tools.CheckPassword(inputPassword.Password, LoginEmployee.Password);
            if(LoginEmployee.PasswordChange)
            {
                Notifications notifications = new Notifications();
                windowChangePassword windowChangePassword = new windowChangePassword();
                windowChangePassword.ShowDialog();
                if(windowChangePassword.Cancel)
                {
                    throw new Exception();
                }
                if(windowChangePassword.inputPassword.Password == windowChangePassword.inputPassword2.Password)
                {
                    if(windowChangePassword.inputPassword.Password.Length < 4 || windowChangePassword.inputPassword.Password.Length > 10)
                    {
                        notifications.E7();
                        //Exception PasswordTerms = new Exception("Parole neatbilst nosacījumiem!");
                        throw new Exception();
                    }
                    LoginEmployee.Password = tools.CreatePassHash(windowChangePassword.inputPassword.Password);
                    LoginEmployee.PasswordChange = false;
                    _context.SaveChanges();
                    notifications.I14();
                }
                else
                {
                    notifications.E19();
                    throw new Exception();
                }
                ///////////////////////////////////////////////////////////////////////////////////////////////Password changed.
            }
        }

        private void ButtonTake_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckLogin();
                windowTakeItem windowTakeItem = new windowTakeItem(_context, LoginEmployee);
                windowTakeItem.ShowDialog();
            }
            catch (UnauthorizedAccessException)
            {
                Notifications notifications = new Notifications();
                notifications.E18();
                return;
            }
            catch (NullReferenceException)
            {
                Notifications notifications = new Notifications();
                notifications.E18();
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }
        }

        private void ButtonReturn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckLogin();
                windowReturnItem windowReturnItem = new windowReturnItem(_context, LoginEmployee);
                windowReturnItem.ShowDialog();
            }
            catch (UnauthorizedAccessException)
            {
                Notifications notifications = new Notifications();
                notifications.E18();
            }
            catch (NullReferenceException)
            {
                Notifications notifications = new Notifications();
                notifications.E18();
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }
        }

        private void ButtonReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckLogin();
                windowReport windowReport = new windowReport(_context, LoginEmployee);
                windowReport.ShowDialog();
            }
            catch (UnauthorizedAccessException)
            {
                Notifications notifications = new Notifications();
                notifications.E18();
            }
            catch (NullReferenceException)
            {
                Notifications notifications = new Notifications();
                notifications.E18();
            }
            catch (Exception exc)
            {
                //MessageBox.Show(exc.Message);
            }
        }

        private void inputID_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var temp = _context.Employees.Find(int.Parse(inputID.Text));
                if (temp.Deleted)
                {
                    infoEmployee.Text = "Ievadīt ID";
                }
                else infoEmployee.Text = temp.ToString();
            }
            catch (Exception)
            {
                infoEmployee.Text = "Ievadīt ID";
            }
        }
    }
}
