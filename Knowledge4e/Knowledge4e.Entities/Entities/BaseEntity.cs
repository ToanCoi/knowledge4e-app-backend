using Knowledge4e.Core.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knowledge4e.Core.Entities
{

    public class BaseEntity
    {
        /// <summary>
        /// ID của bản ghi
        /// </summary>
        [Key]
        [IExcludeOnUpdate]
        public int ID { get; set; }

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
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Người sửa
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Có xóa mềm không
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }

    public enum EntityState
    {
        Add = 1,
        Update = 2,
        Delete = 3
    }
}
