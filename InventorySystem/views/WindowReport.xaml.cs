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

namespace InventorySystem.views
{
    /// <summary>
    /// Interaction logic for WindowReport.xaml
    /// </summary>
    public partial class windowReport : Window
    {
        InventorySystemContext Context;
        Inventory Inventory;
        Employee Employee;
        string Scanned;
        public windowReport(InventorySystemContext context, Employee employee)
        {
            InitializeComponent();
            Context = context;
            Employee = employee;
            RefreshData();
            inputCode.Focus();
        }
        void CleanTextFields()
        {
            textInfoLarge.Text = "";
            textStatus.Text = "";
            inputCode.Text = "";
        }

        private void RefreshData()
        {
            Context.Inventories.Load();
            //comboObjects.ItemsSource = Context.WorkingObjects.ToList();
        }

        private void ShowInfo()
        {
            Notifications notifications = new Notifications();
            Scanned = inputCode.Text;
            CleanTextFields();
            int InvID;
            try
            {
                InvID = int.Parse(Scanned);
            }
            catch (Exception)
            {
                /// E10
                inputCode.Text = "";
                MessageBox.Show("Nav ievadīts inventāra ID");
                return;
            }
            Inventory = Context.Inventories.Find(InvID);
            if (Inventory == null)
            {
                notifications.E20(InvID);
                return;
            }
            if (Inventory.Status == status.Norakstīts)
            {
                textStatus.Foreground = Brushes.Red;
                textStatus.Text = "Norakstīts!";
                return;
            }
            if (Inventory.Status == status.Bojāts)
            {
                textStatus.Foreground = Brushes.Yellow;
                textStatus.Text = "Jau ziņots par bojājumu!";
                return;
            }

            textInfoLarge.Text = Inventory.ToString();

            /// Inventory code not found!
            /// 
        }

        private void inputCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inputCode.Text.Length > 5)
            {
                if (inputCode.Text == "")
                {
                    CleanTextFields();
                    return;
                }
                ShowInfo();
                Console.Beep(650, 240);
            }
        }

        private void buttonReport_Click(object sender, RoutedEventArgs e)
        {
            Notifications notifications = new Notifications();
            DamageReport damageReport = new DamageReport(Inventory, inputReport.Text);
            if (Context.DamageReports.Count() == 1)
                Context.DamageReports.Update(damageReport);
            else
                Context.DamageReports.Add(damageReport);
            Context.SaveChanges(); ////////////////////////// Otreizējam bojājumam kļūda.
            Emailing emailing = new Emailing();
            string emailmessage = $"Bojāts Inventārs Nr: {Inventory.InventoryId}\n\n";
            emailmessage += $"Inventāra Nosaukums: {Inventory.Name}\nInventāra Ražotājs: {Inventory.Manufacturer}\nInventāra Modelis: {Inventory.Model}\n\n";
            emailmessage += $"Problēma: {damageReport.Description}\n\n";
            emailmessage += $"Ziņoja: {Employee}\n\n";
            if (Inventory.InventoryUsages.Count() > 0)
            {
                emailmessage += $"Lietošanas reizes: {Inventory.InventoryUsages.Count}\n\n";
                emailmessage += $"Dati par pēdējo lietošanas reizi:\n{Inventory.InventoryUsages.Last()}\n\n\n";
            }
            else
            {
                emailmessage += "Šis inventārs vēl nav bijis izmantots.\n\n\n";
            }
            emailmessage += "Jūsu Inventāra Sistēma";
            foreach (Administrator administrator in Context.Administrators)
            {
                emailing.SendEmail($"Bojāts Inventārs Nr: {Inventory.InventoryId}", emailmessage, administrator.Email);
            }
            notifications.I17();
            /// Notification about damage.
            /// 
            Close();
        }
    }
}
