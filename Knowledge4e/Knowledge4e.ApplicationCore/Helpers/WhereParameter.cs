using System;
using System.Collections.Generic;
using System.Text;

namespace Knowledge4e.ApplicationCore.Helpers
{
    public class WhereParameter
    {

        #region Fields
        private string _whereClause;

        private Dictionary<string, object> _whereValues = new Dictionary<string, object>();
        #endregion

        #region Properties

        public string WhereClause { get { return this._whereClause; } }

        public Dictionary<string, object> WhereValues { get { return this._whereValues; } }
        #endregion

        #region Methods
        public void AddWhere(string stringWhereClause, Dictionary<string, object> dicWhereValues)
        {
            StringBuilder stringBuilder = new StringBuilder(stringWhereClause);
            this._whereClause = $"{this._whereClause} AND ({stringBuilder.ToString()})";

            if (dicWhereValues != null && dicWhereValues.Count > 0)
            {
                foreach (KeyValuePair<string, object> current in dicWhereValues)
                {
                    string key = current.Key;

                    if (this._whereValues.ContainsKey(key))
                    {
                        this._whereValues[key] = current.Value;
                    }
                    else
                    {
                        this._whereValues.Add(key, current.Value);
                    }
                }
            }
        }

        public void AddWhere(WhereParameter whereParameter)
        {
            if (whereParameter != null)
            {
                this.AddWhere(whereParameter.WhereClause, whereParameter.WhereValues);
            }
        }

        /// <summary>
        /// Compile ra chuỗi sau khi đã convert các kiểu dữ liệu của value
        /// </summary>
        /// <param name="whereParameter"></param>
        /// <returns></returns>
        public static string Compile(WhereParameter whereParameter)
        {
            if(whereParameter != null && !string.IsNullOrWhiteSpace(whereParameter.WhereClause))
            {
                var stringBuilder = new StringBuilder(whereParameter.WhereClause);
                var keys = new List<string>();

                if (whereParameter.WhereValues != null && whereParameter.WhereValues.Count > 0)
                {
                    keys.AddRange(whereParameter.WhereValues.Keys);
                    keys.Sort();
                }

                for (int i = keys.Count - 1; i >= 0; i--)
                {
                    object value = whereParameter.WhereValues[keys[i]];
                    if (value.GetType() == typeof(string))
                    {
                        value = $"'{SecureHelper.SafeSqlLiteralForStringValue(value.ToString())}'";
                    }
                    else if (value.GetType() == typeof(TimeSpan))
                    {
                        value = $"'{((TimeSpan)value).ToString("HH:mm:ss")}'";
                    }
                    else if (value.GetType() == typeof(DateTime))
                    {
                        value = $"'{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}'";
                    }
                    else if (value.GetType() == typeof(int) || value.GetType() == typeof(decimal) || value.GetType() == typeof(double))
                    {
                        value = value.ToString();
                    }
                    else if (value.GetType() == typeof(bool))
                    {
                        value = ((bool)value) ? "1" : "0";
                    }
                    else if (value.GetType() == typeof(Guid))
                    {
                        value = $"'{((Guid)value).ToString()}'";
                    }

                    stringBuilder.Replace(keys[i], value.ToString());
                }

                return stringBuilder.ToString();
            }
           

            return string.Empty;
        }
        #endregion
    }
}
