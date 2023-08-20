using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;


namespace wUtility.Utils
{
    public class DBHelper : IDisposable
    {
        public string ConnectionString { get; private set; }

        

        public SqlConnection Connection { get; private set; }


        private static string _editDataset;
        public static DataSet EditDataSet
        {
            get
            {
                using (DBHelper dbh = new DBHelper(ConnectionStrings.DrOfficeDB))
                    return dbh.TableDataSet(_editDataset);
            }
        }


        public DBHelper(string connectionString)
        {
            this.ConnectionString = connectionString;
            Connection = new SqlConnection(this.ConnectionString);
        }

        //private Telerik.WinControls.UI.RadGridView uiProceedingsGrid;

        //public DBHelper(string connectionString, Telerik.WinControls.UI.RadGridView select)
        //{
        //    this.ConnectionString = connectionString;
        //    this.uiProceedingsGrid = select;
        //}



        //private static Telerik.WinControls.UI.RadGridView SelectRow { get; set; }
        //public static DataRow SelectedRow
        //{
        //    get
        //    {
        //        if (SelectRow.CurrentRow == null)
        //            return null;
        //        return ((System.Data.DataRowView)(SelectRow.CurrentRow.DataBoundItem)).Row;
        //    }
        //}
        
        bool disposed = false;
        //private string p;
        //private Telerik.WinControls.UI.RadGridView uiProCeedingsGrid;

        public static string GetDbVersion()
        {
            using (var dbHelper = new DBHelper(ConnectionStrings.DrOfficeDB))
            {
                return dbHelper.ExecuteScalarQuery<string>("select Value from Settings where Name = 'Version'");
            }
        }



        public static bool CheckDbConnection1(string connectionString)
        {
            using (var dbHelper = new DBHelper(connectionString))
            {
                return dbHelper.CheckDbConnection();
            }
        }

        public static bool CheckDbConfigDbsConnection()
        {
            using (var dbHelper = new DBHelper(dbConfigDataBases.dbConfigDataBasesConnectionString))
            {
                return dbHelper.CheckDbConnection();
            }
        }

        ~DBHelper()
        {
            Dispose(false);
        }

