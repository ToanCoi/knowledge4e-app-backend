using Knowledge4e.Core.Attributes;
using Knowledge4e.Core.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knowledge4e.Core.Entities.Account
{
    [ConfigTables(tableName: "account", HasDeletedColumn = true, UniqueColumns = "UserName")]
    public class Account : BaseEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public Account() 
        {
            this.UserName = "";
            this.Password = "";
        }
    }
}
