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

        Task<CreditConsultationCycleDto?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(CreateCreditConsultationCycleDto dto);
        Task<bool> UpdateAsync(UpdateCreditConsultationCycleDto dto);
        Task<bool> DeleteSelfAsync(Guid id, Guid advisorId);
        Task<int> CountByAdvisorAsync(Guid advisorId);
        Task<IEnumerable<CreditConsultationCycleDto>> GetPendingCyclesAsync();

    }
}
