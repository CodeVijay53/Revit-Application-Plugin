using Autodesk.Revit.DB;
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
    /// Interaction logic for DoorType.xaml
    /// </summary>
    public partial class DoorType : Window
    {   
        public FamilySymbol selectedDoorType {  get; private set; }
        private readonly List<FamilySymbol> _doorTypes;
        public DoorType(List<FamilySymbol> doorTypes)
        {
            InitializeComponent();
            _doorTypes = doorTypes;

            ///Populating in the combobox
            foreach (var doorType in _doorTypes) {
                DoorTypeComboBox.Items.Add(doorType.Name);
            }
        }

        public void OKButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex=DoorTypeComboBox.SelectedIndex;
            if (selectedIndex >= 0)
            {
                selectedDoorType=_doorTypes[selectedIndex]; 
                this.DialogResult=true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select the door type");
            }
        }
    }
}
