using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Web.Models;

namespace FinanceAdvisor.Web.Mappings
{
    public class MeetingProfile : Profile
    {
        public MeetingProfile()
        {
            CreateMap<MeetingDto, MeetingViewModel>();
        }
    }
}
