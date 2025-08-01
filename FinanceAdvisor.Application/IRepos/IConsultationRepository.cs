using FinanceAdvisor.Domain.IRepositories;
using FinanceAdvisor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceAdvisor.Application.IRepos
{
    public interface IConsultationRepository : IRepository<Consultation, Guid>
    {
    }
}
