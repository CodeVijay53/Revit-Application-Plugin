using System;
using System.Collections.Generic;
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

namespace FinalApplication.Views
{
    /// <summary>
    /// Interaction logic for WallHeight.xaml
    /// </summary>
    public partial class WallHeight : Window
    {
        public double? Wallheight { get; private set; }
        public WallHeight()
        {
            InitializeComponent();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(HeightTextBox.Text,out double height))
            {
                Wallheight=Height;
                this.DialogResult=true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter the valid number");
            }
        }
    }
}
