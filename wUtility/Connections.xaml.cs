using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for Connections.xaml
    /// </summary>
    /// 
    
    public partial class Connections : Window
    {
        private bool _TestConnection = false;
        private bool _TestConnectionDatabase = false;
        private bool _IsCurrect;
        private string FullCurrentConnectionString = "";
        DataTable DatabasesDataTable = new DataTable();
        static private string _DatabaseModelName;
        public Connections(string databaseModelName, bool isCurrect)
        {
            InitializeComponent();
            if (databaseModelName != null)
            {
                _DatabaseModelName = databaseModelName;
                _IsCurrect = isCurrect;
            }
            else
                _DatabaseModelName = "dbConfigDataBases";
        }

        private string GetFullConnectionString(string databaseName)
        {
            if (_TestConnection)
            {
                if (databaseName != "")
                {
                    string connStr = GetConnectionString();
                    string full = "";
                    if (uiModeCmb.Text == "Windows")
                    {
                        string[] fullArray = connStr.Split(';');

                        if (uiServerNameText.Text == "")
                            fullArray[0] = fullArray[0] + " . ";

                        full = fullArray[0] + ";Initial Catalog = " + databaseName + ";" + fullArray[1];
                        return full;
                    }
                    else if (uiModeCmb.Text == "Sql")
                    {
                        connStr = "server=" + uiServerNameText.Text
                                            + ";Initial Catalog = " + databaseName
                                            + ";uid=" + uiUserNameText.Text
                                            + ";pwd=" + uiPasswordText.Password + ";";
                        return connStr;
                    }
                }
            }

            return "";
        }

        private string GetConnectionString()
        {
            string conString = "";
            if (uiModeCmb.Text == "Windows")
            {
                if (uiUserNameText.Text == "" && uiPasswordText.Password == "")
                    conString = "Data Source=" + uiServerNameText.Text+ ";Integrated Security=True";
            }

            else if (uiModeCmb.Text == "Sql")
            {
                conString = "server=" + uiServerNameText.Text + ";uid=" + uiUserNameText.Text + ";pwd=" + uiPasswordText.Password + ";";
            }

            return conString;
        }

        bool TestDataBaseConnection(string connectionString, bool showMessage)
        {
            try
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    using (var db = new DBHelper(connectionString))
                    {
                        DataTable dt = db.ExecuteQuery(" select 'Connection Check' ");
                        if (showMessage && dt != null)
                            MessageBox.Show(".با موفقیت انجام شد" + uiDatabaseCmb.Text + "اتصال با دیتابیس");
                        return true;
                    }
                }
                else
                {
                    TestConnectionWitoutConnectionString();
                    return true;
                }
            }
            catch (Exception ex)
            {
                var errorDetails = ex.Message;
                if (ex.InnerException != null)
                    errorDetails += "\r\n" + ex.InnerException.Message;
                if (showMessage)
                    MessageBox.Show("خطا در اتصال:\r\n" + errorDetails);
                return false;
            }
        }

        public void TestConnectionWitoutConnectionString()
        {
            SqlConnection connection = new SqlConnection(GetConnectionString());
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Connection = connection;
            sqlCommand.CommandText = " select 'Connection Check' ";

            SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", connection);

            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            SqlDataAdapter da1 = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);
            if(DatabasesDataTable.Rows.Count >0)
                DatabasesDataTable.Rows.Clear();
            da1.Fill(DatabasesDataTable);

        }

        List<string> DatabasesList = new List<string>();
        private void SetDatabsesCmb()
        {
            List<string> list = new List<string>();
            DataTable table = new DataTable();
            table.Columns.Add("ID");
            table.Columns.Add("Name");
            foreach(DataRow dataRow in DatabasesDataTable.Rows)
            {
                ItemsControl control = new ItemsControl();
                list.Add(dataRow[0].ToString());
                
            }
            //if (DatabasesList.Count > 0)
            //    DatabasesList.RemoveRange(0, DatabasesList.Count());

            //DatabasesList = list;
            uiDatabaseCmb.ItemsSource = list;
        }
        private void uiConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TestDataBaseConnection(GetFullConnectionString(uiDatabaseCmb.Text), false))
                {
                    FullCurrentConnectionString = GetFullConnectionString(uiDatabaseCmb.Text);
                    _TestConnectionDatabase = true;
                    string s = uiDatabaseCmb.Text;
                    
                    //CommonClass.ConnectedDatabases.DatasoursesDatabasesList.Add(s);
                    MessageBox.Show(".با موفقیت انجام شد" + uiDatabaseCmb.Text + "اتصال با دیتابیس");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
        }

        private void uiSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_TestConnectionDatabase)
                {
                    if (_DatabaseModelName == CommanClass.DatabaseModelName.dbConfigDataBases)
                        dbConfigDataBases.dbConfigDataBasesConnectionString = FullCurrentConnectionString;

                    if (_DatabaseModelName == CommanClass.DatabaseModelName.Totalsystem)
                    {
                        if(_IsCurrect)
                            Totalsystem.CurrentTotalsystem.TotalsystemConnectionString = FullCurrentConnectionString;
                        else
                        {
                            Totalsystem.TotalsystemDatabase totalsystemDatabase = new Totalsystem.TotalsystemDatabase(null);
                            totalsystemDatabase.ConnectionString = FullCurrentConnectionString;
                        }
                    }

                    if (_DatabaseModelName == CommanClass.DatabaseModelName.Lab)
                    {
                        Lab.CurrentLab.LabConnectionString = FullCurrentConnectionString;
                    }

                    if (_DatabaseModelName == CommanClass.DatabaseModelName.Radio)
                    {
                        if (_IsCurrect)
                            Radio.CurrentRadio.RadioConnectionString = FullCurrentConnectionString;
                        else
                        {
                            Radio.RadioDatabase totalsystemDatabase = new Radio.RadioDatabase(null);
                            totalsystemDatabase.ConnectionString = FullCurrentConnectionString;
                        }
                        //Radio.CurrentRadio.RadioConnectionString = FullCurrentConnectionString;
                        //Radio.CurrentRadio.DatabaseName = uiDatabaseCmb.Text;
                    }

                    if (_DatabaseModelName == CommanClass.DatabaseModelName.Drug)
                    {
                        Drug.CurrentDrug.DrugConnectionString = FullCurrentConnectionString;
                    }
                    if (_DatabaseModelName == CommanClass.DatabaseModelName.OpRoom)
                    {
                        if (_IsCurrect)
                            OpRoom.CurrentOpRoom.OpRoomConnectionString = FullCurrentConnectionString;
                        else
                        {
                            OpRoom.OpRoomDatabase totalsystemDatabase = new OpRoom.OpRoomDatabase(null);
                            totalsystemDatabase.ConnectionString = FullCurrentConnectionString;
                        }
                        //OpRoom.CurrentOpRoom.OpRoomConnectionString = FullCurrentConnectionString;
                    }

                    if (_DatabaseModelName == CommanClass.DatabaseModelName.Blood)
                    {
                        Blood.CurrentBlood.BloodConnectionString = FullCurrentConnectionString;
                    }

                    if (_DatabaseModelName == CommanClass.DatabaseModelName.Phisio)
                    {
                        Phisio.CurrentPhisio.PhisioConnectionString = FullCurrentConnectionString;
                    }

                    if (_DatabaseModelName == CommanClass.DatabaseModelName.Food)
                    {
                        Food.CurrentFood.FoodConnectionString = FullCurrentConnectionString;
                    }

                    if (_DatabaseModelName == CommanClass.DatabaseModelName.Den)
                    {
                        Den.CurrentDen.DenConnectionString = FullCurrentConnectionString;
                    }

                    if (_DatabaseModelName == CommanClass.DatabaseModelName.Tajhizat)
                    {
                        Tajhizat.CurrentTajhizat.TajhizatConnectionString = FullCurrentConnectionString;
                    }

                    if (_DatabaseModelName == CommanClass.DatabaseModelName.Tasisat)
                    {
                        Tasisat.CurrentTasisat.TasisatConnectionString = FullCurrentConnectionString;
                    }

                    if (_DatabaseModelName == CommanClass.DatabaseModelName.MDoc)
                    {
                        MDoc.CurrentMDoc.MDocConnectionString = FullCurrentConnectionString;
                    }

                    if (_DatabaseModelName == CommanClass.DatabaseModelName.Requests)
                    {
                        if (_IsCurrect)
                            Requests.CurrentRequests.RequestsConnectionString = FullCurrentConnectionString;
                        else
                        {
                            Requests.RequestsDatabase totalsystemDatabase = new Requests.RequestsDatabase(null);
                            totalsystemDatabase.ConnectionString = FullCurrentConnectionString;
                        }
                        //Radio.CurrentRadio.RadioConnectionString = FullCurrentConnectionString;
                        //Radio.CurrentRadio.DatabaseName = uiDatabaseCmb.Text;
                    }

                    if (_DatabaseModelName == "Amval")
                    {
                        Amval.CurrentAmval.AmvalConnectionString = FullCurrentConnectionString;
                    }

                    DialogResult = true;
                    MessageBox.Show(".با موفقیت ذخیره شد" + uiDatabaseCmb.Text + "دیتابیس");


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            
        }

        private void uiTestBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TestDataBaseConnection(null, true))
                {
                    _TestConnection = true;
                    SetDatabsesCmb();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

    

        private void uiDatabaseCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Title = uiDatabaseCmb.Text;
        }
    }
}
