using Knowledge4e.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Knowledge4e.ApplicationCore.Entities
{
    public class ServiceResult
    {
        //Lưu dữ liệu được trả về, bao gồm cả câu thông báo
        public object Data { get; set; }

        //Lưu câu thông báo
        public string Messasge { get; set; }

        //Lưu mã lỗi
        public Knowledge4e.Entities.Enums Code { get; set; }

        //Có thành công không
        public bool IsSuccess { get; set; } = true;

        //Thành công
        public void onSuccess(object? _data, string? _message, Knowledge4e.Entities.Enums _code = Knowledge4e.Entities.Enums.Valid)
        {
            IsSuccess = true;
            if (_data != null) Data = _data;
            if (_message != null) Messasge = _message;
            Code = _code;
        }

        //Thất bại
        public void onError(object? _data, string? _message, Knowledge4e.Entities.Enums _code = Knowledge4e.Entities.Enums.InValid)
        {
            IsSuccess = false;
            if (_data != null) Data = _data;
            if (_message != null) Messasge = _message;
            Code = _code;
        }
    }
}
