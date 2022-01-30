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
    /// Interaction logic for EmailCheck.xaml
    /// </summary>
    public partial class EmailCheck : Window
    {
        string Activation;
        public EmailCheck()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Activation = inputCode.Text.ToUpper();
            Close();
        }
    }
}
