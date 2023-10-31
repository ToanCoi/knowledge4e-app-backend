using Knowledge4e.Core.Entities;
using Knowledge4e.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knowledge4e.Entities.Entities.Group
{
    [ConfigTables(tableName: "room", HasDeletedColumn = true, UniqueColumns = "RoomName")]
    public class KGroup : BaseEntity
    {
        public string TopicID { get; set; }
        public string RoomName { get; set; }
        public int MemberLimited { get; set; }
        public int CreaterID { get; set; }
    }
}
