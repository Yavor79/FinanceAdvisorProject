using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Infrastructure.Seed;
using FinanceAdvisor.Infrastructure.Seed.DataTransferObjects;
using System;
using FinanceAdvisor.Infrastructure.Seed.Seeders;
using FinanceAdvisor.Common.Utilities;

public class ConsultationSeeder : BaseSeeder<Consultation, ImportConsultationDto>
{
    protected override string JsonFilePath
                => FileLocator.FindJsonFile("application-consultation.json")
                   ?? throw new FileNotFoundException("Seed file 'application-consultation.json' not found.");

    protected override bool EntityExists(Consultation existingEntity, ImportConsultationDto newDto)
    {
        return existingEntity.ConsultationId == newDto.ConsultationId;
    }
}
