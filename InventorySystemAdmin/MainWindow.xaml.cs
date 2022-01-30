using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using InventorySystemCore;
using Microsoft.EntityFrameworkCore;

namespace InventorySystemAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly InventorySystemContext _context = new InventorySystemContext();

        //private CollectionViewSource categoryViewSource;
        public MainWindow()
        {
            InitializeComponent();
            Directory.CreateDirectory(_context.DBlocation); // Local database creation ONLY!!!
            if (SenderEmailCheck()) return;
            if (!_context.Database.CanConnect())
            {
                _context.Database.EnsureCreated();
                
                if (RegisterAdmin()) return;
                //MessageBox.Show("Sistēma ir izveidota!");
                //MessageBox.Show("Izveidotā administratora ID ir 1");
            }
            if (_context.Administrators.Count() == 0)
            {
                if (RegisterAdmin()) return;
                //MessageBox.Show("Izveidotā administratora ID ir 1");
            }
            try /////////////////////////////////////////////////////////////////////// Replace with update!!!!!!
            {
                _context.Employees.Load();
            }
            catch
            {
                if (MessageBox.Show("Tiek glabāta veca datubāze!\nVai izdzēst datus un veidot jaunu?", "Izdzēst?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Directory.Delete(_context.DBlocation, true);
                    Application.Current.Shutdown();
                    return;
                }
                else
                {
                    MessageBox.Show("Sistēmas kļūda");
                    Application.Current.Shutdown();
                    return;
                }
            }
        }
        private bool RegisterAdmin()
        {
            EditAdministrator editAdministrator = new EditAdministrator();
            editAdministrator.ShowDialog();
            if (editAdministrator.administrator == null)
            {
                Application.Current.Shutdown();
                return true;
            }
            else
            {
                Notifications notifications = new Notifications();
                Emailing emailing = new Emailing();
                emailing.Register(editAdministrator.administrator.Email);
                EmailCheck emailCheck = new EmailCheck();
                emailCheck.ShowDialog();
                if (emailing.IsRegistrated(emailCheck.inputCode.Text.ToUpper())) // Seperate for administrator.registered bool functionality
                {
                    emailing.Resave();
                    editAdministrator.administrator.Registered = true;
                    notifications.I12();
                }
                else
                {
                    notifications.E17();
                    Application.Current.Shutdown();
                    return true;
                }
                _context.Administrators.Load();
                _context.Administrators.Add(editAdministrator.administrator);
                _context.SaveChanges();
                //MessageBox.Show("Sistēmas administrators ir izveidots!");
                return false;
                ///return;
            }
        }
        private bool SenderEmailCheck()
        {
            try
            {
                InfoStoring infoStoring = new InfoStoring();
            }
            catch (Exception)
            {
                EditSenderEmail editSenderEmail = new EditSenderEmail();
                editSenderEmail.ShowDialog();
            }
            try
            {
                InfoStoring infoStoring = new InfoStoring();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                Application.Current.Shutdown();
                return true;
            }
            return false;
        }
        private void CheckLogin()
        {
            
            inputID.Text = inputID.Text.ToLower();
            _context.Administrators.Load();
            Tools tools = new Tools();
            Administrator LoginAdministrator = _context.Administrators.Find(inputID.Text);
            if (LoginAdministrator == null) throw new UnauthorizedAccessException("Nepareizs Epasts!");
            tools.CheckPassword(inputPassword.Password, LoginAdministrator.Password);
            //inputID.Text = "";
            //inputPassword.Clear();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // clean up database connections
            _context.Dispose();
            base.OnClosing(e);
        }

        private void ButtonInventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckLogin();
                ManageInventory manageInventory = new ManageInventory(_context);
                manageInventory.ShowDialog();
            }
            catch (UnauthorizedAccessException)
            {
                Notifications notifications = new Notifications();
                notifications.E14();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message); // Change to fixed Message!!!
            }
        }

        private void ButtonEmployees_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckLogin();
                ManageEmployee manageEmployee = new ManageEmployee(_context);
                manageEmployee.ShowDialog();
            }
            catch (UnauthorizedAccessException)
            {
                Notifications notifications = new Notifications();
                notifications.E14();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message); // Change to fixed Message!!!
            }
        }

        private void ButtonObjects_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckLogin();
                ManageObject manageObject = new ManageObject(_context);
                manageObject.ShowDialog();
            }
            catch (UnauthorizedAccessException)
            {
                Notifications notifications = new Notifications();
                notifications.E14();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message); // Change to fixed Message!!!
            }
        }
    }
}
