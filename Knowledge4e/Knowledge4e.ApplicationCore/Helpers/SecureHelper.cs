using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Knowledge4e.ApplicationCore.Helpers
{
    public static class SecureHelper
    {
        #region Fields
        public static readonly Regex regSystemThreats = new Regex("\\s?,\\s?|\\s?;\\s?\\s?select\\s+\\S?drop\\s+\\S|\\s?grant\\s+\\S|^'|\\s?--\\s?union\\s+\\S?delete\\s+|\\S|\\s?update\\s+\\S|\\s?truncate\\s+\\S?sysobjects\\s?|\\s?xp_.*?|\\s?syslogins\\s?|\\s?sysremote\\s?|\\s?sysusers\\s?|\\s?sysxlogins\\s?|\\s?sysdatabases\\s?|\\s?aspnet_.*?|\\s?exec\\s+|\\s?execute\\s+\\S", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static readonly Regex objectName = new Regex("^[a-zA-Z_]*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion

        #region Methods

        /// <summary>
        /// Phát hiện sqlinjection
        /// </summary>
        /// <param name="inputSql"></param>
        /// <returns></returns>ư
        public static bool IsDetectedSqlInjection(string inputSql)
        {
            return !string.IsNullOrWhiteSpace(inputSql) && SecureHelper.regSystemThreats.IsMatch(inputSql);
        }

        public static string SafeSqlLiteralForStringValue(string inputSql)
        {
            if (string.IsNullOrEmpty(inputSql))
            {
                return inputSql;
            }
            return inputSql.Replace("'", "''").Replace("-- ", "").Replace("#", "");
        }

        public static string SafeSqlLiteralForObjectName(string tableViewColumnName)
        {
            var isValid = !string.IsNullOrEmpty(tableViewColumnName) && SecureHelper.objectName.IsMatch(tableViewColumnName);
            if(!isValid)
            {
                throw new FormatException();
            }
            return tableViewColumnName.Replace("`", "``").Replace("'", "''").Replace("-- ", "").Replace("#", "");
        }

        public static string SafeSqlLiteralForColumnsName(string columnsName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if(!string.IsNullOrEmpty(columnsName))
            {
                string[] array = columnsName.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if(array != null && array.Length > 0)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        string text = array[i].Trim();
                        if (text.CompareTo("*") != 0 && (!text.StartsWith('`') || !text.EndsWith('`'))) 
                        {
                            text = "`" + SafeSqlLiteralForObjectName(text) + "`";
                        }

                        if (!string.IsNullOrEmpty(text) && stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(",");
                        }

                        stringBuilder.Append(text);
                    }

                }
            }
            return stringBuilder.ToString();
        }

        #endregion
    }
}
