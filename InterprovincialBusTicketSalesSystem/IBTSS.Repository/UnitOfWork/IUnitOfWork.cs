using IBTSS.Repository.Repositories.CustomerRepository;
using IBTSS.Repository.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICustomerRepository Customers { get; }
        IUserRepository Users { get; }
        Task<int> CompleteAsync();
    }
}
