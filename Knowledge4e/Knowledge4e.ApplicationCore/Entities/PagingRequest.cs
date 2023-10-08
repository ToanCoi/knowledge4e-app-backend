using System;

namespace Knowledge4e.ApplicationCore.Entities
{
    public class PagingRequest
    {
        public string Sort { get; set; }

        public string Columns { get; set; }

        public string Filter { get; set; }

        public int Delta { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public CustomParams CustomParams { get; set; }
    }

    public class CustomParams
    {

        public string FilterFolder { get; set; }

        public bool IsGetFolder { get; set; }

        public bool IsReport { get; set; }

        public bool IsSearch { get; set; }

        public Guid ParentFolderID { get; set; }

        public string SortFolder { get; set; }

    }
}