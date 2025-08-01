using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Infrastructure.Seed.DataTransferObjects;
using FinanceAdvisor.Infrastructure.Seed.Seeders;

public class CreditConsultationCycleSeeder : BaseSeeder<CreditConsultationCycle, ImportCreditConsultationCycleDto>
{
    protected override string JsonFilePath =>
        @"C:\Users\USER\Programming\C_Sharp\Finance_Project\FinanceAdvisor\FinanceAdvisor.Infrastructure\Seed\seedData\application-creaditConsultationCycles.json";

    // Check if entity already exists by comparing IDs (since Id is Guid primary key)
    protected override bool EntityExists(CreditConsultationCycle existingEntity, ImportCreditConsultationCycleDto newDto)
    {
        return existingEntity.Id == newDto.Id;
    }

    // Optionally override PostProcessEntityAsync if you need to do extra mapping
    // e.g., initialize collections or set default values.
    // Otherwise base implementation is fine.
}
