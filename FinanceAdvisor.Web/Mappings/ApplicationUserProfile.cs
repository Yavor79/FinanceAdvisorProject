using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Web.Models;

namespace FinanceAdvisor.Web.Mappings
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUserDto, ApplicationUserViewModel>();
        }
    }
}
