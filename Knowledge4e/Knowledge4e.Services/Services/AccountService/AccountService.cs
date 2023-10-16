using Knowledge4e.Core.Entities.Account;
using Knowledge4e.Infarstructure.Repositories;

namespace Knowledge4e.Core.Services.BaseService
{
    public class AccountService : BaseService<Account>, IAccountService
    {
        IAccountRepository _repository;
        public AccountService(IAccountRepository repository) : base(repository)
        {
            _repository = repository;
        }
    }
}
