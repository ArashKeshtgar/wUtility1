﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using wUtility.Utils;

namespace wUtility
{
    class dbConfigDataBases
    {



        public static string dbConfigDataBasesConnectionString
        {
            get
            {
                string encryptedValue = AppSettingsManager.GetSetting("dbConfigDataBasesConnectionString");
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
                AppSettingsManager.SetSetting("dbConfigDataBasesConnectionString", encryptedValue);
            }
        }
    }
}
