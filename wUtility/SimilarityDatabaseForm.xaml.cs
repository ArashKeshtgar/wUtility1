using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using wUtility.Utils;
using static wUtility.Lab;
using static wUtility.Totalsystem;

namespace wUtility
{
    /// <summary>
    /// Interaction logic for SimilarityDatabaseForm.xaml
    /// </summary>
    public partial class SimilarityDatabaseForm : Window
    {
        public class ReprotUpdate
        {
            private string _script { get; set; } = "";
            public string Script
            {
                get
                {
                    return _script;
                }
                set
                {
                    _script = value;
                }
            }
            private string _description { get; set; } = "";
            public string Description
            {
                get
                {
                    return _description;
                }
                set
                {
                    _description = value;
                }
            }
            private string _columnName { get; set; } = "";
            public string ColumnName
            {
                get
                {
                    return _columnName;
                }
                set
                {
                    _columnName = value;
                }
            }
            private string _tableName { get; set; } = "";
            public string TableName
            {
                get
                {
                    return _tableName;
                }
                set
                {
                    _tableName = value;
                }
            }
        }

        static public List<ReprotUpdate> ReprotUpdatesList { get; set; } = new List<ReprotUpdate>();

        public DataTable DatabasesDataTable { get; private set; }

        //public List<CommanClass.DestnationDatabases.DestnationDatabase> DestenationDatabasesList { get;  set; }
        static public DataTable DatasourceDatabasesDt { get; private set; }
        static public DataTable DatasourceViewsDt { get; private set; }
        static public DataTable DatasourceSpsDt { get; private set; }
        static public DataTable DatasourceFunctionsDt { get; private set; }
        static public DataTable DatasourceIndexesDt { get; private set; }
        static public DataTable DestenationDatabasesDt { get; private set; }
        static public DataTable DestenationViewsDt { get; private set; }
        static public DataTable DestenationSpsDt { get; private set; }
        static public DataTable DestenationFunctionsDt { get; private set; }
        static public DataTable DestenationIndexesDt { get; private set; }
        private List<DataRow> _discordResultExecutedList { get;  set; } = new List<DataRow>();
        static public List<DataRow>DiscordResultExecutedList { get; private set; } = new List<DataRow>();
        static public List<DataRow> DiscordIndexesExecutedList { get; private set; } = new List<DataRow>();
        static public List<DataRow> DiscordResultList { get; set; }
        static public List<DataRow> DiscordDataTypeResultList { get; set; }
        static public List<DataRow> DiscordViewsResultList { get; set; }
        static public List<DataRow> DiscordSpsResultList { get; set; }
        static public List<DataRow> DiscordFunctionsResultList { get; set; }
        static public List<DataRow> DiscordIndexesResultList { get; set; }
        static public DataTable jointDT { get; set; }
        static public string DatasourceDatabseName { get; set; } = "";
        static public string DestenationDatabseName { get; set; } = "";
        static public string DestenationDatabseConnectionString { get; set; } = "";
        static public string DatasourceDatabseConnectionString { get; set; } = "";
        static public DataTable DiscardsColumnsDataTable { get; private set; }
        static public string ConnetDatabaseName = "";
        private string DatabseModelName = "";
        static public string ConnectedDatabsesName = "";
        static public bool IsDatasourceMode = true;
        public SimilarityDatabaseForm()
        {
            InitializeComponent();

            SetConnetedDatabasesControl();
            SetModelDatabasesControl();
            DiscordResultExecutedList = _discordResultExecutedList;
            //StyleManager.ApplicationTheme = new Office2016Theme();

          
            
        }

