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
    /// Interaction logic for ColumnInfo.xaml
    /// </summary>
    public partial class ColumnInfo : Window
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public ColumnInfo()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ParameterName= ParameterNameTextBox.Text;
            ParameterValue= ParameterValueTextBox.Text;

            if(string.IsNullOrEmpty(ParameterName) || string.IsNullOrEmpty(ParameterValue))
            {
                MessageBox.Show("Please enter the Parameter Name and Parameter Value");
            }
            else
            {
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
