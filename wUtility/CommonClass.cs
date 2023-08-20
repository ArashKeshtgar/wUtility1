using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using wUtility.Utils;
using static wUtility.Blood;
using static wUtility.Den;
using static wUtility.Drug;
using static wUtility.Food;
using static wUtility.Lab;
using static wUtility.MDoc;
using static wUtility.OpRoom;
using static wUtility.Phisio;
using static wUtility.Radio;
using static wUtility.Tajhizat;
using static wUtility.Tasisat;
using static wUtility.Totalsystem;

namespace wUtility
{
    class  CommanClass
    {
        
        static  public class DatabaseModelName
        {
            static public string dbConfigDataBases
            {
                get
                {
                    return "dbConfigDataBases";
                }
            }

            static public string  Totalsystem
            {
                get
                {
                    return "Totalsystem";
                }
            }

            static public string Lab
            {
                get
                {
                    return "Lab";
                }
            }

            static public string Radio
            {
                get
                {
                    return "Radio";
                }
            }

            static public string  Drug
            {
                get
                {
                    return "Drug";
                }
            }

            static public string Blood
            {
                get
                {
                    return "Blood";
                }
            }

            static public string OpRoom
            {
                get
                {
                    return "OpRoom";
                }
            }

            static public string Phisio
            {
                get
                {
                    return "Phisio";
                }
            }

            static public string Food
            {
                get
                {
                    return "Food";
                }
            }

            static public string Den
            {
                get
                {
                    return "Den";
                }
            }

            static public string Tasisat
            {
                get
                {
                    return "Tasisat";
                }
            }

            static public string Tajhizat
            {
                get
                {
                    return "Tajhizat";
                }
            }

            static public string MDoc
            {
                get
                {
                    return "MDoc"; 
                }
            }

            static public string Requests
            {
                get
                {
                    return "Requests"; 
                }
            }
            static public List<string> ConnectedDatabases
            {
                get
                {
                    List<string> mylist = new List<string>(new string[] { "Totalsystem", "Lab", "Radio",
                                                                            "Drug" , "Blood" , "OpRoom",
                                                                            "Phisio", "Food", "Den",
                                                                             "Tasisat" , "Tajhizat" , "MDoc","Requests"});
                    return mylist;
                }
            }
        }

       static  public class DatasourceDatabases
        {
            public class DatasourceDatabase
            {


                private  string _datasourceDatabaseName { get; set; }
                public  string DatasourceDatabaseName
                {
                    get
                    {
                        return _datasourceDatabaseName;
                    }
                    set
                    {
                        _datasourceDatabaseName = value;
                    }
                }

                private string _datasourceDatabaseModel { get; set; }
                public string DatasourceDatabaseModel
                {
                    get
                    {
                        return _datasourceDatabaseModel;
                    }
                    set
                    {
                        _datasourceDatabaseModel = value;
                    }
                }

                private string _datasourceDatabaseConnectionString { get; set; }
                public string DatasourceDatabaseConnectionString
                {
                    get
                    {
                        return _datasourceDatabaseConnectionString;
                    }
                    set
                    {
                        _datasourceDatabaseConnectionString = value;
                    }
                }
            }
            static private List<DatasourceDatabase> _datasoursesDatabasesList { get; set; } = new List<DatasourceDatabase>();
            static public List<DatasourceDatabase> DatasoursesDatabasesList 
            {
                get
                {
                        return _datasoursesDatabasesList;
                }
                set
                {
                    _datasoursesDatabasesList = value;
                }
            }
            //static  public void AddToConnectedDatabases(string databaseRow)
            //{
            //    DatasoursesDatabasesList.Add(databaseRow);
            //}
            static public string GetDatabaseName(string connectionString)
            {
                string[] connectionStringArray = connectionString.Split(new string[] { "Initial Catalog = " }, StringSplitOptions.None);
                string[] databaseNameArray = connectionStringArray[1].Split(new string[] { ";" }, StringSplitOptions.None);
                return databaseNameArray[0];

            }