        private void SetConnetedDatabasesControl()
        {
            try
            {
                if (CommanClass.DatasourceDatabases.DatasoursesDatabasesList != null)
                {
                    //List<string> s = CommonClass.DatasourceDatabases.GetDatasourceDatabasesList();

                    uiDatasourceDatababasesCmb.ItemsSource = CommanClass.DatasourceDatabases.GetDatasourceDatabasesList();
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
                uiDatababasesModelCmb.ItemsSource = vs;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void FillDatabaseConnectedList(string connectionString,string databaseName)
        {
            
                //if(uiDatababaseConnected.Text == "TotalSystem")
                using (DBHelper dbh = new DBHelper(connectionString))
                {
                    DataTable table = new DataTable();
                DatabasesDataTable = dbh.ExecuteQuery(@"Select  TT.TABLE_CATALOG,TT.TABLE_NAME, TT.COLUMN_NAME,COLUMN_DEFAULT,IS_NULLABLE,
                                                        DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION,
                                                        NUMERIC_PRECISION_RADIX,COLLATION_NAME,	
                                                        case when Tab.Constraint_Type is not null  then 'کلید اصلی' else '' End As Constraint_Type,
                                                        Col.CONSTRAINT_NAME From " + databaseName + @".INFORMATION_SCHEMA.COLUMNS TT
	                                                    Inner Join INFORMATION_SCHEMA.TABLES T on TT.TABLE_NAME = T.TABLE_NAME
	                                                    LEFT join 
	                                                    INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col on 
	                                                    Col.COLUMN_NAME = TT.COLUMN_NAME
	                                                    and Col.TABLE_NAME = TT.TABLE_NAME
                                                        LEFT JOIN 
	                                                    INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab on
	                                                    Tab.TABLE_NAME = TT.TABLE_NAME 
                                                        And Tab.CONSTRAINT_NAME = Col.CONSTRAINT_NAME
                                                        Where T.TABLE_TYPE = 'BASE TABLE' ");
                uiDatasourceDatabaseGrid.ItemsSource = DatabasesDataTable;
                    //uiSelectedDatabase.ItemsSource = table;
                }
            
        }

        static private void FillDatabaseConnectedListStatic(string connectionString, string databaseName,bool isDestenationDatabase)
        {
            DataTable table = new DataTable();
            
            //if(uiDatababaseConnected.Text == "TotalSystem")
            using (DBHelper dbh = new DBHelper(connectionString))
            {
               
                                         table = dbh.ExecuteQuery(@" SELECT row_number() over (order by T.TABLE_CATALOG) RowNumber, T.TABLE_CATALOG,T.TABLE_NAME
		                                            ,T.COLUMN_NAME,T.COLUMN_DEFAULT,
		                                            case when T.IS_NULLABLE = 'YES' then CAST(1 as bit)
														 else CAST(0 as bit)End AS IS_NULLABLE,T.DATA_TYPE,
		                                            T.CHARACTER_MAXIMUM_LENGTH,
		                                            NUMERIC_PRECISION,NUMERIC_PRECISION_RADIX,
		                                            NUMERIC_SCALE,TT.TABLE_TYPE,T.ORDINAL_POSITION,case when CON.CONSTRAINT_TYPE = 'PRIMARY KEY' then CAST(1 as bit)
														 else CAST(0 as bit)End AS Constraint_Type
                                                    ,CON.CONSTRAINT_NAME,cast(COLUMNPROPERTY(object_id(T.TABLE_NAME), T.COLUMN_NAME, 'IsIdentity')as bit)AS IS_IDENTITY
                        FROM  " + databaseName + @".INFORMATION_SCHEMA.COLUMNS T	
		                                            inner join	" + databaseName + @".INFORMATION_SCHEMA.TABLES TT on TT.TABLE_NAME = t.TABLE_NAME
		                                            LEFT JOIN(SELECT Constraint_Type ,Tab.TABLE_CATALOG,Tab.TABLE_NAME,Col.COLUMN_NAME ,Tab.CONSTRAINT_NAME 
										                        from
					                                            " + databaseName + @".INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab, 
					                                            " + databaseName + @".INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col
			
										                        WHERE 
					                                            Col.Constraint_Name = Tab.Constraint_Name
					                                            AND Col.Table_Name = Tab.Table_Name
					                                            AND Constraint_Type = 'PRIMARY KEY')CON
					                                             ON CON.TABLE_CATALOG = T.TABLE_CATALOG
						                                            AND	CON.TABLE_NAME = T.TABLE_NAME
						                                            AND	CON.COLUMN_NAME = T.COLUMN_NAME	
                        where  TT.TABLE_TYPE = 'BASE TABLE'	 ");
                if (!isDestenationDatabase)
                    DatasourceDatabasesDt = table;
                else
                    DestenationDatabasesDt = table;

           
            }

        }

        private void uiDatababaseConnected_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void uiDatasourceDatabaseGrid_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void RadMenu_ItemClick()
        {

        }

        private void RadMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DatabasesForm form = new DatabasesForm();
            form.Show();
            SetConnetedDatabasesControl();
            SetModelDatabasesControl();
        }

        private void uiDatababasesModelCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (uiDestenationDatababases.ItemsSource != null)
                    uiDestenationDatababases.ItemsSource = null;
                DatabseModelName = uiDatababasesModelCmb.Text;
                switch (DatabseModelName)
                {
                    case "Totalsystem":
                        
                        //uiDestenationDatababases.ItemsSource = CommanClass.DestnationDatabases.DestnationDatabasesList;
                        FillDestenationDatabasesCmbFilterd(CurrentTotalsystem.CurrentTotalsystemDatabseModel);
                        FillDatasourceDatabasesCmbFilterd(CurrentTotalsystem.CurrentTotalsystemDatabseModel);
                        break;

                    case "Lab":
                        FillDestenationDatabasesCmbFilterd(CurrentLab.CurrentLabDatabseModel);
                        FillDatasourceDatabasesCmbFilterd(CurrentLab.CurrentLabDatabseModel);
                        break;

                    case "Radio":
                        FillDestenationDatabasesCmbFilterd(Radio.CurrentRadio.CurrentRadioDatabseModel);
                        FillDatasourceDatabasesCmbFilterd(Radio.CurrentRadio.CurrentRadioDatabseModel);
                        break;

                    case "OpRoom":
                        FillDestenationDatabasesCmbFilterd(OpRoom.CurrentOpRoom.CurrentOpRoomDatabseModel);
                        FillDatasourceDatabasesCmbFilterd(OpRoom.CurrentOpRoom.CurrentOpRoomDatabseModel);
                        break;

                    case "Requests":
                        FillDestenationDatabasesCmbFilterd(Requests.CurrentRequests.CurrentRequestsDatabseModel);
                        FillDatasourceDatabasesCmbFilterd(Requests.CurrentRequests.CurrentRequestsDatabseModel);
                        break;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public void FillDestenationDatabasesCmbFilterd(string databaseModel)
        {
            CommanClass.DestnationDatabases.DestnationDatabasesList = CommanClass.DestnationDatabases.GetDestnationDatabasesList(databaseModel);

            CommanClass.DestnationDatabases.DestnationDatabasesList = CommanClass.DestnationDatabases.DestnationDatabasesList.Where(o =>
                                                                        o.DestnationDatabaseModel == databaseModel).ToList();

            if (CommanClass.DestnationDatabases.DestnationDatabasesList.Count > 0)
                uiDestenationDatababases.ItemsSource = CommanClass.DestnationDatabases.DestnationDatabasesList;
        }

        public void FillDatasourceDatabasesCmbFilterd(string databaseModel)
        {
            CommanClass.DatasourceDatabases.DatasoursesDatabasesList = CommanClass.DatasourceDatabases.GetDatasourceDatabasesList();

            CommanClass.DatasourceDatabases.DatasoursesDatabasesList = CommanClass.DatasourceDatabases.DatasoursesDatabasesList.Where(o =>
                                                                        o.DatasourceDatabaseModel == databaseModel).ToList();

            if (CommanClass.DatasourceDatabases.DatasoursesDatabasesList.Count > 0)
                uiDatasourceDatababasesCmb.ItemsSource = CommanClass.DatasourceDatabases.DatasoursesDatabasesList;
        }

        static async Task GetDirectoryAsync(string source, string databaseName,bool Is2thdatabase)
        {
            //IEnumerable<DirectoryInfo> infos = null;
            //XCopyForm xCopyForm = new XCopyForm();

            await Task.Run(() => {
                //backgroundWorker.RunWorkerAsync();
                //eventWaitHandle.WaitOne();
                //backgroundWorker.RunWorkerAsync();
                Thread.Sleep(1000);
                FillDatabaseConnectedListStatic(source, databaseName, Is2thdatabase);
                
                //caztonCountdown.Signal();
            });
        }

        static async Task ShowProgressbarValueAsync(int counter, decimal calcprogresPersent, RadProgressBar uiProgressBar,Telerik.Windows.Controls.Label uiDiscordCountLbl,int count)
        {
            //IEnumerable<DirectoryInfo> infos = null;
            //XCopyForm xCopyForm = new XCopyForm();

            await Task.Run(() => {
                //backgroundWorker.RunWorkerAsync();
                //eventWaitHandle.WaitOne();
                //backgroundWorker.RunWorkerAsync();
                Thread.Sleep(1000);
                //FillDatabaseConnectedListStatic(source, databaseName, Is2thdatabase);
                uiProgressBar.Visibility = Visibility.Visible;
                uiProgressBar.Value = (double)(counter * calcprogresPersent);
                uiDiscordCountLbl.Content = counter.ToString() + "/" + count.ToString();
                //caztonCountdown.Signal();
            });
        }

        private void uiSelectedDatabase_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            
        }

        private void uiDatababases2th_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DestenationDatabseName = uiDestenationDatababases.Text;
                var databaseModelNamelist = CommanClass.DestnationDatabases.DestnationDatabasesList.Where(o => o.DestnationDatabaseModel == uiDatababasesModelCmb.Text)
                                                                                    .Select(os => os.DestnationDatabaseModel).ToList();
                var databaseModelName = "";// databaseModelNamelist[0].ToString();
                //ConnectedDatabsesName = uiDatababaseConnected.Text;
                var connectionStringlist = new List<string>();//Totalsystem.CurrentTotalsystem.TotalsystemDatabasesList.Where(o => o.DatabaseName == uiDestenationDatababases.Text)
                                                              //.Select(os =>  os.ConnectionStringRead ).ToList();
                var connectionString = "";//connectionStringlist[0].ToString();
               

                switch (DatabseModelName)
                {
                    case "Totalsystem":
                        if (databaseModelNamelist[0].ToString() == CommanClass.DatabaseModelName.Totalsystem)
                        {
                             databaseModelNamelist = CommanClass.DestnationDatabases.DestnationDatabasesList.Where(o => o.DestnationDatabaseModel == uiDatababasesModelCmb.Text)
                                                                                   .Select(os => os.DestnationDatabaseModel).ToList();
                             databaseModelName = databaseModelNamelist[0].ToString();
                            //ConnectedDatabsesName = uiDatababaseConnected.Text;
                             connectionStringlist = Totalsystem.CurrentTotalsystem.TotalsystemDatabasesList.Where(o => o.DatabaseName == uiDestenationDatababases.Text)
                                                                                                .Select(os => os.ConnectionStringRead).ToList();
                             connectionString = connectionStringlist[0].ToString();
                            //FillDatabaseConnectedList(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString, uiDatababaseConnected.Text);

                            Task task = Task.Run(async () => await GetDirectoryAsync(connectionString, DestenationDatabseName, true));
                            Task.WaitAll(task);
                            DataTable dataTable = DestenationDatabasesDt;
                            uiDestenationDatabaseGrid.ItemsSource = dataTable;
                            DestenationDatabseConnectionString = connectionString;
                        }
                        break;
                    case "Lab":
                        if (databaseModelName == CommanClass.DatabaseModelName.Lab)
                            FillDatabaseConnectedList(Lab.CurrentLab.LabConnectionString, uiDestenationDatababases.Text);
                        break;
                    case "Radio":
                        if (databaseModelNamelist[0].ToString() == CommanClass.DatabaseModelName.Radio)
                        {
                             databaseModelName = databaseModelNamelist[0].ToString();
                            //ConnectedDatabsesName = uiDatababaseConnected.Text;
                             connectionStringlist = Radio.CurrentRadio.RadioDatabasesList.Where(o => o.DatabaseName == uiDestenationDatababases.Text)
                                                                                                .Select(os => os.ConnectionStringRead).ToList();
                             connectionString = connectionStringlist[0].ToString();
                            DestenationDatabseConnectionString = connectionString;
                            //FillDatabaseConnectedList(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString, uiDatababaseConnected.Text);

                            Task task = Task.Run(async () => await GetDirectoryAsync(connectionString, DestenationDatabseName, true));
                            Task.WaitAll(task);
                            DataTable dataTable = DestenationDatabasesDt;
                            uiDestenationDatabaseGrid.ItemsSource = dataTable;
                            DestenationDatabseConnectionString = connectionString;
                        }
                        //if (databaseModelName == CommanClass.DatabaseModelName.Radio)
                        //    FillDatabaseConnectedList(Radio.CurrentRadio.RadioConnectionString, uiDestenationDatababases.Text);
                        break;
                    case "Drug":
                        if (databaseModelName == CommanClass.DatabaseModelName.Drug)
                            FillDatabaseConnectedList(Drug.CurrentDrug.DrugConnectionString, uiDestenationDatababases.Text);
                        break;
                    case "Blood":
                        if (databaseModelName == CommanClass.DatabaseModelName.Blood)
                            FillDatabaseConnectedList(Blood.CurrentBlood.BloodConnectionString, uiDestenationDatababases.Text);
                        break;
                    case "OpRoom":
                        if (databaseModelNamelist[0].ToString() == CommanClass.DatabaseModelName.OpRoom)
                        {
                            databaseModelNamelist = CommanClass.DestnationDatabases.DestnationDatabasesList.Where(o => o.DestnationDatabaseModel == uiDatababasesModelCmb.Text)
                                                                                  .Select(os => os.DestnationDatabaseModel).ToList();
                            databaseModelName = databaseModelNamelist[0].ToString();
                            //ConnectedDatabsesName = uiDatababaseConnected.Text;
                            connectionStringlist = OpRoom.CurrentOpRoom.OpRoomDatabasesList.Where(o => o.DatabaseName == uiDestenationDatababases.Text)
                                                                                               .Select(os => os.ConnectionStringRead).ToList();
                            connectionString = connectionStringlist[0].ToString();
                            //FillDatabaseConnectedList(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString, uiDatababaseConnected.Text);

                            Task task = Task.Run(async () => await GetDirectoryAsync(connectionString, DestenationDatabseName, true));
                            Task.WaitAll(task);
                            DataTable dataTable = DestenationDatabasesDt;
                            uiDestenationDatabaseGrid.ItemsSource = dataTable;
                            DestenationDatabseConnectionString = connectionString;
                        }
                        //if (databaseModelName == CommanClass.DatabaseModelName.OpRoom)
                        //    FillDatabaseConnectedList(OpRoom.CurrentOpRoom.OpRoomConnectionString, uiDestenationDatababases.Text);
                        break;
                    case "Phisio":
                        if (databaseModelName == CommanClass.DatabaseModelName.Phisio)
                            FillDatabaseConnectedList(Phisio.CurrentPhisio.PhisioConnectionString, uiDestenationDatababases.Text);
                        break;
                    case "Den":
                        if (databaseModelName == CommanClass.DatabaseModelName.Den)
                            FillDatabaseConnectedList(Den.CurrentDen.DenConnectionString, uiDestenationDatababases.Text);
                        break;
                    case "Food":
                        if (databaseModelName == CommanClass.DatabaseModelName.Food)
                            FillDatabaseConnectedList(Food.CurrentFood.FoodConnectionString, uiDestenationDatababases.Text);
                        break;
                    case "Tajhizat":
                        if (databaseModelName == CommanClass.DatabaseModelName.Tajhizat)
                            FillDatabaseConnectedList(Tajhizat.CurrentTajhizat.TajhizatConnectionString, uiDestenationDatababases.Text);
                        break;
                    case "Tasisat":
                        if (databaseModelName == CommanClass.DatabaseModelName.Tasisat)
                            FillDatabaseConnectedList(Tasisat.CurrentTasisat.TasisatConnectionString, uiDestenationDatababases.Text);
                        break;
                    case "Requests":
                        if (databaseModelNamelist[0].ToString() == CommanClass.DatabaseModelName.Requests)
                        {
                            databaseModelName = databaseModelNamelist[0].ToString();
                            //ConnectedDatabsesName = uiDatababaseConnected.Text;
                            connectionStringlist = Requests.CurrentRequests.RequestsDatabasesList.Where(o => o.DatabaseName == uiDestenationDatababases.Text)
                                                                                               .Select(os => os.ConnectionStringRead).ToList();
                            connectionString = connectionStringlist[0].ToString();
                            DestenationDatabseConnectionString = connectionString;
                            //FillDatabaseConnectedList(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString, uiDatababaseConnected.Text);

                            Task task = Task.Run(async () => await GetDirectoryAsync(connectionString, DestenationDatabseName, true));
                            Task.WaitAll(task);
                            DataTable dataTable = DestenationDatabasesDt;
                            uiDestenationDatabaseGrid.ItemsSource = dataTable;
                            DestenationDatabseConnectionString = connectionString;
                        }
                        break;
                }
                //if (DatabseModelName == "Totalsystem" )
                //{
                //   FillDatabaseConnectedList(TotalSystem.CurrentTotalsystem.TotalsystemConnectionString, uiDatababaseConnected.Text);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiDatasourceDatababasesCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DatasourceDatabseName = uiDatasourceDatababasesCmb.Text;
                //ConnetDatabaseName = uiDatababaseConnected.Text;
                //ConnectedDatabsesName = uiDatababaseConnected.Text;
                string s = ConnectedDatabsesName;

                switch (DatabseModelName)
                {
                    case "Totalsystem":
                        if (CurrentTotalsystem.CurrentTotalsystemDatabseModel == CommanClass.DatabaseModelName.Totalsystem)
                        {
                            //FillDatabaseConnectedList(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString, uiDatababaseConnected.Text);

                            Task task = Task.Run(async () => await GetDirectoryAsync(CurrentTotalsystem.TotalsystemConnectionString, ConnetDatabaseName, false));
                            Task.WaitAll(task);
                            DataTable dataTable = DatasourceDatabasesDt;
                            uiDatasourceDatabaseGrid.ItemsSource = dataTable;
                            DatasourceDatabseConnectionString = CurrentTotalsystem.TotalsystemConnectionString;
                        }
                        break;
                    case "Lab":
                        if (Lab.CurrentLab.CurrentLabDatabseModel == CommanClass.DatabaseModelName.Lab)
                            FillDatabaseConnectedList(Lab.CurrentLab.LabConnectionString, uiDatasourceDatababasesCmb.Text);
                        break;
                    case "Radio":
                        if (Radio.CurrentRadio.CurrentRadioDatabseModel == CommanClass.DatabaseModelName.Radio)
                        {
                            //FillDatabaseConnectedList(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString, uiDatababaseConnected.Text);

                            Task task = Task.Run(async () => await GetDirectoryAsync(Radio.CurrentRadio.RadioConnectionString, ConnetDatabaseName, false));
                            Task.WaitAll(task);
                            DataTable dataTable = DatasourceDatabasesDt;
                            uiDatasourceDatabaseGrid.ItemsSource = dataTable;
                            DatasourceDatabseConnectionString = Radio.CurrentRadio.RadioConnectionString;
                        }
                        //if (Radio.CurrentRadio.CurrentRadioDatabseModel == CommanClass.DatabaseModelName.Radio)
                        //    FillDatabaseConnectedList(Radio.CurrentRadio.RadioConnectionString, uiDatasourceDatababasesCmb.Text);
                        break;
                    case "Drug":
                        if (Drug.CurrentDrug.CurrentDrugDatabseModel == CommanClass.DatabaseModelName.Drug)
                            FillDatabaseConnectedList(Drug.CurrentDrug.DrugConnectionString, uiDatasourceDatababasesCmb.Text);
                        break;
                    case "Blood":
                        if (Blood.CurrentBlood.CurrentBloodDatabseModel == CommanClass.DatabaseModelName.Blood)
                            FillDatabaseConnectedList(Blood.CurrentBlood.BloodConnectionString, uiDatasourceDatababasesCmb.Text);
                        break;
                    case "OpRoom":
                        if (OpRoom.CurrentOpRoom.CurrentOpRoomDatabseModel == CommanClass.DatabaseModelName.OpRoom)
                        {
                            //FillDatabaseConnectedList(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString, uiDatababaseConnected.Text);

                            Task task = Task.Run(async () => await GetDirectoryAsync(OpRoom.CurrentOpRoom.OpRoomConnectionString, ConnetDatabaseName, false));
                            Task.WaitAll(task);
                            DataTable dataTable = DatasourceDatabasesDt;
                            uiDatasourceDatabaseGrid.ItemsSource = dataTable;
                            DatasourceDatabseConnectionString = OpRoom.CurrentOpRoom.OpRoomConnectionString;
                        }
                        //if (OpRoom.CurrentOpRoom.CurrentOpRoomDatabseModel == CommanClass.DatabaseModelName.OpRoom)
                        //    FillDatabaseConnectedList(OpRoom.CurrentOpRoom.OpRoomConnectionString, uiDatasourceDatababasesCmb.Text);
                        break;
                    case "Phisio":
                        if (Phisio.CurrentPhisio.CurrentPhisioDatabseModel == CommanClass.DatabaseModelName.Phisio)
                            FillDatabaseConnectedList(Phisio.CurrentPhisio.PhisioConnectionString, uiDatasourceDatababasesCmb.Text);
                        break;
                    case "Den":
                        if (Den.CurrentDen.CurrentDenDatabseModel == CommanClass.DatabaseModelName.Den)
                            FillDatabaseConnectedList(Den.CurrentDen.DenConnectionString, uiDatasourceDatababasesCmb.Text);
                        break;
                    case "Food":
                        if (Food.CurrentFood.CurrentFoodDatabseModel == CommanClass.DatabaseModelName.Food)
                            FillDatabaseConnectedList(Food.CurrentFood.FoodConnectionString, uiDatasourceDatababasesCmb.Text);
                        break;
                    case "Tajhizat":
                        if (Tajhizat.CurrentTajhizat.CurrentTajhizatDatabseModel == CommanClass.DatabaseModelName.Tajhizat)
                            FillDatabaseConnectedList(Tajhizat.CurrentTajhizat.TajhizatConnectionString, uiDatasourceDatababasesCmb.Text);
                        break;
                    case "Tasisat":
                        if (Tasisat.CurrentTasisat.CurrentTasisatDatabseModel == CommanClass.DatabaseModelName.Tasisat)
                            FillDatabaseConnectedList(Tasisat.CurrentTasisat.TasisatConnectionString, uiDatasourceDatababasesCmb.Text);
                        break;
                    case "Requests":
                        if (Requests.CurrentRequests.CurrentRequestsDatabseModel == CommanClass.DatabaseModelName.Requests)
                        {
                            //FillDatabaseConnectedList(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString, uiDatababaseConnected.Text);

                            Task task = Task.Run(async () => await GetDirectoryAsync(Requests.CurrentRequests.RequestsConnectionString, ConnetDatabaseName, false));
                            Task.WaitAll(task);
                            DataTable dataTable = DatasourceDatabasesDt;
                            uiDatasourceDatabaseGrid.ItemsSource = dataTable;
                            DatasourceDatabseConnectionString = Requests.CurrentRequests.RequestsConnectionString;
                        }
                        break;
                }
                //if (DatabseModelName == "Totalsystem" )
                //{
                //   FillDatabaseConnectedList(TotalSystem.CurrentTotalsystem.TotalsystemConnectionString, uiDatababaseConnected.Text);
                //}DifferenceInvert
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

       

        public void FillViewsTables()
        {
            string connectionString = "";
            if (IsDatasourceMode)
                connectionString = DatasourceDatabseConnectionString;
            else
                connectionString = DestenationDatabseConnectionString;
            
            using (DBHelper dbh = new DBHelper(connectionString))
            {
                string query = @"SELECT 
                                ROW_NUMBER() OVER(ORDER BY TABLE_CATALOG) RowNumber,TABLE_CATALOG,TABLE_SCHEMA,TABLE_NAME,VIEW_DEFINITION
                                FROM INFORMATION_SCHEMA.VIEWS";
                DataTable dataTable = dbh.ExecuteQuery(query);

                if (IsDatasourceMode)
                    DatasourceViewsDt = dataTable;
                else
                    DestenationViewsDt = dataTable;
            }
        }

        public void FillSpsTables()
        {
            string connectionString = "";
            if (IsDatasourceMode)
                connectionString = DatasourceDatabseConnectionString;
            else
                connectionString = DestenationDatabseConnectionString;

            using (DBHelper dbh = new DBHelper(connectionString))
            {
                string query = @"SELECT ROW_NUMBER() OVER(ORDER BY ROUTINE_CATALOG) RowNumber, ROUTINE_CATALOG, ROUTINE_NAME, convert(nvarchar(max),object_definition(o.object_id))AS ROUTINE_DEFINITION
                                FROM INFORMATION_SCHEMA.ROUTINES i
                                inner join sys.procedures o on o.name = i.ROUTINE_NAME
                                WHERE ROUTINE_TYPE='PROCEDURE'";
                DataTable dataTable = dbh.ExecuteQuery(query);

                if (IsDatasourceMode)
                    DatasourceSpsDt = dataTable;
                else
                    DestenationSpsDt = dataTable;
            }
        }

        public void FillFunctionsTables()
        {
            string connectionString = "";
            if (IsDatasourceMode)
                connectionString = DatasourceDatabseConnectionString;
            else
                connectionString = DestenationDatabseConnectionString;

            using (DBHelper dbh = new DBHelper(connectionString))
            {
                string query = @"SELECT ROW_NUMBER() over(order by type_desc) RowNumber,name, definition, type_desc 
                                FROM sys.sql_modules m 
                                    INNER JOIN sys.objects o 
                                    ON m.object_id=o.object_id
                                WHERE RIGHT(type_desc, 8) = 'FUNCTION'";
                DataTable dataTable = dbh.ExecuteQuery(query);

                if (IsDatasourceMode)
                    DatasourceFunctionsDt = dataTable;
                else
                    DestenationFunctionsDt = dataTable;
            }
        }

        public void FillIndexsesTables()
        {
            string connectionString = "";
            if (IsDatasourceMode)
                connectionString = DatasourceDatabseConnectionString;
            else
                connectionString = DestenationDatabseConnectionString;

            using (DBHelper dbh = new DBHelper(connectionString))
            {
                string query = @"SELECT 
                                ROW_NUMBER() over(order by DB_NAME()) RowNumber,
                                DB_NAME() AS Database_Name,
                                     TableName = t.name,
                                     IndexName = ind.name,
                                     IndexId = ind.index_id,
                                     ColumnId = ic.index_column_id,
                                     ColumnName = col.name
                                FROM 
                                     sys.indexes ind 
                                INNER JOIN 
                                     sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
                                INNER JOIN 
                                     sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
                                INNER JOIN 
                                     sys.tables t ON ind.object_id = t.object_id 
                                WHERE 
                                     ind.is_primary_key = 0 
                                     AND ind.is_unique = 0 
                                     AND ind.is_unique_constraint = 0 
                                     AND t.is_ms_shipped = 0 
                                ORDER BY 
                                     t.name, ind.name, ind.index_id";

                DataTable dataTable = dbh.ExecuteQuery(query);

                if (IsDatasourceMode)
                    DatasourceIndexesDt = dataTable;
                else
                    DestenationIndexesDt = dataTable;
            }
        }

        private void RadMenuItem_Click_1(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {

        }

        private void uiDifferenceMenuItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                IsDatasourceMode = true;
                DataTable dataTable = DatasourceDatabasesDt;
                DataTable dataTable1 = DestenationDatabasesDt;

                List<DataRow> discordResult = GetResultJoin(true, false);

                DiscordResultList = discordResult;
                uiDiscordCountLbl.Content = discordResult.Count.ToString();
                uiDiscordDatabasesGrid.ItemsSource = discordResult;
                FillViewsTables();
                FillSpsTables();
                FillFunctionsTables();
                FillIndexsesTables();

                uiDatasourceViewsGrid.ItemsSource = DatasourceViewsDt;
                uiDatasourceSpsGrid.ItemsSource = DatasourceSpsDt;
                uiDatasourceFunctionsGrid.ItemsSource = DatasourceFunctionsDt;
                uiDatasourceIndexesGrid.ItemsSource = DatasourceIndexesDt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message);
            }
        }
        private void uiDDiffrenceItemMenu_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                IsDatasourceMode = false;
                DataTable dataTable = DatasourceDatabasesDt;
                DataTable dataTable1 = DestenationDatabasesDt;
                List<DataRow> discordResult = GetResultJoin(false, false);

               
                DiscordResultList = discordResult;
                uiDiscordCountLbl.Content = discordResult.Count.ToString();
                uiDiscordDatabasesGrid.ItemsSource = discordResult;

                FillViewsTables();
                FillSpsTables();
                FillFunctionsTables();
                FillIndexsesTables();

                uiDestenationViewsGrid.ItemsSource = DestenationViewsDt;
                uiDestenationSpsGrid.ItemsSource = DestenationSpsDt;
                uiDestenationFunctionsGrid.ItemsSource = DestenationFunctionsDt;
                uiDestenationIndexesGrid.ItemsSource = DestenationIndexesDt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message);
            }
        }
        public void CreateScripts(List<DataRow> discordListRow,string connectionString,bool isDatasourceDatabase)
        {
            using (DBHelper dbh = new DBHelper(connectionString))
            {
                decimal calcprogresPersent = 100m/(discordListRow.Count);
                int counter = 0;
                DataTable dataTable = new DataTable();
                if (isDatasourceDatabase)
                    dataTable = DatasourceDatabasesDt;
                else
                    dataTable = DestenationDatabasesDt;

                foreach (var item in discordListRow)
                {
                    ReprotUpdate reprotUpdate = new ReprotUpdate();
                    string tableName = item["TABLE_NAME"].ToString();
                    string createOrAlter = "ALTER";

                    string columnName = "";
                    if (!string.IsNullOrEmpty(item["COLUMN_NAME"].ToString()))
                         columnName = "[" + item["COLUMN_NAME"].ToString() + "]";

                    string dataType = item["DATA_TYPE"].ToString();
                    string characterMaximumLength = item["CHARACTER_MAXIMUM_LENGTH"].ToString();
                    string nUMERIC_PRECISION = item["NUMERIC_PRECISION"].ToString();
                    string nUMERIC_PRECISION_RADIX = item["NUMERIC_PRECISION_RADIX"].ToString();
                    string nUMERIC_SCALE = item["NUMERIC_SCALE"].ToString();

                    string cONSTRAINT_NAME = "";
                    if (!string.IsNullOrEmpty(item["CONSTRAINT_NAME"].ToString()))
                         cONSTRAINT_NAME = "[" + item["CONSTRAINT_NAME"].ToString() + "]";

                    string cOLUMN_DEFAULT =  item["COLUMN_DEFAULT"].ToString();
                    bool iS_IDENTITY = (bool)item["IS_IDENTITY"];
                    bool iS_NULLABLE = (bool)item["IS_NULLABLE"];
                    string nOT_NULLExplanation =    "NOT NULL"; 
                    bool constraint_Type = (bool)item["Constraint_Type"];
                    string constraintQuery = "";
                    string script = "TABLE";
                    string dropConstraintQuery = "";
                    string bracesExplanation = "";
                    string addExplanation = " ADD ";
                    bool hasIdentityTable = false;
                    bool hasPrimaryKey = false;
                    var resultAny = dataTable.AsEnumerable().Where(o => o["TABLE_NAME"].ToString().ToUpper() == tableName.ToUpper()).Any();
                    hasIdentityTable = dataTable.AsEnumerable().Where(o => (bool) o["IS_IDENTITY"] == true).Any();

                    if (!resultAny)
                    {
                        var tableNameExistDiscord = DiscordResultExecutedList.Where(o => o["TABLE_NAME"].ToString().ToUpper() == tableName.ToUpper()).Any();
                        if (!tableNameExistDiscord)
                        {
                            createOrAlter = "CREATE";
                            bracesExplanation = " ( ";
                            addExplanation = "";
                        }
                    }
                    script = createOrAlter + ' ' + script + ' ' + tableName + bracesExplanation + addExplanation + columnName;
                    if (dataType == "char" || dataType == "varchar" || dataType == "nvarchar" || dataType == "nchar")
                    {
                        if (characterMaximumLength == (-1).ToString())
                            characterMaximumLength = "max";
                        script = script + ' ' + dataType + ' ' + "(" + characterMaximumLength + ")" + ' ';
                        cOLUMN_DEFAULT = "";
                    }
                    else if (dataType == "decimal" || dataType == "numeric")
                    {
                        script = script+ ' ' + dataType + ' ' + "(" + nUMERIC_PRECISION + ", " + nUMERIC_SCALE + ")" + ' ';
                        cOLUMN_DEFAULT = 0.ToString();
                    }
                    else if (dataType == "datetime" || dataType == "datetime2")
                    {
                        script = script + ' ' + dataType +' ';// + "(" + nUMERIC_PRECISION + ", " + nUMERIC_SCALE + ")" + ' ';
                        cOLUMN_DEFAULT = "GetDate()";
                    }
                    else if(dataType == "varbinary" || dataType == "text")
                    {
                        script = script + ' ' + dataType + ' ';
                    }
                    else
                    {
                        script = script + ' ' + dataType;
                        cOLUMN_DEFAULT = 0.ToString();
                    }


                    if (!iS_NULLABLE)
                    {
                        script = script + ' ' + nOT_NULLExplanation;
                        
                    }
                    //else
                    if (iS_IDENTITY)
                        script = script + ' ' + "IDENTITY(1,1)" + ' ';
                    else if (dataType != "varbinary" && dataType != "image" && !constraint_Type)
                    {
                        if(dataType == "char" || dataType == "varchar" || dataType == "nvarchar" || dataType == "nchar")
                         script = script + " DEFAULT " + "'" + cOLUMN_DEFAULT + "'";
                        else
                            script = script + " DEFAULT " + cOLUMN_DEFAULT ;
                    }
                    //script = script + ' ' + iS_NULLABLE;

                    var isConstraintName = DiscordResultExecutedList.Where(o => o["CONSTRAINT_NAME"].ToString() == item["CONSTRAINT_NAME"].ToString()).Any();
                    var resultAnyCONSTRAINT_NAME = dataTable.AsEnumerable().Where(o => o["CONSTRAINT_NAME"].ToString() == item["CONSTRAINT_NAME"].ToString()).Any();

                    if (!isConstraintName && !string.IsNullOrEmpty(cONSTRAINT_NAME))
                    {
                        //if(createOrAlter == "CREATE")
                        //var resultAnyCONSTRAINT_NAME = dataTable.AsEnumerable().Where(o => o["CONSTRAINT_NAME"].ToString() == item["CONSTRAINT_NAME"].ToString()).Any();
                        if(!resultAnyCONSTRAINT_NAME)
                         constraintQuery = "CONSTRAINT " + cONSTRAINT_NAME + " PRIMARY KEY (" + columnName + ")";


                    //script = script + "PRIMARY KEY" + " ";CONSTRAINT PK_Person PRIMARY KEY (ID,LastName)
                        
                    }

                     if((isConstraintName || resultAnyCONSTRAINT_NAME) && !string.IsNullOrEmpty(cONSTRAINT_NAME))
                    {
                        dropConstraintQuery = @"ALTER TABLE " + tableName + @" DROP CONSTRAINT " + cONSTRAINT_NAME;

                        var constraintItems = DiscordResultExecutedList.Where(o => o["CONSTRAINT_NAME"].ToString() == cONSTRAINT_NAME);
                        var constraintItemsInDB = dataTable.AsEnumerable().Where(o => o["CONSTRAINT_NAME"].ToString() == item["CONSTRAINT_NAME"].ToString());

                        constraintQuery = @"ALTER TABLE " + tableName + @" ADD CONSTRAINT "+ ' '+cONSTRAINT_NAME+' '+ @" PRIMARY KEY (";

                        if (constraintItems.Count() > 0)
                        {
                            foreach (DataRow itemConstrant in constraintItems)
                            {
                                constraintQuery = constraintQuery + itemConstrant["COLUMN_NAME"].ToString() + ",";
                            }

                        }

                        else if (constraintItemsInDB.Count() > 0)
                        {
                            constraintQuery =@" CONSTRAINT "+ ' '+cONSTRAINT_NAME+' '+ @" PRIMARY KEY(";

                            foreach (DataRow itemConstrantDb in constraintItemsInDB)
                            {
                                constraintQuery = constraintQuery + itemConstrantDb["COLUMN_NAME"].ToString() + ",";
                            }
                        }
                        constraintQuery = constraintQuery + ' ' + columnName;

                        constraintQuery = constraintQuery + ")";
                        //constraintQuery =  
                        //ALTER TABLE Persons
                        //DROP CONSTRAINT PK_Person;
                    }
                    else if (!string.IsNullOrEmpty(cONSTRAINT_NAME))
                    {
                        hasPrimaryKey = dataTable.AsEnumerable().Where(o => o["Constraint_Type"].ToString() == item["Constraint_Type"].ToString()
                                                                              && o["TABLE_NAME"].ToString() == item["TABLE_NAME"].ToString()).Any();
                        if (hasPrimaryKey)
                        {
                            var primaryKeyItems = dataTable.AsEnumerable().Where(o => o["Constraint_Type"].ToString() == item["Constraint_Type"].ToString()
                                                                               && o["TABLE_NAME"].ToString() == item["TABLE_NAME"].ToString()).ToList();
                            string consNameItem = primaryKeyItems[0]["CONSTRAINT_NAME"].ToString();

                            dropConstraintQuery = @"ALTER TABLE " + tableName + @" DROP CONSTRAINT " + consNameItem;
                            constraintQuery = @"ALTER TABLE " + tableName + @" ADD CONSTRAINT " + ' ' + cONSTRAINT_NAME + ' ' + @" PRIMARY KEY (";
                            if (primaryKeyItems.Count() > 0)
                            {
                                foreach (DataRow itemConstrant in primaryKeyItems)
                                {
                                    constraintQuery = constraintQuery + itemConstrant["COLUMN_NAME"].ToString() + ",";
                                }

                            }
                            constraintQuery = constraintQuery + ' ' + columnName;

                            constraintQuery = constraintQuery + ")";
                        }
                    }

                    if (createOrAlter == "CREATE")
                    {
                        bracesExplanation = ")";
                        addExplanation = "";
                        if(!string.IsNullOrEmpty(constraintQuery))
                            script = script + ", " + constraintQuery + bracesExplanation;
                        else
                            script = script + bracesExplanation;


                        dbh.ExecuteQuery(script);
                    }
                    else
                    {
                        
                        bracesExplanation = "";
                        addExplanation = "";
                        script = script + addExplanation+ ' ' + constraintQuery;
                        if(((isConstraintName || resultAnyCONSTRAINT_NAME) && !string.IsNullOrEmpty(constraintQuery)) || hasPrimaryKey) 
                            dbh.ExecuteQuery(dropConstraintQuery);
                        dbh.ExecuteQuery(script);
                    }
                    reprotUpdate.ColumnName = columnName;
                    reprotUpdate.TableName = tableName;
                    reprotUpdate.Script = script;
                    reprotUpdate.Description =   "اضافه شد "  + ' ' + tableName + ' ' + "به جدول " + ' '  + columnName + ' ' + "ستون";
                    ReprotUpdatesList.Add(reprotUpdate);
                    //item.Table.Columns.Add("Script");
                    //item["Script"] = script;
                    DiscordResultExecutedList.Add(item);
                    //DiscordResultExecutedList.Add(item);
                    if (uiProgressBar.Value == 0)
                    {
                        uiProgressBar.Value = 1;
                        counter = 1;
                    }
                    //double calcvalueprogress = uiProgressBar.Value * calcprogresPersent;
                    if (counter <= discordListRow.Count)
                    {
                        //    //calcprogresPersent = counter / discordListRow.Count;
                        //    //Task task = Task.Run(async () => await ShowProgressbarValueAsync(counter, calcprogresPersent, uiProgressBar, uiDiscordCountLbl, discordListRow.Count));
                        //    //Task.WaitAll(task);
                        //uiProgressBar.Visibility = Visibility.Visible;
                        //uiProgressBar.Value = (double)(counter * calcprogresPersent);
                        uiDiscordCountLbl.Content = counter.ToString() + "/" + discordListRow.Count.ToString();
                        //uiProgressBar.Refresh();

                        //Invoke(new MethodInvoker(delegate
                        //{
                        //    textBox1.Text = (int.Parse(textBox1.Text) + 1).ToString();
                        //    textBox1.Refresh();
                        //    progressBar1.Value += 10;
                        //    progressBar1.Refresh();

                        //}));

                        counter = counter + 1;
                        //}
                        //else
                        //{
                        //    //timer1.Stop();
                        //    uiProgressBar.Visibility = Visibility.Hidden;
                    }

            }
            }
        }

