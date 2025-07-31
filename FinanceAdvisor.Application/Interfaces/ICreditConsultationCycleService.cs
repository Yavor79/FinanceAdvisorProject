using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceAdvisor.Application.Interfaces
{
    public interface ICreditConsultationCycleService
    {
        Task<IEnumerable<CreditConsultationCycleDto>> GetAllAsync();
        Task<IEnumerable<CreditConsultationCycleDto>> GetAllByClientIdAsync(Guid? Id);
        Task<IEnumerable<CreditConsultationCycleDto>> GetAllByManagerIdAsync(Guid? managerId);
    }
}
