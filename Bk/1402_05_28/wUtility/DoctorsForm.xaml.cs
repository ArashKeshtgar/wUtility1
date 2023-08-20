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
    /// Interaction logic for DoctorsForm.xaml
    /// </summary>
    public partial class DoctorsForm : Window
    {
        public static DataTable DoctorsTable  = new DataTable();

        public DoctorsForm()
        {
            InitializeComponent();
            FillDoctorsGrid();
        }

        private void FillDoctorsGrid()
        {
            try
            {
                using (DBHelper dbh = new DBHelper(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString))
                {
                    string query = @"
                                    SELECT        Code, fName, lName, Name, PostNo
                                    FROM            Doctors";
                    DoctorsTable = dbh.ExecuteQuery(query);
                    uiDoctorsGrid.ItemsSource = DoctorsTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiDoctorsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRow row =  uiDoctorsGrid.SelectedItem as DataRow;
            DoctorsPriceGroupsForm.DoctorCode =int.Parse(row["Code"].ToString());
            DoctorsPriceGroupsForm.DoctorFullName = row["Name"].ToString();
            this.Close();
        }
    }
}
