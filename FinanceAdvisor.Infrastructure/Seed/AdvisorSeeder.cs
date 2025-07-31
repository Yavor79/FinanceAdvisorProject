using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Data.Seeding.DataTransferObjects;
using FinanceAdvisor.Infrastructure.Seed;

public class AdvisorSeeder : BaseSeeder<Advisor, ImportAdvisorDto>
{
    protected override string JsonFilePath => @"C:\Users\USER\Programming\C_Sharp\Finance_Project\FinanceAdvisor\FinanceAdvisor.Infrastructure\Seed\application-advisors.json";

    protected override bool EntityExists(Advisor existingEntity, ImportAdvisorDto newDto)
    {

        return existingEntity.AdvisorId.ToString() == newDto.AdvisorId.ToString();
    }

    protected override Func<ImportAdvisorDto, bool>? CustomDtoValidation => (dto) =>
    {
        if (dto.AdvisorId == Guid.Empty)
            return false;

        return true;
    };

    // Example of overriding post-processing if needed
    //protected override async Task<Advisor> PostProcessEntityAsync(Advisor entity, ImportAdvisorDto dto)
    //{
    //    // You could add special handling here, e.g., initialize collections or fetch related entities

    //    // For example, set CreatedAt if missing
    //    if (entity.CreatedAt == default)
    //        entity.CreatedAt = DateTime.UtcNow;

    //    return await base.PostProcessEntityAsync(entity, dto);
    //}
}
