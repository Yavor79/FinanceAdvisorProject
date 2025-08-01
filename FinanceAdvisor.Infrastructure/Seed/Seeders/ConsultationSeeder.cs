using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Infrastructure.Seed;
using FinanceAdvisor.Infrastructure.Seed.DataTransferObjects;
using System;
using FinanceAdvisor.Infrastructure.Seed.Seeders;

public class ConsultationSeeder : BaseSeeder<Consultation, ImportConsultationDto>
{
    protected override string JsonFilePath =>
        @"C:\Users\USER\Programming\C_Sharp\Finance_Project\FinanceAdvisor\FinanceAdvisor.Infrastructure\Seed\seedData\application-consultation.json";

    protected override bool EntityExists(Consultation existingEntity, ImportConsultationDto newDto)
    {
        return existingEntity.ConsultationId == newDto.ConsultationId;
    }
}
