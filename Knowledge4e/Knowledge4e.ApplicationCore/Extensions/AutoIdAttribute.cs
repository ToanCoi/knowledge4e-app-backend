using System;
using System.Collections.Generic;
using System.Text;

namespace Knowledge4e.Domain.Shared.Attributes
{
    /// <summary>
    /// Attribute đánh dấu thuộc tính là mã của đối tượng
    /// xử lý lấy mới giá trị theo autoid khi thêm/nhân bản
    /// </summary>
    public class AutoIdAttribute : Attribute
    {
        /// <summary>
        /// Loại chứng từ/danh mục
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// nếu mà người dùng nhập chỉ có prefix mà k có value (ví vụ SBB) thì dự vào biến này để sinh mã tự tăng và độ dài value là theo cấu hình truyền vào
        /// muốn 5 chữ số thì lần sau sẽ là SBB00001
        /// muốn 2 chữ số thì lần sau là SBB01
        /// mặc định lengthvalue = 0
        /// </summary>
        public int LengthValue { get; set; } = 0;

        public AutoIdAttribute(string categoryName, int lengthValue = 0)
        {
            this.CategoryName = categoryName;
            this.LengthValue = lengthValue;
        }
    }
}
