using Knowledge4e.Core.Entities.Account;
using Knowledge4e.Entities.Entities.Group;
using Knowledge4e.Infarstructure.Repositories;

namespace Knowledge4e.Core.Services.BaseService
{
    public class GroupService : BaseService<KGroup>, IGroupService
    {
        IGroupRepository _repository;
        public GroupService(IGroupRepository repository) : base(repository)
        {
            _repository = repository;
        }
    }
}
