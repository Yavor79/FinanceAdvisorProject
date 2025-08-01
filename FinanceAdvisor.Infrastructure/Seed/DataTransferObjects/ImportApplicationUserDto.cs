namespace FinanceAdvisor.Infrastructure.Seed.DataTransferObjects
{
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;

    //using FinanceAdvisor.Infrastructure.Identity;


    public class ImportApplicationUserDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string NormalizedUserName { get; set; } = null!;

        [Required]
        public string NormalizedEmail { get; set; } = null!;

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<ImportApplicationUserDto, User>();
            //.ForMember(d => d.Id, opt => opt.MapFrom(s => Guid.Parse(s.Id)));
        }
    }
}