        public bool CheckDbConnection()
        {
            try
            {
                ExecuteQuery("select 'Testing Connection.'");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public System.Data.DataTable ExecuteQuery(string query)
        {
            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            cmd.CommandText = query;
            cmd.CommandTimeout = 1800;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            System.Data.DataTable dt = new System.Data.DataTable();
            da.Fill(dt);
            return dt;
        }

        public object ExecuteScalarQuery(string query)
        {
            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            cmd.CommandText = query;

            var result = cmd.ExecuteScalar();
            return result;
        }

        public TResult ExecuteScalarQuery<TResult>(string query)
        {
            return (TResult)ExecuteScalarQuery(query);
        }

        public System.Data.DataTable Select(string tableName, string filter)
        {
            var query = "Select * FROM " + tableName;

            if (!(string.IsNullOrEmpty(filter)))
                query += " where " + filter;

            return ExecuteQuery(query);
        }

        public System.Data.DataTable Select(string tableName)
        {
            return Select(tableName, null);
        }

        public DataRow SelectSingle(string tableName, string filter)
        {
            var table = Select(tableName, filter);
            if (table.Rows.Count > 0)
                return table.Rows[0];
            else
                return null;
        }

        public DataRow GetNewRow(string tableName)
        {
            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            cmd.CommandText = "Select top 0 * FROM " + tableName;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, tableName);
            DataRow row = ds.Tables[tableName].NewRow();
            ds.Tables[tableName].Rows.Add(row);
            return row;
        }

        public DataRow Insert(DataRow row)
        {
            DataRow result = null;

            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            string tableName = row.Table.TableName;

            SqlCommand cmd = new SqlCommand();
            //cmd.Transaction = tran;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            cmd.CommandText = "Select * FROM " + tableName;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, row.Table.TableName);
            ds.Tables[row.Table.TableName].ImportRow(row);
            SqlCommandBuilder cb = new SqlCommandBuilder(da);
            cmd.ExecuteNonQuery();
            da.Update(ds, row.Table.TableName);

            da.Fill(ds, row.Table.TableName);
            result = ds.Tables[row.Table.TableName].Rows[ds.Tables[row.Table.TableName].Rows.Count - 1];

            return result;
        }

        public void Insert1(DataRow row, string tableName)
        {
            //DataRow result = null;

            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

             //= row.Table.TableName;

            SqlCommand cmd = new SqlCommand();
            //cmd.Transaction = tran;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            string query = "INSERT INTO "+tableName+" (";
            string como = "";
            foreach(DataColumn column in row.Table.Columns)
            {
                if (column.ColumnName != "ID")
                {
                    string columnName = "";
                    columnName = column.ColumnName;
                    if (como == "")
                    {
                        query = query + column;
                        como = ",";
                    }
                    else
                    {
                        query = query + como + column;
                    }
                }
            }
            query = query + ") Values (";
            string comoInsert = "";
            foreach (DataColumn column in row.Table.Columns)
            {
                double intValue;
                bool tryInt = double.TryParse(row[column].ToString(), out intValue);
                string nvarcharN = "N";

                if (column.ColumnName != "ID")
                {
                    if (comoInsert == "")
                    {
                        if (!string.IsNullOrEmpty(row[column].ToString()))
                            if(!tryInt)
                                query = query + nvarcharN +"'" +row[column].ToString() + "'";
                            else
                                query = query +  row[column].ToString();

                        else if (string.IsNullOrEmpty(row[column].ToString()))
                            query = query + " NULL ";
                        comoInsert = " , ";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(row[column].ToString()))
                        {
                            if(row[column].ToString() != "True" && row[column].ToString() != "False")
                                if (!tryInt)
                                    query = query + comoInsert + nvarcharN +"'" + row[column].ToString() + "'";
                                else
                                    query = query + comoInsert + row[column].ToString();

                            else if(row[column].ToString() == "True")
                                query = query + comoInsert + 1.ToString();

                            else if (row[column].ToString() == "False")
                                query = query + comoInsert + 0.ToString();
                        }
                        else if (string.IsNullOrEmpty(row[column].ToString()))
                            query = query + comoInsert + " NULL ";
                        //query = query + comoInsert + row[column];
                    }
                }
            }
            cmd.CommandText = query +")";
            cmd.ExecuteNonQuery();
        }

        public void Update(DataRow row, string tableName)
        {
            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            cmd.CommandText = "Select top 0 * FROM " + tableName;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            
            da.Fill(ds, tableName);
            ds.Tables[tableName].ImportRow(row);
            SqlCommandBuilder cb = new SqlCommandBuilder(da);
            //cmd.ExecuteNonQuery();

            da.Update(ds, tableName);
        }

        public DataSet TableDataSet(string tableName)
        {
            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            cmd.CommandText = "Select top 0 * FROM " + tableName;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            SqlCommandBuilder cb = new SqlCommandBuilder(da);
            da.Fill(ds,"tableName");
            return ds;
            
        }

        public void UpdateForDebug(DataRow row, string tableName,string[] columnsArray, DataRow currentRow)
        {
            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            string query = "UPDATE " + tableName + " SET ";
            string como = " , ";
            int count = 1;
            string queryFilter = "";
            foreach (DataColumn dataColumn in row.Table.Columns)
            {
                double intValue;
                bool tryInt = double.TryParse(row[dataColumn].ToString(), out intValue);
                string nvarcharN = "N";

                if (dataColumn.ColumnName != "ID")
                {
                    if (!string.IsNullOrEmpty(row[dataColumn].ToString()))
                    {
                        if (row[dataColumn].ToString() != "True" && row[dataColumn].ToString() != "False")
                            if (!tryInt)
                                query = query + dataColumn.ColumnName + " = " + nvarcharN + "'" + row[dataColumn].ToString() + "'";
                            else
                                query = query + dataColumn.ColumnName + " = " + row[dataColumn].ToString();

                        else if (row[dataColumn].ToString() == "True")
                            query = query + dataColumn.ColumnName + " = " + 1.ToString();
                        else if (row[dataColumn].ToString() == "False")
                            query = query + dataColumn.ColumnName + " = " + 0.ToString();
                    }
                    else if (string.IsNullOrEmpty(row[dataColumn].ToString()))
                    {
                        query = query + dataColumn.ColumnName + " = NULL ";
                    }

                    if (row.Table.Columns.Count  != count)
                        query = query + como;
                    count = count + 1;
                }
                else
                    count = count + 1;
            }
            if(columnsArray == null)
                query = query + "WHERE ID = " + row["ID"].ToString();
            else
            {
                count = 0;
                foreach(var item in columnsArray)
                {
                    if (count == 0)
                        queryFilter = string.Format(" WHERE {0} = "+currentRow[item].ToString() , item);
                    else
                        queryFilter = queryFilter+ string.Format(" And {0} = " + currentRow[item].ToString(), item);
                    count = count+1;
                }
                query = query + queryFilter;
            }
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
        }

