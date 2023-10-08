using Knowledge4e.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Knowledge4e.ApplicationCore.Entities
{
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

    public class BaseEntity
    {
        /// <summary>
        /// Trạng thái của Entity
        /// </summary>
        [IExclude]
        public EntityState EntityState { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        [IExcludeOnUpdate]
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Người tạo
        /// </summary>
        [IExcludeOnUpdate]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Người sửa
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Trạng thái insert của entity
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// Có xóa mềm không
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}
