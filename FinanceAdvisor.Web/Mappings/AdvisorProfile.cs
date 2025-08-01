using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Web.Models;

namespace FinanceAdvisor.Web.Mappings
{
    public class AdvisorProfile : Profile
    {
        public AdvisorProfile()
        {
            CreateMap<AdvisorDto, AdvisorViewModel>();
        }
    }
}
