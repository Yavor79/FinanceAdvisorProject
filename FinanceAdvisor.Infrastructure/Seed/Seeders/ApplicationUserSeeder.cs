using FinanceAdvisor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FinanceAdvisor.Infrastructure.Seed.DataTransferObjects;
using FinanceAdvisor.Common.Utilities;


namespace FinanceAdvisor.Infrastructure.Seed.Seeders
{
    public class ApplicationUserSeeder : BaseSeeder<User, ImportApplicationUserDto>
    {
        protected override string JsonFilePath
            => FileLocator.FindJsonFile("application-users.json")
               ?? throw new FileNotFoundException("Seed file 'application-users.json' not found.");

        // Override to check if the user already exists by matching GUIDs parsed from string Ids
        protected override bool EntityExists(User existingEntity, ImportApplicationUserDto newDto)
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
