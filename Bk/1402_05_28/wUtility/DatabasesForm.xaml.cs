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
using Telerik.Windows.Controls;
using wUtility.UserControls;


//using System.Windows.Forms;

namespace wUtility
{
    /// <summary>
    /// Interaction logic for DatabasesForm.xaml
    /// </summary>
    public partial class DatabasesForm : Window
    {
        public DataTable DatabasesDataTable { get; private set; }

        public DatabasesForm()
        {

            InitializeComponent();
            if (DBHelper.CheckDbConfigDbsConnection())
            {
               
                FillDatabasesGird();
                SetControls();
                SetModelDatabasesControl();
                FillConnectedDatabasesGird();
                //using (var loginForm = new LoginForm())
                //{
                //    loginForm.ShowDialog();
                //    if (Program.LoginUser == null)
                //    {
                //        Environment.Exit(0);
                //    }Data Source= . ;Initial Catalog = Totalsystem_Old;Integrated Security=True
                //}
            }
            else
            {
                MessageBox.Show("امکان اتصال به دیتابیس dbConfigDataBases وجود ندارد. لطفا تنظیمات دیتابیس را بررسی نمایید.", "خطا");
                Connections connections = new Connections(null,false);
                connections.ShowDialog();
            }


        }

        private void FillConnectedDatabasesGird()
        {
            try
            {
                if(uiDatabaseModelCmb.Text == CommanClass.DatabaseModelName.Totalsystem)
                {
                    //Totalsystem totalsystem = new Totalsystem();
                    //List<Totalsystem.TotalsystemDatabase> vs = Totalsystem.CurrentTotalsystem.TotalsystemDatabasesList;
                    DataTable data = Totalsystem.CurrentTotalsystem.FillTotalsystemConnectionStringsList();
                    uiConnectedDatabasesGrid.ItemsSource = data;
                    uiConnectedDatabasesGrid.Rebind();
                }

                if (uiDatabaseModelCmb.Text == CommanClass.DatabaseModelName.Lab)
                {
                    DataTable data = Lab.CurrentLab.FillLabConnectionStringsList();
                    uiConnectedDatabasesGrid.ItemsSource = data;
                    uiConnectedDatabasesGrid.Rebind();
                }

                if (uiDatabaseModelCmb.Text == CommanClass.DatabaseModelName.Radio)
                {
                    DataTable data = Radio.CurrentRadio.FillRadioConnectionStringsList();
                    uiConnectedDatabasesGrid.ItemsSource = data;
                    uiConnectedDatabasesGrid.Rebind();
                }

                if (uiDatabaseModelCmb.Text == CommanClass.DatabaseModelName.Drug)
                {
                    DataTable data = Drug.CurrentDrug.FillDrugConnectionStringsList();
                    uiConnectedDatabasesGrid.ItemsSource = data;
                    uiConnectedDatabasesGrid.Rebind();
                }

                if (uiDatabaseModelCmb.Text == CommanClass.DatabaseModelName.Phisio)
                {
                    DataTable data = Phisio.CurrentPhisio.FillPhisioConnectionStringsList();
                    uiConnectedDatabasesGrid.ItemsSource = data;
                    uiConnectedDatabasesGrid.Rebind();
                }

                if (uiDatabaseModelCmb.Text == CommanClass.DatabaseModelName.OpRoom)
                {
                    DataTable data = OpRoom.CurrentOpRoom.FillOpRoomConnectionStringsList();
                    uiConnectedDatabasesGrid.ItemsSource = data;
                    uiConnectedDatabasesGrid.Rebind();
                }

                if (uiDatabaseModelCmb.Text == CommanClass.DatabaseModelName.Blood)
                {
                    DataTable data = Blood.CurrentBlood.FillBloodConnectionStringsList();
                    uiConnectedDatabasesGrid.ItemsSource = data;
                    uiConnectedDatabasesGrid.Rebind();
                }

                if (uiDatabaseModelCmb.Text == CommanClass.DatabaseModelName.Food)
                {
                    DataTable data = Food.CurrentFood.FillFoodConnectionStringsList();
                    uiConnectedDatabasesGrid.ItemsSource = data;
                    uiConnectedDatabasesGrid.Rebind();
                }
                if (uiDatabaseModelCmb.Text == CommanClass.DatabaseModelName.Den)
                {
                    DataTable data = Den.CurrentDen.FillDenConnectionStringsList();
                    uiConnectedDatabasesGrid.ItemsSource = data;
                    uiConnectedDatabasesGrid.Rebind();
                }
                if (uiDatabaseModelCmb.Text == CommanClass.DatabaseModelName.Tajhizat)
                {
                    DataTable data = Tajhizat.CurrentTajhizat.FillTajhizatConnectionStringsList();
                    uiConnectedDatabasesGrid.ItemsSource = data;
                    uiConnectedDatabasesGrid.Rebind();
                }
                if (uiDatabaseModelCmb.Text == CommanClass.DatabaseModelName.Tasisat)
                {
                    DataTable data = Tasisat.CurrentTasisat.FillTasisatConnectionStringsList();
                    uiConnectedDatabasesGrid.ItemsSource = data;
                    uiConnectedDatabasesGrid.Rebind();
                }
                if (uiDatabaseModelCmb.Text == CommanClass.DatabaseModelName.Requests)
                {
                    DataTable data = Requests.CurrentRequests.FillRequestsConnectionStringsList();
                    uiConnectedDatabasesGrid.ItemsSource = data;
                    uiConnectedDatabasesGrid.Rebind();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void SetModelDatabasesControl()
        {
            try
            {
                List<string> vs = CommanClass.DatabaseModelName.ConnectedDatabases;
                uiDatabaseModelCmb.ItemsSource = vs;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        private void FillDatabasesGird()
        {
            try
            {
                using (DBHelper dbh = new DBHelper(dbConfigDataBases.dbConfigDataBasesConnectionString))
                {
                    string query = @"SELECT         *
                                    FROM          Tbl_dbConfigSetting 
                                                    ";
                    DatabasesDataTable = dbh.ExecuteQuery(query);
                    uiDatabasesGrid.ItemsSource = DatabasesDataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا"); 
            }
        }

        private void uiDatabasesGrid_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {

        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseControl databaseControl = new DatabaseControl();
            databaseControl.Visibility = Visibility.Visible;
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void uiTotalsystemBtn_Click(object sender, RoutedEventArgs e)
        {
           Connections connections = new Connections("Totalsystem", true);
            connections.ShowDialog();
            if (connections.DialogResult.Value)
                FillConnectedDatabasesGird();
        }

        private void SetControls()
        {
            try
            {
                TotalsystemControlsChecked();
                LabControlsChecked();
                RadioControlsChecked();
                DrugControlsChecked();
                BloodControlsChecked();
                OpRoomControlsChecked();
                PhisioControlsChecked();
                FoodControlsChecked();
                DenControlsChecked();
                TajhizatControlsChecked();
                TasisatControlsChecked();
                MDocChecked();
                RequestsChecked();
                AmvalControlsChecked();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
          
        }

        private void uiTotalsystemChb_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void TotalsystemControlsChecked()
        {
            if (DBHelper.CheckDbConnection1(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString);

                uiTotalsystemChb.IsChecked = true;
                uiTotalsystemText.Text = databaseName;
        
                bool result = CheckDatabasesList(databaseName);
                //if (CommonClass.ConnectedDatabases.ConnectedDatabasesList.Count > 0)
                //    result = CommonClass.ConnectedDatabases.ConnectedDatabasesList.Where(es => es == databaseName).Any();

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiTotalsystemChb.IsChecked = false;
        }

        private void LabControlsChecked()
        {
            
            if (DBHelper.CheckDbConnection1(Lab.CurrentLab.LabConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(Lab.CurrentLab.LabConnectionString);

                uiLabChb.IsChecked = true;
                uiLabText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiLabChb.IsChecked = false;
        }

        private void RadioControlsChecked()
        {
            if (DBHelper.CheckDbConnection1(Radio.CurrentRadio.RadioConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(Radio.CurrentRadio.RadioConnectionString);

                uiRadioChb.IsChecked = true;
                uiRadioText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiRadioChb.IsChecked = false;

        }

        private void DrugControlsChecked()
        {
            if (DBHelper.CheckDbConnection1(Drug.CurrentDrug.DrugConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(Drug.CurrentDrug.DrugConnectionString);

                uiDrugChb.IsChecked = true;
                uiDrugText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (CommonClass.ConnectedDatabases.ConnectedDatabasesList.Count > 0)
                //    result = CommonClass.ConnectedDatabases.ConnectedDatabasesList.Where(es => (es != databaseName)).Any();

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiDrugChb.IsChecked = false;

        }

        private void BloodControlsChecked()
        {
            if (DBHelper.CheckDbConnection1(Blood.CurrentBlood.BloodConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(Blood.CurrentBlood.BloodConnectionString);

                uiBloodChb.IsChecked = true;
                uiBloodText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiBloodChb.IsChecked = false;

        }

        private void OpRoomControlsChecked()
        {
            if (DBHelper.CheckDbConnection1(OpRoom.CurrentOpRoom.OpRoomConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(OpRoom.CurrentOpRoom.OpRoomConnectionString);

                uiOpRoomChb.IsChecked = true;
                uiOpRoomText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiOpRoomChb.IsChecked = false;

        }

        private void PhisioControlsChecked()
        {
            if (DBHelper.CheckDbConnection1(Phisio.CurrentPhisio.PhisioConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(Phisio.CurrentPhisio.PhisioConnectionString);

                uiPhisioChb.IsChecked = true;
                uiPhisioText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiPhisioChb.IsChecked = false; 

        }


        private void FoodControlsChecked()
        {
            if (DBHelper.CheckDbConnection1(Food.CurrentFood.FoodConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(Food.CurrentFood.FoodConnectionString);

                uiFoodChb.IsChecked = true;
                uiFoodText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiFoodChb.IsChecked = false;

        }

        private void DenControlsChecked()
        {
            if (DBHelper.CheckDbConnection1(Den.CurrentDen.DenConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(Den.CurrentDen.DenConnectionString);

                uiDenChb.IsChecked = true;
                uiDenText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiDenChb.IsChecked = false;

        }

        private void TasisatControlsChecked()
        {
            if (DBHelper.CheckDbConnection1(Tasisat.CurrentTasisat.TasisatConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(Tasisat.CurrentTasisat.TasisatConnectionString);

                uiTasisatChb.IsChecked = true;
                uiTasisatText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiTasisatChb.IsChecked = false;

        }

        private void TajhizatControlsChecked()
        {
            if (DBHelper.CheckDbConnection1(Tajhizat.CurrentTajhizat.TajhizatConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(Tajhizat.CurrentTajhizat.TajhizatConnectionString);

                uiTajhizatChb.IsChecked = true;
                uiTajhizatText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiTajhizatChb.IsChecked = false;

        }

        private void MDocChecked()
        {
            if (DBHelper.CheckDbConnection1(MDoc.CurrentMDoc.MDocConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(MDoc.CurrentMDoc.MDocConnectionString);

                uiMDocChb.IsChecked = true;
                uiMDocText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiMDocChb.IsChecked = false;

        }

        private void RequestsChecked()
        {
            if (DBHelper.CheckDbConnection1(Requests.CurrentRequests.RequestsConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(Requests.CurrentRequests.RequestsConnectionString);

                uiRequestsChb.IsChecked = true;
                uiRequestsText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiRequestsChb.IsChecked = false;

        }

        private bool CheckDatabasesList(string databaseName)
        {
            bool result = false;

            if (CommanClass.DatasourceDatabases.DatasoursesDatabasesList.Count > 0)
                result = CommanClass.DatasourceDatabases.DatasoursesDatabasesList.Where(es => es.DatasourceDatabaseName == databaseName).Any();

            if (result == true)
                return true;
            else
                return false;
        }
        private void AmvalControlsChecked()
        {
            if (DBHelper.CheckDbConnection1(Amval.CurrentAmval.AmvalConnectionString))
            {
                string databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(Amval.CurrentAmval.AmvalConnectionString);

                uiAmvalChb.IsChecked = true;
                uiAmvalText.Text = databaseName;

                bool result = CheckDatabasesList(databaseName);

                //if (result == false)
                //    CommonClass.DatasourceDatabases.DatasoursesDatabasesList.Add(databaseName);
            }

            else
                uiAmvalChb.IsChecked = false;

        }
        private void uiTotalsystemChb_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                //uiTotalsystemText.IsEnabled = true;
                //uiTotalsystemBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private void uiLabBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(CommanClass.DatabaseModelName.Lab, true);
            connections.ShowDialog();
        }

        private void uiRadioBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(CommanClass.DatabaseModelName.Radio, true);
            connections.ShowDialog();
        }

        private void uiRadioChb_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void uiLabChb_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void uiTajhizatBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(CommanClass.DatabaseModelName.Tajhizat, true);
            connections.ShowDialog();
            
        }

        private void uiTajhizatChb_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void uiTajhizatChb_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void uiTasisatChb_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void uiTasisatChb_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void uiTasisatBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(CommanClass.DatabaseModelName.Tasisat, true);
            connections.ShowDialog();
        }

        private void uiDenChb_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void uiDenChb_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void uiDenBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(CommanClass.DatabaseModelName.Den, true);
            connections.ShowDialog();
        }

        private void uiFoodChb_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void uiFoodChb_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void uiFoodBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(CommanClass.DatabaseModelName.Food, true);
            connections.ShowDialog();

        }

        private void uiPhisioChb_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void uiPhisioChb_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void uiPhisioBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(CommanClass.DatabaseModelName.Phisio, true);
            connections.ShowDialog();
        }

        private void uiOpRoomChb_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void uiOpRoomChb_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void uiOpRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(CommanClass.DatabaseModelName.OpRoom, true);
            connections.ShowDialog();
        }

        private void uiBloodChb_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void uiBloodChb_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void uiBloodBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(CommanClass.DatabaseModelName.Blood, true);
            connections.ShowDialog();
        }

        private void uiDrugChb_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void uiDrugChb_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void uiDrugBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(CommanClass.DatabaseModelName.Drug, true);
            connections.ShowDialog();
        }

        private void uiMDocChb_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void uiMDocChb_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void uiMDocBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(CommanClass.DatabaseModelName.MDoc, true);
            connections.ShowDialog();
        }

        private void uiDatabaseModelBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(uiDatabaseModelCmb.Text, false);
            connections.ShowDialog();
            if (connections.DialogResult.Value)
                FillConnectedDatabasesGird();
        }

        private void uiDatabaseModelCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillConnectedDatabasesGird();
        }

        private void uiRequestsBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections(CommanClass.DatabaseModelName.Requests, true);
            connections.ShowDialog();
        }

        private void uiAmvalBtn_Click(object sender, RoutedEventArgs e)
        {
            Connections connections = new Connections("Amval", true);
            connections.ShowDialog();
        }
    }
}