        public void ChngeDataType(List<DataRow> discordListRow, string connectionString)
        {
            using (DBHelper dbh = new DBHelper(connectionString))
            {
                foreach (var item in discordListRow)
                {
                    string script = " ALTER TABLE ";
                    string columnName = "[" + item["COLUMN_NAME"].ToString() + "]";
                    string tableName = item["TABLE_NAME"].ToString();
                    string dataType = item["DATA_TYPE"].ToString();
                    string characterMaximumLength = item["CHARACTER_MAXIMUM_LENGTH"].ToString();
                    string nUMERIC_PRECISION = item["NUMERIC_PRECISION"].ToString();
                    string nUMERIC_PRECISION_RADIX = item["NUMERIC_PRECISION_RADIX"].ToString();
                    string nUMERIC_SCALE = item["NUMERIC_SCALE"].ToString();
                    string cOLUMN_DEFAULT = item["COLUMN_DEFAULT"].ToString();
                    string dropConstraint = "";
                    string createConstraint = "";
                    

                    string findConstraint = @"SELECT 
                                                TableName = t.Name,
                                                ColumnName = c.Name,
                                                dc.Name DCName,
                                                dc.definition
                                            FROM sys.tables t
                                            INNER JOIN sys.default_constraints dc ON t.object_id = dc.parent_object_id
                                            INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND c.column_id = dc.parent_column_id
                                            where c.Name = '"+ item["COLUMN_NAME"].ToString() + @"' and t.Name = '"+tableName+"'";
                    DataTable dataTable = dbh.ExecuteQuery(findConstraint);

                   
                    script = script + ' ' + tableName + ' '+ "ALTER COLUMN" +' ' +columnName+' ';
                    if (dataType == "char" || dataType == "varchar" || dataType == "nvarchar" || dataType == "nchar")
                    {
                        if (characterMaximumLength == (-1).ToString())
                            characterMaximumLength = "max";

                        if (characterMaximumLength == (10).ToString())
                            characterMaximumLength = (50).ToString();

                        script = script + ' ' + dataType + ' ' + "(" + characterMaximumLength + ")" + ' ';
                        cOLUMN_DEFAULT = "''";
                    }
                    else if (dataType == "decimal" || dataType == "numeric")
                    {

                        script = script + ' ' + dataType + ' ' + "(" + nUMERIC_PRECISION + ", " + nUMERIC_SCALE + ")" + ' ';
                        cOLUMN_DEFAULT = "1";
                    }
                    else if (dataType == "tinyint")
                    {
                        dataType = "int";
                        script = script + ' ' + dataType;
                        cOLUMN_DEFAULT = "1";
                    }
                    else
                    {
                        script = script + ' ' + dataType;
                        cOLUMN_DEFAULT = "1";
                    }

                    if (dataTable.Rows.Count > 0)
                    {
                        dropConstraint = "Alter Table " + tableName + " drop CONSTRAINT " + dataTable.Rows[0]["DCName"].ToString();
                        createConstraint = @" alter table " + tableName + @" add constraint " + dataTable.Rows[0]["DCName"].ToString()
                           + " default " + cOLUMN_DEFAULT+' ' + " for " + columnName;
                        //@" CREATE INDEX "+ dataTable.Rows[0]["DCName"].ToString()+
                        //                   @" ON "+tableName+" ("+columnName+@") ";
                    }

                    if (!string.IsNullOrEmpty(dropConstraint))
                         dbh.ExecuteQuery(dropConstraint);

                    dbh.ExecuteQuery(script);

                    if (!string.IsNullOrEmpty(createConstraint))
                        dbh.ExecuteQuery(createConstraint);
                }
            }
        }

