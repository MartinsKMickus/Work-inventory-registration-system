﻿using System;
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

namespace InventorySystem.views
{
    /// <summary>
    /// Interaction logic for windowChangePassword.xaml
    /// </summary>
    public partial class windowChangePassword : Window
    {
        public bool Cancel = false;
        public windowChangePassword()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel = true;
            Close();
        }
    }
}
