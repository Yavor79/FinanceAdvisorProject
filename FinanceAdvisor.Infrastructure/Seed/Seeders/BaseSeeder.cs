using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceAdvisor.Infrastructure.Seed.Seeders
{
    public abstract class BaseSeeder<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {

        // Optional validation delegate, can be overridden by subclasses or set externally
        protected virtual Func<TDto, bool>? CustomDtoValidation => null;
        protected abstract string JsonFilePath { get; }
        protected FinanceDbContext _dbContext;

        // Query existing entities for deduplication
        protected async Task<List<TEntity>> GetExistingEntitiesAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        // Override to check if entity already exists in DB (deduplication logic)
        protected abstract bool EntityExists(TEntity existingEntity, TDto newDto);

        // Optional validation hook for DTO
        protected virtual bool IsValidDto(TDto dto)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(dto);
            return Validator.TryValidateObject(dto, context, validationResults, true);
        }

        // Hook to modify entity after mapping (e.g., additional setup)
        protected virtual Task<TEntity> PostProcessEntityAsync(TEntity entity, TDto dto)
        {
            return Task.FromResult(entity);
        }

        public async Task SeedAsync(IServiceProvider services)
        {
            _dbContext = services.GetRequiredService<FinanceDbContext>();
            var existingEntities = await GetExistingEntitiesAsync();

            string jsonInput = await File.ReadAllTextAsync(JsonFilePath, Encoding.UTF8);
            var dtos = JsonConvert.DeserializeObject<List<TDto>>(jsonInput);

            if (dtos == null)
                return;

            var newEntitiesToAdd = new List<TEntity>();

            foreach (var dto in dtos)
            {
                if (!IsValidDto(dto))
                    continue;

                if (CustomDtoValidation != null && !CustomDtoValidation(dto))
                {
                    Console.WriteLine("The following DTOs failed validation:");
                    return;
                }

                if (existingEntities.Any(e => EntityExists(e, dto)))
                    continue;

                if (newEntitiesToAdd.Any(e => EntityExists(e, dto)))
                    continue;

                var entity = AutoMapperConfig.MapperInstance.Map<TEntity>(dto);
                entity = await PostProcessEntityAsync(entity, dto);

                newEntitiesToAdd.Add(entity);

            }

            await _dbContext.Set<TEntity>().AddRangeAsync(newEntitiesToAdd);
            await _dbContext.SaveChangesAsync();
        }
    }
}

