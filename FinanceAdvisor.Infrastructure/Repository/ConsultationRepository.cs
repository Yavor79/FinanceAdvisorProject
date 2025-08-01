using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Application.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceAdvisor.Infrastructure.Repository
{
    public class ConsultationRepository : BaseRepository<Consultation, Guid>, IConsultationRepository
    {
        public ConsultationRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
