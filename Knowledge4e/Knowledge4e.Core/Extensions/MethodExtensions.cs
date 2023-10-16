using Knowledge4e.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Knowledge4e.Core.Extensions
{
    public static class MethodExtensions
    {
        /// <summary>
        /// Lấy tên class
        /// </summary>
        /// <returns></returns>
        public static string GetTableName(this Type type)
        {
            var configTable = GetConfigTable(type);
            if (configTable == null)
            {
                if (string.IsNullOrWhiteSpace(type.Name))
                {
                    throw new ArgumentException($"{nameof(type)} không có tên table");
                }
            };
            return configTable.TableName;
        }

        /// <summary>
        /// Lấy trường unique trong table
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueColumns(this Type type)
        {
            var configTable = GetConfigTable(type);
            return configTable.UniqueColumns;
        }

        /// <summary>
        /// Lấy trường không đưa vào update
        /// </summary>
        /// <returns></returns>
        public static List<string> GetExcludeOnUpdateColumns(this Type type)
        {
            var columns = type.GetProperties().Where(f => f.IsDefined(typeof(IExcludeOnUpdate), true)).Select(f => f.Name);
            return columns.ToList();
        }

        /// <summary>
        /// Lấy tên trường hiển thị
        /// </summary>
        /// <returns></returns>
        public static string GetColumnDisplayName(this Type type, string name)
        {
            var obj = type.GetProperty(name).GetCustomAttributes(typeof(DisplayAttribute),
                                               false).Cast<DisplayAttribute>().SingleOrDefault();
            if(obj == null) return name;  

            return obj.Name;
        }

        /// <summary>
        /// Lấy trạng thái table có trường deleted không
        /// </summary>
        /// <returns></returns>
        public static bool GetHasDeletedColumn(this Type type)
        {
            var configTable = GetConfigTable(type);
            return configTable.HasDeletedColumn;
        }

        /// <summary>
        /// Lấy config table
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ConfigTables GetConfigTable(this Type type)
        {
            var configTable = type.GetCustomAttributes(typeof(ConfigTables), true).FirstOrDefault() as ConfigTables;
            
            if(configTable == null)
            {
                configTable = new ConfigTables();
            }

            return configTable;
        }

        /// <summary>
        /// Lấy giá trị khóa chính
        /// </summary>
        /// <returns></returns>
        public static object GetKeyValue(this Type type, object data)
        {
            var propeties = type.GetProperties();
            var key = propeties.FirstOrDefault(f => f.IsDefined(typeof(KeyAttribute), true));
            if (key == null) return null;
            return key.GetValue(data);
        }

        /// <summary>
        /// Lấy tên khóa chính
        /// </summary>
        /// <returns></returns>
        public static string GetKeyName(this Type type)
        {
            var propeties = type.GetProperties();
            var key = propeties.FirstOrDefault(f => f.IsDefined(typeof(KeyAttribute), true));
           
            if (key == null)
            {
                throw new ArgumentException($"{type.GetTableName()} Không có primarykey");
            };

            return key.Name;
        }

        /// <summary>
        /// Lấy danh sách tên cột trong đối tượng
        /// </summary>
        /// <returns></returns>
        public static List<string> GetColumNames(this Type type)
        {
            var propeties = type.GetProperties().Where(f => !f.IsDefined(typeof(IExclude), true)).Select(f => f.Name);

            return propeties.ToList();
        }

        /// <summary>
        /// Lấy danh sách tên cột mapping với database
        /// </summary>
        /// <returns></returns>
        public static List<string> GetColumNameDatabases(this Type type)
        {
            var propeties = type.GetProperties().Where(f => !f.IsDefined(typeof(IExclude),true) && f.IsDefined(typeof(ColumnNameAttribute),true)).Select(f => f.GetCustomAttribute<ColumnNameAttribute>().ColumnName);

            return propeties.ToList();
        }

        /// <summary>
        /// Lấy giá trị theo tên
        /// </summary>
        /// <returns></returns>
        public static object GetValueByFieldName(this Type type, object data, string name)
        {
            var property = type.GetProperties().FirstOrDefault(f => f.Name == name);
            if (property == null) return null;
            return property.GetValue(data);
        }


        /// <summary>
        /// Lấy các trường exclude
        /// </summary>
        /// <returns></returns>
        public static List<string> GetExcludeColumnNames(this Type type)
        {
            var properties = type.GetProperties().Where(f => f.IsDefined(typeof(IExclude), true)).Select(f => f.Name);
            return properties.ToList();
        }

        /// <summary>
        /// Merge 2 dictionart
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
        this Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            foreach (var item in dict2)
            {
                if (dict1.ContainsKey(item.Key))
                {
                    dict1[item.Key] = item.Value;
                }
                else
                {
                    dict1.Add(item.Key, item.Value);
                }
            }

            return dict1;
        }
    }
}
