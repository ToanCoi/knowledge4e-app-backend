namespace Knowledge4e.Core.Entities
{
    public class PagingResponse
    {
        public int TotalRecord { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public object PageData { get; set; }
    }
}
