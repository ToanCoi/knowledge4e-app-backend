using Dapper;
using Knowledge4e.Core.Entities;
using Knowledge4e.Core.Entities.Account;
using Knowledge4e.Core.Extensions;
using Knowledge4e.Entities.Entities.Group;
using Knowledge4e.Infarstructure.Repositories.BaseRepository;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Knowledge4e.Infarstructure.Repositories
{
    public class GroupRepository : BaseRepository<KGroup>, IGroupRepository
    {
        public GroupRepository(IConfiguration configuration) : base(configuration)
        {

        }
    }
}