        public void CreateDiscordViews( List<DataRow> discordListRow , string connectionString)
        {
            using (DBHelper dbh = new DBHelper(connectionString))
            {
                foreach (var item in discordListRow)
                {
                    string tableName = item["TABLE_NAME"].ToString();
                    string vIEW_DEFINITION = item["VIEW_DEFINITION"].ToString();
                   
                    dbh.ExecuteQuery(vIEW_DEFINITION);
                }
            }
        }

        public void CreateDiscordFunctions(List<DataRow> discordListRow, string connectionString)
        {
            using (DBHelper dbh = new DBHelper(connectionString))
            {
                foreach (var item in discordListRow)
                {
                    string name = item["name"].ToString();
                    string definition = item["definition"].ToString();

                    if(!string.IsNullOrEmpty(definition))
                        dbh.ExecuteQuery(definition);
                }
            }
        }

        public void CreateDiscordIndexes(List<DataRow> discordListRow, string connectionString)
        {
            using (DBHelper dbh = new DBHelper(connectionString))
            {
                foreach (var item in discordListRow)
                {
                    //string script = @" CREATE INDEX ";
                                        //ON table_name (column1, column2, ...); ";
                    string indexName = item["IndexName"].ToString();
                    string tableName = item["TableName"].ToString();
                    string columnName = item["ColumnName"].ToString();
                    //if (!string.IsNullOrEmpty(definition))ColumnName
                    //    dbh.ExecuteQuery(definition);
                    string script = "";
                    string dropIndex = "";

                    var isIndexName = DiscordIndexesExecutedList.Where(o => o["IndexName"].ToString() == indexName).Any();

                    if (!isIndexName)
                    {
                        //if(createOrAlter == "CREATE")
                        //constraintQuery = "CONSTRAINT " + cONSTRAINT_NAME + " PRIMARY KEY (" + columnName + ")";
                        //script = script + "PRIMARY KEY" + " ";CONSTRAINT PK_Person PRIMARY KEY (ID,LastName)
                         script = @" CREATE INDEX " + indexName + ' ' + " ON " + ' ' + tableName + ' ' + "(" + columnName + ')';
                    }
                    else 
                    {
                        dropIndex = @"DROP INDEX " +' '+ tableName+"."+ indexName;  //" + tableName + @" DROP CONSTRAINT " + cONSTRAINT_NAME;
                        var indexItems = DiscordResultExecutedList.Where(o => o["IndexName"].ToString() == indexName);
                        script = @" CREATE INDEX " + indexName + ' ' + " ON " + ' ' + tableName + ' ' + "(";

                        if (indexItems.Count() > 0)
                        {
                            foreach (DataRow itemConstrant in indexItems)
                            {
                                script = script + itemConstrant["ColumnName"].ToString() + ",";
                            }

                        }
                        else
                            script = script + ' ' + columnName;

                        script = script + ")";
                        //constraintQuery =  
                        //ALTER TABLE Persons
                        //DROP CONSTRAINT PK_Person;
                    }


                    
                    dbh.ExecuteQuery(script);
                }
            }
        }
        public void CreateDiscordSps(List<DataRow> discordListRow, string connectionString)
        {
            using (DBHelper dbh = new DBHelper(connectionString))
            {
                foreach (var item in discordListRow)
                {
                    //string tableName = item["TABLE_NAME"].ToString();
                    string rOUTINE_DEFINITION = item["ROUTINE_DEFINITION"].ToString();

                    dbh.ExecuteQuery(rOUTINE_DEFINITION);
                }
            }
        }

