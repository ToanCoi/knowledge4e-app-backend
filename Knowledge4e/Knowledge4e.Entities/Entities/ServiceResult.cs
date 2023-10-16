using System;
using System.Collections.Generic;
using System.Text;

namespace Knowledge4e.Core.Entities
{
    public class ServiceResult
    {
        //Lưu dữ liệu được trả về, bao gồm cả câu thông báo
        public object Data { get; set; }

        //Lưu câu thông báo
        public string Messasge { get; set; }

        //Lưu mã lỗi
        public Enums.Enums Code { get; set; }

        //Có thành công không
        public bool IsSuccess { get; set; } = true;

        //Thành công
        public void onSuccess(object _data, string _message = "", Enums.Enums _code = Enums.Enums.Valid)
        {
            IsSuccess = true;
            Data = _data;
            Messasge = _message;
            Code = _code;
        }

        //Thất bại
        public void onError(object _data, string _message = "", Enums.Enums _code = Enums.Enums.InValid)
        {
            IsSuccess = false;
            Data = _data;
            Messasge = _message;
            Code = _code;
        }
    }
}
