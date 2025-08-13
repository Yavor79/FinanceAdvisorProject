using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceAdvisor.Common.Utilities;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Infrastructure.Seed.DataTransferObjects;
using FinanceAdvisor.Infrastructure.Seed.Seeders;

public class CreditConsultationCycleSeeder : BaseSeeder<CreditConsultationCycle, ImportCreditConsultationCycleDto>
{
    protected override string JsonFilePath
            => FileLocator.FindJsonFile("application-creaditConsultationCycles.json")
               ?? throw new FileNotFoundException("Seed file 'application-creaditConsultationCycles.json' not found.");

    // Check if entity already exists by comparing IDs (since Id is Guid primary key)
    protected override bool EntityExists(CreditConsultationCycle existingEntity, ImportCreditConsultationCycleDto newDto)
    {
        return existingEntity.Id == newDto.Id;
    }

    // Optionally override PostProcessEntityAsync if you need to do extra mapping
    // e.g., initialize collections or set default values.
    // Otherwise base implementation is fine.
}
