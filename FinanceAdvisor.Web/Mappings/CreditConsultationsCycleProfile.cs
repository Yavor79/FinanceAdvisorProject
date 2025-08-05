using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Web.Models;

namespace FinanceAdvisor.Web.Mappings
{
    public class CreditConsultationsCycleProfile : Profile
    {
        public CreditConsultationsCycleProfile()
        {
            // View / display
            CreateMap<CreditConsultationCycleDto, CreditConsultationCycleViewModel>();
            CreateMap<CreditConsultationCycleViewModel, CreditConsultationCycleDto>();

            // Create
            CreateMap<CreateCreditConsultationCycleViewModel, CreateCreditConsultationCycleDto>();
            CreateMap<CreateCreditConsultationCycleDto, CreateCreditConsultationCycleViewModel>();

            // Update
            CreateMap<UpdateCreditConsultationCycleViewModel, UpdateCreditConsultationCycleDto>();
            CreateMap<UpdateCreditConsultationCycleDto, CreditConsultationCycleViewModel>();
            CreateMap<CreditConsultationCycleDto, UpdateCreditConsultationCycleViewModel>();
            CreateMap<UpdateCreditConsultationCycleViewModel, CreditConsultationCycleDto>();
        }
    }
}