        public void UpdateForDebug1(DataRow row,string tableName)
        {
            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            string query = "UPDATE "+ tableName + " SET ";
            string como = " , ";
            int count = 1;
            foreach (DataColumn dataColumn in row.Table.Columns)
            {
                double intValue;
                bool tryInt = double.TryParse(row[dataColumn].ToString(), out intValue);
                string nvarcharN = "N";

                if (dataColumn.ColumnName != "ID")
                {
                    if (!string.IsNullOrEmpty(row[dataColumn].ToString()))
                    {
                        if (row[dataColumn].ToString() != "True" && row[dataColumn].ToString() != "False")
                            if(!tryInt)
                                query = query + dataColumn.ColumnName + " = "+nvarcharN+"'" + row[dataColumn].ToString() + "'";
                            else
                                query = query + dataColumn.ColumnName + " = " + row[dataColumn].ToString();

                        else if (row[dataColumn].ToString() == "True")
                            query = query + dataColumn.ColumnName + " = " + 1.ToString();
                        else if (row[dataColumn].ToString() == "False")
                            query = query + dataColumn.ColumnName + " = " + 0.ToString();
                    }
                    else if (string.IsNullOrEmpty(row[dataColumn].ToString()))
                    {
                        query = query + dataColumn.ColumnName + " = NULL ";
                    }

                    if (row.Table.Columns.Count-1 != count)
                        query = query + como;
                    count = count + 1;
                }
            }

            cmd.CommandText = query + "WHERE ID = " + row["ID"].ToString();
            cmd.ExecuteNonQuery();
        }


        public void Delete(string tableName, string id, string[] columnsArray, DataRow currentRow)
        {
            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();
            string query = "";
            int count;
            string queryFilter = "";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            query = "DELETE FROM " + tableName;
            if (columnsArray == null)
                queryFilter = " WHERE ID = " + id;
            else
            {
                count = 0;
                foreach (var item in columnsArray)
                {
                    if (count == 0)
                        queryFilter = string.Format(" WHERE {0} = " + currentRow[item].ToString(), item);
                    else
                        queryFilter = queryFilter + string.Format(" And {0} = " + currentRow[item].ToString(), item);
                    count = count + 1;
                }
               
            }
            query = query + queryFilter;
            cmd.CommandText = query;
            cmd.Connection = Connection;
            cmd.ExecuteNonQuery();
        }

        public DataSet DsGr(DataTable tableName, DataTable tableName1)
        {
            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            cmd.CommandText = "Select * FROM " + tableName;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            ds.Tables.Add(tableName);
            ds.Tables.Add(tableName1);

            DataColumn dcparent = tableName.Columns["ID"];
            DataColumn dcChild = tableName1.Columns["ProceedingsID"];
            DataRelation dr = new DataRelation("dr", dcparent, dcChild);
            ds.Relations.Add(dr);
            return ds;
        }

        public DataSet DsGr(DataTable tableName, DataTable tableName1,string child)
        {
            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            if (Connection.State == ConnectionState.Closed)
                Connection.Open();
            tableName.TableName = "dbo.Patient_Reception";
            tableName1.TableName = "tableName1";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = Connection;
            cmd.CommandText = "Select * FROM " + tableName.TableName;
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();

            ds.Tables.Add(tableName);
            ds.Tables.Add(tableName1);
            
            DataColumn dcparent = ds.Tables[tableName.TableName].Columns["PatientID"];
            DataColumn dcChild = tableName1.Columns[child];
            DataRelation dr = new DataRelation("dr", dcparent, dcChild);
            ds.Relations.Add(dr);
            return ds;
        }



        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (this.Connection != null)
                    this.Connection.Dispose();
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }
    }
}
