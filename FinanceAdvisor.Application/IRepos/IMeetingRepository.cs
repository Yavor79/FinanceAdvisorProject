using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceAdvisor.Application.IRepos
{
    public interface IMeetingRepository : IRepository<Meeting, Guid>
    {
    }
}
