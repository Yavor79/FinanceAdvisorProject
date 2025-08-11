using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Web.Models;

namespace FinanceAdvisor.Web.Mappings
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            // View only
            CreateMap<ApplicationUserDto, ApplicationUserViewModel>();
            CreateMap<ApplicationUserDto, ChooseUserViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));


            // Create
            CreateMap<CreateApplicationUserViewModel, ApplicationUserDto>();
            CreateMap<ApplicationUserDto, CreateApplicationUserViewModel>();

            // Update
            CreateMap<UpdateApplicationUserViewModel, ApplicationUserDto>();
            CreateMap<ApplicationUserDto, UpdateApplicationUserViewModel>();

            //CreateMap<ApplicationUser, ApplicationUserDto>()
            //    .ForMember(dest => dest.Roles, opt =>
            //        opt.MapFrom(src => src.Roles.Select(r => r.ToString()).ToList()));

            //CreateMap<ApplicationUserDto, ApplicationUser>()
            //    .ForMember(dest => dest.Roles, opt =>
            //        opt.MapFrom(src => src.Roles.Select(r => Enum.Parse<UserRole>(r))));

        }
    }
}
