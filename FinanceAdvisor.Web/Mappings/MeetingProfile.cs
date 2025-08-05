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
            CreateMap<MeetingViewModel, MeetingDto>();
            CreateMap<CreateMeetingViewModel, CreateMeetingDto>();
            CreateMap<CreateMeetingDto, CreateMeetingViewModel>();
            CreateMap<UpdateMeetingViewModel, UpdateMeetingDto>();
            CreateMap<UpdateMeetingDto, UpdateMeetingViewModel>();
        }
    }
}
