using System;
using System.Collections.Generic;
using System.Data;
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
using wUtility.Utils;

namespace wUtility
{
    /// <summary>
    /// Interaction logic for PartsForm.xaml
    /// </summary>
    public partial class PartsForm : Window
    {
        public static DataTable PartTable = new DataTable();
        public PartsForm()
        {
            InitializeComponent();
            FillPartsGrid();
        }

        private void FillPartsGrid()
        {
            try
            {
                using (DBHelper dbh = new DBHelper(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString))
                {
                    string query = @"
                                    select Name,Code
                                    from Parts";
                    PartTable = dbh.ExecuteQuery(query);
                    uiPartsGrid.ItemsSource = PartTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiDoctorsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void uiPartsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRow row = uiPartsGrid.SelectedItem as DataRow;
            AmvalFrm.CurretntPartCode = int.Parse(row["Code"].ToString());
            AmvalFrm.CurrentPartName = row["Name"].ToString();
            this.Close();
        }
    }
}
