using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using wUtility.Utils;

namespace wUtility
{
   public class Totalsystem
    {
        
        public class TotalsystemDatabase
        {
            public class TotalsystemDatabaseItem
            {
         
                public TotalsystemDatabaseItem()
                {

                }
                //connectionString, connectionStringName, databaseName
                public string ConnectionString { get; set; }
                public string ConnectionStringName { get; set; }
                public string DatabaseName { get; set; }
            }
            //public List<TotalsystemDatabaseItem> TotalsystemDatabaseItemsList { get; set; }
            private string _connectionStringName { get; set; } = null;
            private string _databaseName { get; set; } = null;
            public TotalsystemDatabase(string connectionStringName)
            {
                _connectionStringName = connectionStringName;
            }
            public string DatabaseName
            {
                get
                {
                    return _databaseName;
                }
                set
                {
                    _databaseName = value;
                }
            }


            public string ConnectionStringName
            {
                get
                {
                    if (string.IsNullOrEmpty(_connectionStringName))
                    {
                        int count = 1;
                        if (CurrentTotalsystem.DatabasesNameDataTable.Rows.Count > 0)
                            count = CurrentTotalsystem.DatabasesNameDataTable.Rows.Count + 1;
                        return count.ToString() + "***Totalsystem";
                    }
                    else
                        return _connectionStringName;
                }
                set
                {

                }
            }

            /// <summary>
            /// //// this property for save connectionString
            /// </summary>
            public string ConnectionString
            {
                get
                {
                    string encryptedValue = AppSettingsManager.GetSetting(ConnectionStringName);
                    if (!string.IsNullOrEmpty(ConnectionStringName))
                        return CryptographyUtils.AESCryptography.DecryptStringAES(encryptedValue, "452654645");
                    else
                        return null;
                }
                set
                {
                    string encryptedValue = "";
                    if (!string.IsNullOrEmpty(value))
                        encryptedValue = CryptographyUtils.AESCryptography.EncryptStringAES(value, "452654645");
                    AppSettingsManager.SetSetting(ConnectionStringName, encryptedValue);
                }
            }

            private string _connectionStringRead { get; set; }
            public string ConnectionStringRead
            {
                get
                {
                        return _connectionStringRead;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                        _connectionStringRead = value;
                }
            }

            public string TotalsystemDatabseModel
            {
                get
                {
                    return CommanClass.DatabaseModelName.Totalsystem;
                }
            }
        }

        
        public Totalsystem()
        {

        }

        public string DatabaseName { get; set; }
        private string _connectionString { get; set; }
        private string _connectionStringName
        {
            get
            {
                int count = 1;
                if(TotalsystemConnectionStringNamesList.Count>0)
                 count = TotalsystemConnectionStringNamesList.Count() + 1;
                return count.ToString() + "***Totalsystem";
            }

            set
            { }
        }


        public string ConnectionStringName { get; set; }
        
        //private List<string> _totalsystemDatabsesNamesList { get; set; } = new List<string>();
        //public List<string> TotalsystemDatabsesNamesList
        //{
        //    get
        //    {
        //        return _totalsystemDatabsesNamesList;
        //    }
        //    set
        //    {
        //        _totalsystemDatabsesNamesList = value;
        //    }
        //}

        private List<string> _totalsystemConnectionStringNamesList { get; set; } = new List<string>();
        public List<string> TotalsystemConnectionStringNamesList
        {
            get
            {
                return _totalsystemConnectionStringNamesList;
            }
            set
            {
                _totalsystemConnectionStringNamesList = value;
            }
        }

        
        public string TotalsystemDatabseModel
        {
            get
            {
                return CommanClass.DatabaseModelName.Totalsystem;
            }
        }


        public static class CurrentTotalsystem
        {
           static public DataTable FillTotalsystemConnectionStringsList()
            {
                
                for (int i = 1; i <= 60; i++)
                {
                    
                    EncryptedValueToConnetionString(i);
                    
                }
                return DatabasesNameDataTable;
            }

            private static void EncryptedValueToConnetionString(int num)
            {
                string connectionString = "";
                string databaseName = "";
                string connestr = "***Totalsystem";
                string connectionStringName = num.ToString() + connestr;

                string encryptedValue = AppSettingsManager.GetSetting(connectionStringName);

                if (!string.IsNullOrEmpty(encryptedValue))
                {
                    //DataRow row = TotalsystemDatabaseNewRow;
                    connectionString = CryptographyUtils.AESCryptography.DecryptStringAES(encryptedValue, "452654645");

                    //ConnectionStringName = connectionStringName;
                    databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(connectionString);

                    SetDataRowDatabaseName(databaseName);
                    
                    TotalsystemDatabase totalsystemDatabase = new TotalsystemDatabase(connectionStringName);
                    //TotalsystemDatabase.TotalsystemDatabaseItem totalsystemDatabaseItem = new TotalsystemDatabase.TotalsystemDatabaseItem(totalsystemDatabase);

                    AddTotalsystemDatabsesListItem(totalsystemDatabase, connectionString, connectionStringName, databaseName);
                    
                }
            }