        private void uiExcute_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                string connectionString = "";
                bool mode = false;
                if (IsDatasourceMode)
                {
                    connectionString = DatasourceDatabseConnectionString;
                    mode = true;
                }
                else
                    connectionString = DestenationDatabseConnectionString;

                //uiProgressBar.Visibility = Visibility.Visible;
                //BackgroundWorker worker = new BackgroundWorker();
                //worker.WorkerReportsProgress = true;
                //worker.DoWork += worker_DoWork;
                //worker.ProgressChanged += worker_ProgressChanged;
                //worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

                //worker.RunWorkerAsync();
           
                CreateScripts(DiscordResultList, connectionString, mode);
                uiReportUpdatesGrid.ItemsSource = ReprotUpdatesList;
            }
            catch(Exception ex)
            {
                uiReportUpdatesGrid.ItemsSource = ReprotUpdatesList;
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //uiProgressBar.Visibility = Visibility.Hidden;
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            uiProgressBar.Value = e.ProgressPercentage;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //uiProgressBar.Visibility = Visibility.Visible;
            for (int i = 0; i < 100; i++)
            {
          
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(100);
            }
        }

        private void uiShowDataYpe_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                IsDatasourceMode = false;
                DataTable dataTable = DatasourceDatabasesDt;
                DataTable dataTable1 = DestenationDatabasesDt;
                List<DataRow> discordResult = GetResultJoin(false, true);
               
                DiscordDataTypeResultList = discordResult;
                uiDiscordCountLbl.Content = discordResult.Count.ToString();
                uiDatasourceDataTypeGrid.ItemsSource = discordResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message);
            }
        }
        public List<DataRow> GetViewsDiscord(bool isDatasourceMode)
        {
            IsDatasourceMode = isDatasourceMode;
            DataTable dataTable = new DataTable();
            DataTable dataTable1 = new DataTable();
            if (isDatasourceMode)
            {
                dataTable = DatasourceViewsDt;
                dataTable1 = DestenationViewsDt;
            }
            else
            {
                dataTable = DestenationViewsDt;
                dataTable1 = DatasourceViewsDt;
            }
                List<DataRow> discordResult = new List<DataRow>();
            var result = dataTable1.AsEnumerable().Join(dataTable.AsEnumerable(),
                              s => new
                              {
                                  s2 = s.Field<string>("TABLE_NAME").ToUpper()
                              },
                              a => new
                              {
                                  s2 = a.Field<string>("TABLE_NAME").ToUpper()
                              },
                              (s, a) => new
                              {
                                  q1 = s.Field<string>("TABLE_CATALOG"),
                                  q2 = s.Field<string>("TABLE_NAME"),
                                  w1 = a.Field<string>("TABLE_CATALOG"),
                                  w2 = a.Field<string>("TABLE_NAME")
                              }).OrderBy(or => or.q1).ToList();


            discordResult = dataTable1.AsEnumerable().Where(o => !(result.Where
                                  (os => os.q2 == o["TABLE_NAME"].ToString() 
                                  )).Any()).ToList();
            return discordResult;
        }

        public List<DataRow> GetSpsDiscord(bool isDatasourceMode)
        {
            IsDatasourceMode = isDatasourceMode;
            DataTable dataTable = new DataTable();
            DataTable dataTable1 = new DataTable();
            if (isDatasourceMode)
            {
                dataTable = DatasourceSpsDt;
                dataTable1 = DestenationSpsDt;
            }
            else
            {
                dataTable = DestenationSpsDt;
                dataTable1 = DatasourceSpsDt;
            }
            List<DataRow> discordResult = new List<DataRow>();
            var result = dataTable1.AsEnumerable().Join(dataTable.AsEnumerable(),
                              s => new
                              {
                                  s2 = s.Field<string>("ROUTINE_NAME").ToUpper()
                              },
                              a => new
                              {
                                  s2 = a.Field<string>("ROUTINE_NAME").ToUpper()
                              },
                              (s, a) => new
                              {
                                  q1 = s.Field<string>("ROUTINE_NAME"),
                                  w1 = a.Field<string>("ROUTINE_NAME")
                              }).OrderBy(or => or.q1).ToList();


            discordResult = dataTable1.AsEnumerable().Where(o => !(result.Where
                                  (os => os.q1 == o["ROUTINE_NAME"].ToString()
                                  )).Any()).ToList();
            return discordResult;
        }

        public List<DataRow> GetFunctionsDiscord(bool isDatasourceMode)
        {
            IsDatasourceMode = isDatasourceMode;
            DataTable dataTable = new DataTable();
            DataTable dataTable1 = new DataTable();
            if (isDatasourceMode)
            {
                dataTable = DatasourceFunctionsDt;
                dataTable1 = DestenationFunctionsDt;
            }
            else
            {
                dataTable = DestenationFunctionsDt;
                dataTable1 = DatasourceFunctionsDt;
            }
            List<DataRow> discordResult = new List<DataRow>();
            var result = dataTable1.AsEnumerable().Join(dataTable.AsEnumerable(),
                              s => new
                              {
                                  s2 = s.Field<string>("name").ToUpper()
                              },
                              a => new
                              {
                                  s2 = a.Field<string>("name").ToUpper()
                              },
                              (s, a) => new
                              {
                                  q1 = s.Field<string>("name"),
                                  w1 = a.Field<string>("name")
                              }).OrderBy(or => or.q1).ToList();


            discordResult = dataTable1.AsEnumerable().Where(o => !(result.Where
                                  (os => os.q1 == o["name"].ToString()
                                  )).Any()).ToList();
            return discordResult;
        }
        public List<DataRow> GetIndexesDiscord(bool isDatasourceMode)
        {
            IsDatasourceMode = isDatasourceMode;
            DataTable dataTable = new DataTable();
            DataTable dataTable1 = new DataTable();
            if (isDatasourceMode)
            {
                dataTable = DatasourceIndexesDt;
                dataTable1 = DestenationIndexesDt;
            }
            else
            {
                dataTable = DestenationIndexesDt;
                dataTable1 = DatasourceIndexesDt;
            }
            List<DataRow> discordResult = new List<DataRow>();
            var result = dataTable1.AsEnumerable().Join(dataTable.AsEnumerable(),
                              s => new
                              {
                                  s2 = s.Field<string>("IndexName").ToUpper()
                              },
                              a => new
                              {
                                  s2 = a.Field<string>("IndexName").ToUpper()
                              },
                              (s, a) => new
                              {
                                  q1 = s.Field<string>("IndexName"),
                                  w1 = a.Field<string>("IndexName")
                              }).OrderBy(or => or.q1).ToList();


            discordResult = dataTable1.AsEnumerable().Where(o => !(result.Where
                                  (os => os.q1 == o["IndexName"].ToString()
                                  )).Any()).ToList();
            return discordResult;
        }

        public List<DataRow> GetResultJoin(bool isDatasourceMode, bool dataTypeMode)
        {
            IsDatasourceMode = isDatasourceMode;
            DataTable dataTable = new DataTable();
            DataTable dataTable1 = new DataTable();
            if (isDatasourceMode)
            {
                dataTable = DatasourceDatabasesDt;
                dataTable1 = DestenationDatabasesDt;
            }
            else
            {
                dataTable = DestenationDatabasesDt;
                dataTable1 = DatasourceDatabasesDt;
            }
            //DataTable dataTable1 = DestenationDatabasesDt;
            List<DataRow> discordResult = new List<DataRow>();
            if (!dataTypeMode)
            {
                var result = dataTable1.AsEnumerable().Join(dataTable.AsEnumerable(),
                                s => new
                                {
                                    s1 = s.Field<string>("COLUMN_NAME").ToUpper(),
                                    s2 = s.Field<string>("TABLE_NAME").ToUpper()
                                },
                                a => new
                                {
                                    s1 = a.Field<string>("COLUMN_NAME").ToUpper(),
                                    s2 = a.Field<string>("TABLE_NAME").ToUpper()
                                },
                                (s, a) => new
                                {
                                    q = s.Field<string>("COLUMN_NAME"),
                                    q1 = s.Field<string>("TABLE_CATALOG"),
                                    q2 = s.Field<string>("TABLE_NAME"),
                                    w = a.Field<string>("COLUMN_NAME"),
                                    w1 = a.Field<string>("TABLE_CATALOG"),
                                    w2 = a.Field<string>("TABLE_NAME")
                                }).OrderBy(or => or.q).ToList();

            
                 discordResult = dataTable1.AsEnumerable().Where(o => !(result.Where
                                       (os => os.q == o["COLUMN_NAME"].ToString() &&
                                       os.q2 == o["TABLE_NAME"].ToString() &&
                                       os.q1 == o["TABLE_CATALOG"].ToString() 
                                       )).Any()).ToList();
            }

            else 
            {
                var result = dataTable1.AsEnumerable().Join(dataTable.AsEnumerable(),
                                s => new
                                {
                                    s1 = s.Field<string>("COLUMN_NAME").ToUpper(),
                                    s2 = s.Field<string>("TABLE_NAME").ToUpper(),
                                    s3 = s.Field<string>("DATA_TYPE").ToUpper()
                                },
                                a => new
                                {
                                    s1 = a.Field<string>("COLUMN_NAME").ToUpper(),
                                    s2 = a.Field<string>("TABLE_NAME").ToUpper(),
                                    s3 = a.Field<string>("DATA_TYPE").ToUpper()
                                },
                                (s, a) => new
                                {
                                    q = s.Field<string>("COLUMN_NAME"),
                                    q1 = s.Field<string>("TABLE_CATALOG"),
                                    q2 = s.Field<string>("TABLE_NAME"),
                                    q3 = s.Field<string>("DATA_TYPE"),
                                    w = a.Field<string>("COLUMN_NAME"),
                                    w1 = a.Field<string>("TABLE_CATALOG"),
                                    w2 = a.Field<string>("TABLE_NAME"),
                                    w3 = a.Field<string>("DATA_TYPE")
                                }).OrderBy(or => or.q).ToList();

                discordResult = dataTable1.AsEnumerable().Where(o => !(result.Where
                                      (os => os.q == o["COLUMN_NAME"].ToString() &&
                                      os.q2 == o["TABLE_NAME"].ToString() &&
                                      os.q1 == o["TABLE_CATALOG"].ToString() &&
                                      os.q3 == o["DATA_TYPE"].ToString()
                                      )).Any()).ToList();
            }


            return discordResult;
        }

        private void uiChangeDataYpe_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                if (IsDatasourceMode)
                    ChngeDataType(DiscordDataTypeResultList, DatasourceDatabseConnectionString);
                else
                    ChngeDataType(DiscordDataTypeResultList, DestenationDatabseConnectionString);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiDiscordViews_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            
            try
            {
                List<DataRow> discordResult = new List<DataRow>();
                if (IsDatasourceMode)
                 discordResult = GetViewsDiscord(true);
                else
                 discordResult = GetViewsDiscord(false);

                DiscordViewsResultList = discordResult;

                uiDiscordViewsGrid.ItemsSource = discordResult;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiCreateViews_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                if (IsDatasourceMode)
                    CreateDiscordViews(DiscordViewsResultList, DatasourceDatabseConnectionString);
                else
                    CreateDiscordViews(DiscordViewsResultList, DestenationDatabseConnectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiSpsDiscordShow_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                List<DataRow> discordResult = new List<DataRow>();
                if (IsDatasourceMode)
                    discordResult = GetSpsDiscord(true);
                else
                    discordResult = GetSpsDiscord(false);

                DiscordFunctionsResultList = discordResult;

                uiDiscordSpsGrid.ItemsSource = discordResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiFunctionsDiscord_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                List<DataRow> discordResult = new List<DataRow>();
                if (IsDatasourceMode)
                    discordResult = GetFunctionsDiscord(true);
                else
                    discordResult = GetFunctionsDiscord(false);

                DiscordSpsResultList = discordResult;

                uiDiscordFunctionsGrid.ItemsSource = discordResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiFunctionsCreate_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                if (IsDatasourceMode)
                    CreateDiscordFunctions(DiscordFunctionsResultList, DatasourceDatabseConnectionString);
                else
                    CreateDiscordFunctions(DiscordFunctionsResultList, DestenationDatabseConnectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiDiscordIndexes_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                List<DataRow> discordResult = new List<DataRow>();
                if (IsDatasourceMode)
                    discordResult = GetIndexesDiscord(true);
                else
                    discordResult = GetIndexesDiscord(false);

                DiscordIndexesResultList = discordResult;

                uiDiscordIndexesGrid.ItemsSource = discordResult;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void uiCreateIndexes_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                if (IsDatasourceMode)
                    CreateDiscordIndexes(DiscordIndexesResultList, DatasourceDatabseConnectionString);
                else
                    CreateDiscordIndexes(DiscordIndexesResultList, DestenationDatabseConnectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