            public static bool ExistingDatabaseDatasourceList(string databaseName)
            {
                bool result = false;
                result = DatasoursesDatabasesList.Where(o => o.DatasourceDatabaseName == databaseName).Any();
                return result;
            }


            static public List<DatasourceDatabase> GetDatasourceDatabasesList()
            {
                if (DBHelper.CheckDbConnection1(Totalsystem.CurrentTotalsystem.TotalsystemConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = Totalsystem.CurrentTotalsystem.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = Totalsystem.CurrentTotalsystem.CurrentTotalsystemDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = Totalsystem.CurrentTotalsystem.TotalsystemConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }
                if (DBHelper.CheckDbConnection1(Lab.CurrentLab.LabConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = Lab.CurrentLab.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = Lab.CurrentLab.CurrentLabDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = Lab.CurrentLab.LabConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }
                if (DBHelper.CheckDbConnection1(Radio.CurrentRadio.RadioConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = Radio.CurrentRadio.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = Radio.CurrentRadio.CurrentRadioDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = Radio.CurrentRadio.RadioConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }

                if (DBHelper.CheckDbConnection1(Drug.CurrentDrug.DrugConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = Drug.CurrentDrug.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = Drug.CurrentDrug.CurrentDrugDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = Drug.CurrentDrug.DrugConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }

                if (DBHelper.CheckDbConnection1(Blood.CurrentBlood.BloodConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = Blood.CurrentBlood.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = Blood.CurrentBlood.CurrentBloodDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = Blood.CurrentBlood.BloodConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }

                if (DBHelper.CheckDbConnection1(OpRoom.CurrentOpRoom.OpRoomConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = OpRoom.CurrentOpRoom.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = OpRoom.CurrentOpRoom.CurrentOpRoomDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = OpRoom.CurrentOpRoom.OpRoomConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }

                if (DBHelper.CheckDbConnection1(Phisio.CurrentPhisio.PhisioConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = Phisio.CurrentPhisio.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = Phisio.CurrentPhisio.CurrentPhisioDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = Phisio.CurrentPhisio.PhisioConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }

                if (DBHelper.CheckDbConnection1(Den.CurrentDen.DenConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = Den.CurrentDen.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = Den.CurrentDen.CurrentDenDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = Den.CurrentDen.DenConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }

                if (DBHelper.CheckDbConnection1(Food.CurrentFood.FoodConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = Food.CurrentFood.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = Food.CurrentFood.CurrentFoodDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = Food.CurrentFood.FoodConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }

                if (DBHelper.CheckDbConnection1(Tajhizat.CurrentTajhizat.TajhizatConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = Tajhizat.CurrentTajhizat.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = Tajhizat.CurrentTajhizat.CurrentTajhizatDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = Tajhizat.CurrentTajhizat.TajhizatConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }

                if (DBHelper.CheckDbConnection1(Tasisat.CurrentTasisat.TasisatConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = Tasisat.CurrentTasisat.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = Tasisat.CurrentTasisat.CurrentTasisatDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = Tasisat.CurrentTasisat.TasisatConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }

                if (DBHelper.CheckDbConnection1(MDoc.CurrentMDoc.MDocConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = MDoc.CurrentMDoc.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = MDoc.CurrentMDoc.CurrentMDocDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = MDoc.CurrentMDoc.MDocConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }

                if (DBHelper.CheckDbConnection1(Requests.CurrentRequests.RequestsConnectionString))
                {
                    DatasourceDatabase datasourceDatabase = new DatasourceDatabase();
                    datasourceDatabase.DatasourceDatabaseName = Requests.CurrentRequests.DatabaseName;
                    datasourceDatabase.DatasourceDatabaseModel = Requests.CurrentRequests.CurrentRequestsDatabseModel;
                    datasourceDatabase.DatasourceDatabaseConnectionString = Requests.CurrentRequests.RequestsConnectionString;
                    if (!ExistingDatabaseDatasourceList(datasourceDatabase.DatasourceDatabaseName))
                        DatasoursesDatabasesList.Add(datasourceDatabase);
                }

                return DatasoursesDatabasesList;
            }
        }
        static public class DestnationDatabases
        {
            public class DestnationDatabase
            {


                private string _destnationDatabaseName { get; set; }
                public string DestnationDatabaseName
                {
                    get
                    {
                        return _destnationDatabaseName;
                    }
                    set
                    {
                        _destnationDatabaseName = value;
                    }
                }

                private string _destnationDatabaseModel { get; set; }
                public string DestnationDatabaseModel
                {
                    get
                    {
                        return _destnationDatabaseModel;
                    }
                    set
                    {
                        _destnationDatabaseModel = value;
                    }
                }

                private string _destnationDatabaseConnectionString { get; set; }
                public string DestnationDatabaseConnectionString
                {
                    get
                    {
                        return _destnationDatabaseConnectionString;
                    }
                    set
                    {
                        _destnationDatabaseConnectionString = value;
                    }
                }
            }
            static private List<DestnationDatabase> _destnationDatabasesList { get; set; } = new List<DestnationDatabase>();
            static public List<DestnationDatabase> DestnationDatabasesList
            {
                get
                {
                    return _destnationDatabasesList;
                }
                set
                {
                    _destnationDatabasesList = value;
                }
            }
            //static  public void AddToConnectedDatabases(string databaseRow)
            //{
            //    DatasoursesDatabasesList.Add(databaseRow);
            //}
            static public string GetDatabaseName(string connectionString)
            {
                string[] connectionStringArray = connectionString.Split(new string[] { "Initial Catalog = " }, StringSplitOptions.None);
                string[] databaseNameArray = connectionStringArray[1].Split(new string[] { ";" }, StringSplitOptions.None);
                return databaseNameArray[0];

            }

            public static bool ExistingDatabaseDestnationList(string databaseName)
            {
                bool result = false;
                result = DestnationDatabasesList.Where(o => o.DestnationDatabaseName == databaseName).Any();
                return result;
            }


            static public List<DestnationDatabase> GetDestnationDatabasesList(string databaseModel)
            {
                if (databaseModel == CurrentTotalsystem.CurrentTotalsystemDatabseModel)
                {
                    if (CurrentTotalsystem.TotalsystemDatabasesList.Count == 0)
                        CurrentTotalsystem.FillTotalsystemConnectionStringsList();

                    foreach (var item in CurrentTotalsystem.TotalsystemDatabasesList)
                    {
                        DestnationDatabase destnationDatabase = new DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.TotalsystemDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }
                
                if (databaseModel == CurrentLab.CurrentLabDatabseModel)
                {
                    if (CurrentLab.LabDatabasesList.Count == 0)
                        CurrentLab.FillLabConnectionStringsList();

                    foreach (var item in CurrentLab.LabDatabasesList)
                    {
                        CommanClass.DestnationDatabases.DestnationDatabase destnationDatabase = new CommanClass.DestnationDatabases.DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.LabDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }

                if (databaseModel == CurrentRadio.CurrentRadioDatabseModel)
                {
                    if (CurrentRadio.RadioDatabasesList.Count == 0)
                        CurrentRadio.FillRadioConnectionStringsList();

                    foreach (var item in CurrentRadio.RadioDatabasesList)
                    {
                        DestnationDatabase destnationDatabase = new CommanClass.DestnationDatabases.DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.RadioDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }

                if (databaseModel == CurrentDrug.CurrentDrugDatabseModel)
                {
                    if (CurrentDrug.DrugDatabasesList.Count == 0)
                        CurrentDrug.FillDrugConnectionStringsList();

                    foreach (var item in CurrentDrug.DrugDatabasesList)
                    {
                        DestnationDatabase destnationDatabase = new DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.DrugDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }

                if (databaseModel == CurrentBlood.CurrentBloodDatabseModel)
                {
                    if (CurrentBlood.BloodDatabasesList.Count == 0)
                        CurrentBlood.FillBloodConnectionStringsList();

                    foreach (var item in CurrentBlood.BloodDatabasesList)
                    {
                        DestnationDatabase destnationDatabase = new DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.BloodDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }

                if (databaseModel == CurrentOpRoom.CurrentOpRoomDatabseModel)
                {
                    if (CurrentOpRoom.OpRoomDatabasesList.Count == 0)
                        CurrentOpRoom.FillOpRoomConnectionStringsList();

                    foreach (var item in CurrentOpRoom.OpRoomDatabasesList)
                    {
                        DestnationDatabase destnationDatabase = new DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.OpRoomDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }

                if (databaseModel == CurrentPhisio.CurrentPhisioDatabseModel)
                {
                    if (CurrentPhisio.PhisioDatabasesList.Count == 0)
                        CurrentPhisio.FillPhisioConnectionStringsList();

                    foreach (var item in CurrentPhisio.PhisioDatabasesList)
                    {
                        DestnationDatabase destnationDatabase = new DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.PhisioDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }

                if (databaseModel == CurrentDen.CurrentDenDatabseModel)
                {
                    if (CurrentDen.DenDatabasesList.Count == 0)
                        CurrentDen.FillDenConnectionStringsList();

                    foreach (var item in CurrentDen.DenDatabasesList)
                    {
                        DestnationDatabase destnationDatabase = new DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.DenDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }

                if (databaseModel == CurrentFood.CurrentFoodDatabseModel)
                {
                    if (CurrentFood.FoodDatabasesList.Count == 0)
                        CurrentFood.FillFoodConnectionStringsList();

                    foreach (var item in CurrentFood.FoodDatabasesList)
                    {
                        DestnationDatabase destnationDatabase = new DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.FoodDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }


                
                if (databaseModel == CurrentTajhizat.CurrentTajhizatDatabseModel)
                {
                    if (CurrentTajhizat.TajhizatDatabasesList.Count == 0)
                        CurrentTajhizat.FillTajhizatConnectionStringsList();

                    foreach (var item in CurrentTajhizat.TajhizatDatabasesList)
                    {
                        DestnationDatabase destnationDatabase = new DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.TajhizatDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }

                if (databaseModel == CurrentTasisat.CurrentTasisatDatabseModel)
                {
                    if (CurrentTasisat.TasisatDatabasesList.Count == 0)
                        CurrentTasisat.FillTasisatConnectionStringsList();

                    foreach (var item in CurrentTasisat.TasisatDatabasesList)
                    {
                        DestnationDatabase destnationDatabase = new DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.TasisatDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }

                if (databaseModel == CurrentMDoc.CurrentMDocDatabseModel)
                {
                    if (CurrentMDoc.MDocDatabasesList.Count == 0)
                        CurrentMDoc.FillMDocConnectionStringsList();

                    foreach (var item in CurrentMDoc.MDocDatabasesList)
                    {
                        DestnationDatabase destnationDatabase = new DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.MDocDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }

                if (databaseModel ==Requests.CurrentRequests.CurrentRequestsDatabseModel)
                {
                    if (Requests.CurrentRequests.RequestsDatabasesList.Count == 0)
                        Requests.CurrentRequests.FillRequestsConnectionStringsList();

                    foreach (var item in Requests.CurrentRequests.RequestsDatabasesList)
                    {
                        DestnationDatabase destnationDatabase = new DestnationDatabase();
                        destnationDatabase.DestnationDatabaseName = item.DatabaseName;
                        destnationDatabase.DestnationDatabaseModel = item.RequestsDatabseModel;
                        destnationDatabase.DestnationDatabaseConnectionString = item.ConnectionStringRead;
                        if (!ExistingDatabaseDestnationList(destnationDatabase.DestnationDatabaseName))
                            DestnationDatabasesList.Add(destnationDatabase);
                    }
                }

                return DestnationDatabasesList;
            }
        }

    }
}

/////Multiple Selection <ListBox x:Name="listBoxSelectedItems" 
//DisplayMemberPath="Name" 
 //ItemsSource="{Binding SelectedItems, ElementName=radGridView}" />   listBoxSelectedItems.ItemsSource = this.radGridView.SelectedItems; 
/// select column this.radGridView.Columns["Name"].IsSelected = true;   this.radGridView.SelectCellRegion(new CellRegion(this.radGridView.Columns["Name"].DisplayIndex, 0, 1, this.radGridView.Items.Count)); 