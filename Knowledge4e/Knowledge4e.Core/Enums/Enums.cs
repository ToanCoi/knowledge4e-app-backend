using System;
using System.Collections.Generic;
using System.Text;

namespace Knowledge4e.Core.Enums
{
    public enum Enums
    {
        /// <summary>
        /// Hợp lệ
        /// </summary>
        Valid = 100,

        /// <summary>
        /// Không hợp lệ
        /// </summary>
        InValid = 200,

        /// <summary>
        /// Thành công  
        /// </summary>
        Success = 900,

        /// <summary>
        /// Thất bại  
        /// </summary>
        Fail = 700,

        /// <summary>
        /// Không tồn tại  
        /// </summary>
        NotFound = 701,

        /// <summary>
        /// Lỗi exception
        /// </summary>
        Exception = 500,

        /// <summary>
        /// Trùng
        /// </summary>
        Duplicate = 600
    }
}
