using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Infrastructure.Seed.DataTransferObjects;
//using FinanceAdvisor.Infrastructure.Identity;

public static class AutoMapperConfig
{
    // static AutoMapperConfig class
    // BC the seeding code is often run early in the app lifecycle —
    // sometimes before the full dependency injection container is built or fully configured.
    public static IMapper MapperInstance { get; private set; }

    public static void Initialize()
    {
        var config = new MapperConfiguration(cfg =>
        {
            // You can call CreateMappings on DTOs here, or add profiles
            var userDto = new ImportApplicationUserDto();
            userDto.CreateMappings(cfg);

            var advisorDto = new ImportAdvisorDto();
            advisorDto.CreateMappings(cfg);

            var cDto = new ImportCreditConsultationCycleDto();
            cDto.CreateMappings(cfg);

            var consultationDto = new ImportConsultationDto();
            consultationDto.CreateMappings(cfg);

            var meetingDto = new ImportMeetingDto();
            meetingDto.CreateMappings(cfg);
        });

       
        //config.AssertConfigurationIsValid();

        MapperInstance = config.CreateMapper();
    }
}
