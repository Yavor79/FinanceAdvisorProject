using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Data.Seeding.DataTransferObjects;
using FinanceAdvisor.Infrastructure.Seed;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FinanceAdvisor.Infrastructure.Identity;

namespace CinemaApp.Data.Seeding
{
    public class ApplicationUserSeeder : BaseSeeder<ApplicationUser, ImportApplicationUserDto>
    {
        protected override string JsonFilePath => @"C:\Users\USER\Programming\C_Sharp\Finance_Project\FinanceAdvisor\FinanceAdvisor.Infrastructure\Seed\application-creaditConsultationCycles.json";

        // Override to check if the user already exists by matching GUIDs parsed from string Ids
        protected override bool EntityExists(ApplicationUser existingEntity, ImportApplicationUserDto newDto)
        {
            
            return existingEntity.Id == newDto.Id;
        }

        // Override DTO validation to include GUID validation on Id string property
        protected override bool IsValidDto(ImportApplicationUserDto dto)
        {
            if (dto.Id == Guid.Empty)
                return false;

            return true;
        }
    }
}
