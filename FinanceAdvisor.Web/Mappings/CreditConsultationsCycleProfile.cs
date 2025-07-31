using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Web.Models;

namespace FinanceAdvisor.Web.Mappings
{
    public class CreditConsultationsCycleProfile : Profile
    {
        public CreditConsultationsCycleProfile()
        {
            CreateMap<CreditConsultationCycleDto, CreditConsultationCycleViewModel>();
        }
    }
}