            private static void SetDataRowDatabaseName(string databaseName)
            {
                if (!ExistingTotalsystemDatabase(databaseName))
                {
                    DataRow dataRow = DatabaseNameNewRow;
                    dataRow["DatabaseName"] = databaseName;
                    DatabasesNameDataTable.Rows.Add(dataRow);
                }
            }


            
            private static void AddTotalsystemDatabsesListItem(TotalsystemDatabase totalsystemDatabase, string connectionString
                                                               ,string connectionStringName, string databaseName)
            {
                if (!ExistingTotalsystemDatabaseList(totalsystemDatabase))
                {
                    totalsystemDatabase.ConnectionStringRead = connectionString;
                    totalsystemDatabase.ConnectionStringName = connectionStringName;
                    totalsystemDatabase.DatabaseName = databaseName;

                    TotalsystemDatabasesList.Add(totalsystemDatabase);
                }
            }

     

            private static  bool ExistingTotalsystemDatabaseList(TotalsystemDatabase totalsystemDatabase)
            {
                bool result = false;
                result = TotalsystemDatabasesList.Where(o => o.DatabaseName == totalsystemDatabase.DatabaseName).Any();
                return result;
            }

            static private bool ExistingTotalsystemDatabase(string databaseName)
            {
                bool result = false;
                result = DatabasesNameDataTable.AsEnumerable().Where(o => o["DatabaseName"].ToString() == databaseName).Any();
                return result;
            }


            static private bool ExistingDatabaseNameList(string databaseName)
            {
                bool result = false;
                result = _databasesNamesList.Where(o => o == databaseName).Any();
                return result;
            }

            static public List<TotalsystemDatabase> TotalsystemDatabasesList { get; set;} = new List<TotalsystemDatabase>();

            static private DataTable _databasesNameDataTable { get; set; } = null;
            static public DataTable DatabasesNameDataTable
            {
                get
                {
                    if (_databasesNameDataTable == null)
                    {
                        _databasesNameDataTable = new DataTable();

                        _databasesNameDataTable.Columns.Add("DatabaseName");
                        FillTotalsystemConnectionStringsList();
                        //_databasesNameDataTable.Columns.Add("ConnectionStringName");
                        //_databasesNameDataTable.Columns.Add("DatabaseName");
                    }
                    return _databasesNameDataTable;
                }
            }

            static public List<string> _databasesNamesList { get; set; } = new List<string>();
            static public List<string> DatabasesNamesList
            {
                get
                {
                    if (_databasesNamesList.Count == 0)
                        _databasesNamesList = new List<string>();

                    foreach (DataRow dataRow in DatabasesNameDataTable.Rows)
                    {
                        if (!ExistingDatabaseNameList(dataRow["DatabaseName"].ToString()))
                            _databasesNamesList.Add(dataRow["DatabaseName"].ToString());
                    }
                    return _databasesNamesList;
                }
            }
            //static public DataRow TotalsystemDatabaseNewRow
            //{
            //    get
            //    {
            //        return TotalsystemDatabasesDataTable.NewRow();
            //    }
            //}

            static public DataRow DatabaseNameNewRow
            {
                get
                {
                    return DatabasesNameDataTable.NewRow();
                }
            }
            //static private List<string> _totalsystemConnectionStringNamesList { get; set; } = new List<string>();
            //static public List<string> TotalsystemConnectionStringNamesList
            //{
            //    get
            //    {
            //        return _totalsystemConnectionStringNamesList;
            //    }
            //    set
            //    {
            //        _totalsystemConnectionStringNamesList = value;
            //    }
            //}


            public static string CurrentTotalsystemDatabseModel
            {
                get
                {
                    return CommanClass.DatabaseModelName.Totalsystem;
                }
            }

            

            static private  string _connectionStringName = "CurrentTotalsystem";
            static public string ConnectionStringName
            {
                get
                {
                    //string encryptedValue = AppSettingsManager.GetSetting(_connectionStringName);
                    //if (!string.IsNullOrEmpty(_connectionStringName))
                    //    return _connectionStringName;
                    //else
                        return _connectionStringName;
                }
                set
                {
                    if (!string.IsNullOrEmpty(value))
                        _connectionStringName = value;
                }
            }
            static public string DatabaseName
            {
                get
                {
                    string encryptedValue = AppSettingsManager.GetSetting(_connectionStringName);
                    string connectionString = CryptographyUtils.AESCryptography.DecryptStringAES(encryptedValue, "452654645");
                    string[] connectionStringArray = connectionString.Split(new string[] { "Initial Catalog = " }, StringSplitOptions.None);
                    string[] databaseNameArray = connectionStringArray[1].Split(new string[] { ";" }, StringSplitOptions.None);
                    if (!string.IsNullOrEmpty(databaseNameArray[0]))
                        return databaseNameArray[0];
                    else
                        return null;
                }

            }
            public static string TotalsystemConnectionString
            {
                get
                {
                    string encryptedValue = AppSettingsManager.GetSetting(ConnectionStringName);
                    if (!string.IsNullOrEmpty(ConnectionStringName))
                        return CryptographyUtils.AESCryptography.DecryptStringAES( encryptedValue, "452654645");
                    else
                        return null;
                }
                set
                {
                    string encryptedValue = "";
                    if (!string.IsNullOrEmpty(value))
                        encryptedValue = CryptographyUtils.AESCryptography.EncryptStringAES(value, "452654645");
                    AppSettingsManager.SetSetting(_connectionStringName, encryptedValue);
                }
            }
        }

        public static string OrginalTotalsystemDatabaseName
        {
            get
            {
                return OrginalTotalsystemDatabaseName;
            }
            set
            {
                OrginalTotalsystemDatabaseName = value;
            }
        }
    }
}
