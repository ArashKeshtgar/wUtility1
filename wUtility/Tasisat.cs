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
    class Tasisat
    {
        public class TasisatDatabase
        {
            public class TasisatDatabaseItem
            {

                public TasisatDatabaseItem()
                {

                }
                //connectionString, connectionStringName, databaseName
                public string ConnectionString { get; set; }
                public string ConnectionStringName { get; set; }
                public string DatabaseName { get; set; }
            }
            //public List<TasisatDatabaseItem> TasisatDatabaseItemsList { get; set; }
            private string _connectionStringName { get; set; } = null;
            private string _databaseName { get; set; } = null;
            public TasisatDatabase(string connectionStringName)
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
                        if (CurrentTasisat.DatabasesNameDataTable.Rows.Count > 0)
                            count = CurrentTasisat.DatabasesNameDataTable.Rows.Count + 1;
                        return count.ToString() + "***Tasisat";
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

            public string TasisatDatabseModel
            {
                get
                {
                    return CommanClass.DatabaseModelName.Tasisat;
                }
            }
        }


        public Tasisat()
        {

        }

        public static string TasisatDatabseModel
        {
            get
            {
                return CommanClass.DatabaseModelName.Tasisat;
            }
        }

        public static class CurrentTasisat
        {
            static public DataTable FillTasisatConnectionStringsList()
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
                string connestr = "***Tasisat";
                string connectionStringName = num.ToString() + connestr;

                string encryptedValue = AppSettingsManager.GetSetting(connectionStringName);

                if (!string.IsNullOrEmpty(encryptedValue))
                {
                    connectionString = CryptographyUtils.AESCryptography.DecryptStringAES(encryptedValue, "452654645");

                    databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(connectionString);

                    SetDataRowDatabaseName(databaseName);

                    TasisatDatabase TasisatDatabase = new TasisatDatabase(connectionStringName);

                    AddTasisatDatabsesListItem(TasisatDatabase, connectionString, connectionStringName, databaseName);

                }
            }

            private static void SetDataRowDatabaseName(string databaseName)
            {
                if (!ExistingTasisatDatabase(databaseName))
                {
                    DataRow dataRow = DatabaseNameNewRow;
                    dataRow["DatabaseName"] = databaseName;
                    DatabasesNameDataTable.Rows.Add(dataRow);
                }
            }



            private static void AddTasisatDatabsesListItem(TasisatDatabase TasisatDatabase, string connectionString
                                                               , string connectionStringName, string databaseName)
            {
                if (!ExistingTasisatDatabaseList(TasisatDatabase))
                {
                    TasisatDatabase.ConnectionStringRead = connectionString;
                    TasisatDatabase.ConnectionStringName = connectionStringName;
                    TasisatDatabase.DatabaseName = databaseName;

                    TasisatDatabasesList.Add(TasisatDatabase);
                }
            }



            private static bool ExistingTasisatDatabaseList(TasisatDatabase TasisatDatabase)
            {
                bool result = false;
                result = TasisatDatabasesList.Where(o => o.DatabaseName == TasisatDatabase.DatabaseName).Any();
                return result;
            }

            static private bool ExistingTasisatDatabase(string databaseName)
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

            static public List<TasisatDatabase> TasisatDatabasesList { get; set; } = new List<TasisatDatabase>();

            static private DataTable _databasesNameDataTable { get; set; } = null;
            static public DataTable DatabasesNameDataTable
            {
                get
                {
                    if (_databasesNameDataTable == null)
                    {
                        _databasesNameDataTable = new DataTable();

                        _databasesNameDataTable.Columns.Add("DatabaseName");
                        FillTasisatConnectionStringsList();
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


            public static string CurrentTasisatDatabseModel
            {
                get
                {
                    return CommanClass.DatabaseModelName.Tasisat;
                }
            }

            private static string _connectionStringName = "CurrentTasisat";
            static public string ConnectionStringName
            {
                get
                {
                    string encryptedValue = AppSettingsManager.GetSetting(_connectionStringName);
                    if (!string.IsNullOrEmpty(encryptedValue))
                        return _connectionStringName;
                    else
                        return null;
                }
                set
                {
                    if (string.IsNullOrEmpty(value))
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
            public static string TasisatConnectionString
            {
                get
                {
                    string encryptedValue = AppSettingsManager.GetSetting(_connectionStringName);
                    if (!string.IsNullOrEmpty(encryptedValue))
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
    }
}
