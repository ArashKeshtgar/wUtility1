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
    public class Amval
    {

        public class AmvalDatabase
        {
            public class AmvalDatabaseItem
            {

                public AmvalDatabaseItem()
                {

                }
                public string ConnectionString { get; set; }
                public string ConnectionStringName { get; set; }
                public string DatabaseName { get; set; }
            }

            private string _connectionStringName { get; set; } = null;
            private string _databaseName { get; set; } = null;
            public AmvalDatabase(string connectionStringName)
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
                        if (CurrentAmval.DatabasesNameDataTable.Rows.Count > 0)
                            count = CurrentAmval.DatabasesNameDataTable.Rows.Count + 1;
                        return count.ToString() + "***Amval";
                    }
                    else
                        return _connectionStringName;
                }
                set
                {

                }
            }

      
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

        }


        public Amval()
        {

        }

        public string DatabaseName { get; set; }
        private string _connectionString { get; set; }
        private string _connectionStringName
        {
            get
            {
                int count = 1;
                if (AmvalConnectionStringNamesList.Count > 0)
                    count = AmvalConnectionStringNamesList.Count() + 1;
                return count.ToString() + "***Amval";
            }

            set
            { }
        }


        public string ConnectionStringName { get; set; }

        private List<string> _AmvalConnectionStringNamesList { get; set; } = new List<string>();
        public List<string> AmvalConnectionStringNamesList
        {
            get
            {
                return _AmvalConnectionStringNamesList;
            }
            set
            {
                _AmvalConnectionStringNamesList = value;
            }
        }



        public static class CurrentAmval
        {
            static public DataTable FillAmvalConnectionStringsList()
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
                string connestr = "***Amval";
                string connectionStringName = num.ToString() + connestr;

                string encryptedValue = AppSettingsManager.GetSetting(connectionStringName);

                if (!string.IsNullOrEmpty(encryptedValue))
                {
                    
                    connectionString = CryptographyUtils.AESCryptography.DecryptStringAES(encryptedValue, "452654645");

                   
                    databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(connectionString);

                    SetDataRowDatabaseName(databaseName);

                    AmvalDatabase AmvalDatabase = new AmvalDatabase(connectionStringName);
                    

                    AddAmvalDatabsesListItem(AmvalDatabase, connectionString, connectionStringName, databaseName);

                }
            }

            private static void SetDataRowDatabaseName(string databaseName)
            {
                if (!ExistingAmvalDatabase(databaseName))
                {
                    DataRow dataRow = DatabaseNameNewRow;
                    dataRow["DatabaseName"] = databaseName;
                    DatabasesNameDataTable.Rows.Add(dataRow);
                }
            }



            private static void AddAmvalDatabsesListItem(AmvalDatabase AmvalDatabase, string connectionString
                                                               , string connectionStringName, string databaseName)
            {
                if (!ExistingAmvalDatabaseList(AmvalDatabase))
                {
                    AmvalDatabase.ConnectionStringRead = connectionString;
                    AmvalDatabase.ConnectionStringName = connectionStringName;
                    AmvalDatabase.DatabaseName = databaseName;

                    AmvalDatabasesList.Add(AmvalDatabase);
                }
            }



            private static bool ExistingAmvalDatabaseList(AmvalDatabase AmvalDatabase)
            {
                bool result = false;
                result = AmvalDatabasesList.Where(o => o.DatabaseName == AmvalDatabase.DatabaseName).Any();
                return result;
            }

            static private bool ExistingAmvalDatabase(string databaseName)
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

            static public List<AmvalDatabase> AmvalDatabasesList { get; set; } = new List<AmvalDatabase>();

            static private DataTable _databasesNameDataTable { get; set; } = null;
            static public DataTable DatabasesNameDataTable
            {
                get
                {
                    if (_databasesNameDataTable == null)
                    {
                        _databasesNameDataTable = new DataTable();

                        _databasesNameDataTable.Columns.Add("DatabaseName");
                        FillAmvalConnectionStringsList();

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


            static public DataRow DatabaseNameNewRow
            {
                get
                {
                    return DatabasesNameDataTable.NewRow();
                }
            }
            





            static private string _connectionStringName = "CurrentAmval";
            static public string ConnectionStringName
            {
                get
                {
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
            public static string AmvalConnectionString
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
                    AppSettingsManager.SetSetting(_connectionStringName, encryptedValue);
                }
            }
        }

        public static string OrginalAmvalDatabaseName
        {
            get
            {
                return OrginalAmvalDatabaseName;
            }
            set
            {
                OrginalAmvalDatabaseName = value;
            }
        }
    }
}
