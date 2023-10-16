using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace Knowledge4e.Core.Extensions
{
    public class ConfigTables : Attribute
    {
        public bool HasDeletedColumn { get; set; } = false;

        public string UniqueColumns { get; set; } = string.Empty;

        public string TableName { get; set; } = string.Empty;

        public ConfigTables(string tableName = "" , bool hasDeletedColumn = false, string uniqueColumns = "")
        {
            TableName = tableName;

            HasDeletedColumn = hasDeletedColumn;

            UniqueColumns = uniqueColumns;
        }
    }
}
