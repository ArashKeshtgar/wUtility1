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
    class Requests
    {

        public class RequestsDatabase
        {
            public class RequestsDatabaseItem
            {

                public RequestsDatabaseItem()
                {

                }
                //connectionString, connectionStringName, databaseName
                public string ConnectionString { get; set; }
                public string ConnectionStringName { get; set; }
                public string DatabaseName { get; set; }
            }
            //public List<RequestsDatabaseItem> RequestsDatabaseItemsList { get; set; }
            private string _connectionStringName { get; set; } = null;
            private string _databaseName { get; set; } = null;
            public RequestsDatabase(string connectionStringName)
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
                        if (CurrentRequests.DatabasesNameDataTable.Rows.Count > 0)
                            count = CurrentRequests.DatabasesNameDataTable.Rows.Count + 1;
                        return count.ToString() + "***Requests";
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

            public string RequestsDatabseModel
            {
                get
                {
                    return CommanClass.DatabaseModelName.Requests;
                }
            }
        }


        public Requests()
        {

        }

        public string DatabaseName { get; set; }
        private string _connectionString { get; set; }
        private string _connectionStringName
        {
            get
            {
                int count = 1;
                if (RequestsConnectionStringNamesList.Count > 0)
                    count = RequestsConnectionStringNamesList.Count() + 1;
                return count.ToString() + "***Requests";
            }

            set
            { }
        }


        public string ConnectionStringName { get; set; }

        //private List<string> _RequestsDatabsesNamesList { get; set; } = new List<string>();
        //public List<string> RequestsDatabsesNamesList
        //{
        //    get
        //    {
        //        return _RequestsDatabsesNamesList;
        //    }
        //    set
        //    {
        //        _RequestsDatabsesNamesList = value;
        //    }
        //}

        private List<string> _RequestsConnectionStringNamesList { get; set; } = new List<string>();
        public List<string> RequestsConnectionStringNamesList
        {
            get
            {
                return _RequestsConnectionStringNamesList;
            }
            set
            {
                _RequestsConnectionStringNamesList = value;
            }
        }


        public string RequestsDatabseModel
        {
            get
            {
                return CommanClass.DatabaseModelName.Requests;
            }
        }


        public static class CurrentRequests
        {
            static public DataTable FillRequestsConnectionStringsList()
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
                string connestr = "***Requests";
                string connectionStringName = num.ToString() + connestr;

                string encryptedValue = AppSettingsManager.GetSetting(connectionStringName);

                if (!string.IsNullOrEmpty(encryptedValue))
                {
                    //DataRow row = RequestsDatabaseNewRow;
                    connectionString = CryptographyUtils.AESCryptography.DecryptStringAES(encryptedValue, "452654645");

                    //ConnectionStringName = connectionStringName;
                    databaseName = CommanClass.DatasourceDatabases.GetDatabaseName(connectionString);

                    SetDataRowDatabaseName(databaseName);

                    RequestsDatabase RequestsDatabase = new RequestsDatabase(connectionStringName);
                    //RequestsDatabase.RequestsDatabaseItem RequestsDatabaseItem = new RequestsDatabase.RequestsDatabaseItem(RequestsDatabase);

                    AddRequestsDatabsesListItem(RequestsDatabase, connectionString, connectionStringName, databaseName);

                }
            }

            private static void SetDataRowDatabaseName(string databaseName)
            {
                if (!ExistingRequestsDatabase(databaseName))
                {
                    DataRow dataRow = DatabaseNameNewRow;
                    dataRow["DatabaseName"] = databaseName;
                    DatabasesNameDataTable.Rows.Add(dataRow);
                }
            }



            private static void AddRequestsDatabsesListItem(RequestsDatabase RequestsDatabase, string connectionString
                                                               , string connectionStringName, string databaseName)
            {
                if (!ExistingRequestsDatabaseList(RequestsDatabase))
                {
                    RequestsDatabase.ConnectionStringRead = connectionString;
                    RequestsDatabase.ConnectionStringName = connectionStringName;
                    RequestsDatabase.DatabaseName = databaseName;

                    RequestsDatabasesList.Add(RequestsDatabase);
                }
            }



            private static bool ExistingRequestsDatabaseList(RequestsDatabase RequestsDatabase)
            {
                bool result = false;
                result = RequestsDatabasesList.Where(o => o.DatabaseName == RequestsDatabase.DatabaseName).Any();
                return result;
            }

            static private bool ExistingRequestsDatabase(string databaseName)
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

            static public List<RequestsDatabase> RequestsDatabasesList { get; set; } = new List<RequestsDatabase>();

            static private DataTable _databasesNameDataTable { get; set; } = null;
            static public DataTable DatabasesNameDataTable
            {
                get
                {
                    if (_databasesNameDataTable == null)
                    {
                        _databasesNameDataTable = new DataTable();

                        _databasesNameDataTable.Columns.Add("DatabaseName");
                        FillRequestsConnectionStringsList();
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
            //static public DataRow RequestsDatabaseNewRow
            //{
            //    get
            //    {
            //        return RequestsDatabasesDataTable.NewRow();
            //    }
            //}

            static public DataRow DatabaseNameNewRow
            {
                get
                {
                    return DatabasesNameDataTable.NewRow();
                }
            }
            //static private List<string> _RequestsConnectionStringNamesList { get; set; } = new List<string>();
            //static public List<string> RequestsConnectionStringNamesList
            //{
            //    get
            //    {
            //        return _RequestsConnectionStringNamesList;
            //    }
            //    set
            //    {
            //        _RequestsConnectionStringNamesList = value;
            //    }
            //}


            public static string CurrentRequestsDatabseModel
            {
                get
                {
                    return CommanClass.DatabaseModelName.Requests;
                }
            }



            static private string _connectionStringName = "CurrentRequests";
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
            public static string RequestsConnectionString
            {
                get
                {
                    string encryptedValue = AppSettingsManager.GetSetting(ConnectionStringName);
                    if (!string.IsNullOrEmpty(ConnectionStringName) && !string.IsNullOrEmpty(encryptedValue))
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

        public static string OrginalRequestsDatabaseName
        {
            get
            {
                return OrginalRequestsDatabaseName;
            }
            set
            {
                OrginalRequestsDatabaseName = value;
            }
        }
    }
}
