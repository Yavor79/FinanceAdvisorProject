using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Web.Models;

namespace FinanceAdvisor.Web.Mappings
{
    public class ConsultationProfile : Profile
    {
        public ConsultationProfile()
        {
            CreateMap<ConsultationDto, ConsultationViewModel>();
            CreateMap<ConsultationDto, UpdateConsultationViewModel>();
            CreateMap<UpdateConsultationViewModel, UpdateConsultationDto>();
            CreateMap<CreateConsultationViewModel, ConsultationDto>();
            CreateMap<CreateConsultationViewModel, CreateConsultationDto>();

        }
    }
}
