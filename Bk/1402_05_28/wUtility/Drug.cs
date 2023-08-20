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
    class Drug
    {
        public class DrugDatabase
        {
            public class DrugDatabaseItem
            {

                public DrugDatabaseItem()
                {

                }
                //connectionString, connectionStringName, databaseName
                public string ConnectionString { get; set; }
                public string ConnectionStringName { get; set; }
                public string DatabaseName { get; set; }
            }
            //public List<DrugDatabaseItem> DrugDatabaseItemsList { get; set; }
            private string _connectionStringName { get; set; } = null;
            private string _databaseName { get; set; } = null;
            public DrugDatabase(string connectionStringName)
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
                        if (CurrentDrug.DatabasesNameDataTable.Rows.Count > 0)
                            count = CurrentDrug.DatabasesNameDataTable.Rows.Count + 1;
                        return count.ToString() + "***Drug";
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

            public string DrugDatabseModel
            {
                get
                {
                    return CommanClass.DatabaseModelName.Drug;
                }
            }
        }



        public Drug()
        {

        }

        public static string DrugDatabseModel
        {
            get
            {
                return CommanClass.DatabaseModelName.Drug;
            }
        }

        public static class CurrentDrug
        {
            static public DataTable FillDrugConnectionStringsList()
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
                string connestr = "***Drug";
                string connectionStringName = num.ToString() + connestr;

                string encryptedValue = AppSettingsManager.GetSetting(connectionStringName);

                if (!string.IsNullOrEmpty(encryptedValue))
                {
                    connectionString = CryptographyUtils.AESCryptography.DecryptStringAES(encryptedValue, "452654645");

                    databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(connectionString);

                    SetDataRowDatabaseName(databaseName);

                    DrugDatabase DrugDatabase = new DrugDatabase(connectionStringName);

                    AddDrugDatabsesListItem(DrugDatabase, connectionString, connectionStringName, databaseName);

                }
            }

            private static void SetDataRowDatabaseName(string databaseName)
            {
                if (!ExistingDrugDatabase(databaseName))
                {
                    DataRow dataRow = DatabaseNameNewRow;
                    dataRow["DatabaseName"] = databaseName;
                    DatabasesNameDataTable.Rows.Add(dataRow);
                }
            }



            private static void AddDrugDatabsesListItem(DrugDatabase DrugDatabase, string connectionString
                                                               , string connectionStringName, string databaseName)
            {
                if (!ExistingDrugDatabaseList(DrugDatabase))
                {
                    DrugDatabase.ConnectionStringRead = connectionString;
                    DrugDatabase.ConnectionStringName = connectionStringName;
                    DrugDatabase.DatabaseName = databaseName;

                    DrugDatabasesList.Add(DrugDatabase);
                }
            }



            private static bool ExistingDrugDatabaseList(DrugDatabase DrugDatabase)
            {
                bool result = false;
                result = DrugDatabasesList.Where(o => o.DatabaseName == DrugDatabase.DatabaseName).Any();
                return result;
            }

            static private bool ExistingDrugDatabase(string databaseName)
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

            static public List<DrugDatabase> DrugDatabasesList { get; set; } = new List<DrugDatabase>();

            static private DataTable _databasesNameDataTable { get; set; } = null;
            static public DataTable DatabasesNameDataTable
            {
                get
                {
                    if (_databasesNameDataTable == null)
                    {
                        _databasesNameDataTable = new DataTable();

                        _databasesNameDataTable.Columns.Add("DatabaseName");
                        FillDrugConnectionStringsList();
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


            public static string CurrentDrugDatabseModel
            {
                get
                {
                    return CommanClass.DatabaseModelName.Drug;
                }
            }

            private static string _connectionStringName = "CurrentDrug";
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
            public static string DrugConnectionString
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
