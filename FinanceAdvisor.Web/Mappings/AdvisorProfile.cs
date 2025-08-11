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
            CreateMap<AdvisorDto, ChooseAdvisorViewModel>();
            CreateMap<AdvisorViewModel, AdvisorDto>();
            CreateMap<CreateApplicationUserViewModel, AdvisorDto>();
            CreateMap<AdvisorDto, CreateApplicationUserViewModel>();
            CreateMap<UpdateAdvisorViewModel, AdvisorDto>();
            CreateMap<AdvisorDto, UpdateAdvisorViewModel>();
        }
    }
}
