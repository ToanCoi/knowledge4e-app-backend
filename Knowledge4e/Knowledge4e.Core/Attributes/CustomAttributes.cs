using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knowledge4e.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ColumnNameAttribute : Attribute
    {
        public string ColumnName { get; }

        public ColumnNameAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IRequired : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IDuplicate : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IExcludeOnUpdate : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IPrimaryKey : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IEmailFormat : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IExclude : Attribute
    {

    }
}
