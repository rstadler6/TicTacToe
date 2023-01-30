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
using System.Windows.Shapes;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for StartSelection.xaml
    /// </summary>
    public partial class StartSelection : Window
    {
        public StartSelection()
        {
            InitializeComponent();
        }

        private void AIButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void PlayerButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
