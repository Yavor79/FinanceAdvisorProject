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
        }
    }
}
