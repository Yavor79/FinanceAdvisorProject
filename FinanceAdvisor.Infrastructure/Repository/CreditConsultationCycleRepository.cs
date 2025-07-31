using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Application.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceAdvisor.Infrastructure.Repository
{
    public class CreditConsultationCycleRepository : BaseRepository<CreditConsultationCycle, Guid>, ICreditConsultationCycleRepository
    {
        public CreditConsultationCycleRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
