using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wUtility.Utils;

namespace wUtility.Models
{
    class Tbl_dbConfigSetting
    {
        public int fldID { get; set; }
        public string fldMainDataBase { get; set; }
        public string fldDataBaseName { get; set; }
        public string fldDelphiConnectionName { get; set; }
        public string fldPersianDescDataBase { get; set; }
        public bool fldIsActive { get; set; }
        public string fldInstanceName { get; set; }
        public string fldSecondInstance { get; set; }

        public static DataTable GetdbConfigSettings()
        {
            using (DBHelper dbh = new DBHelper(dbConfigDataBases.dbConfigDataBasesConnectionString))
            {
                string query = @"SELECT         *
                                FROM          Tbl_dbConfigSetting 
                                                ";
                DataTable DatabasesDataTable = dbh.ExecuteQuery(query);
                return DatabasesDataTable;
            }
        }
    }
}
