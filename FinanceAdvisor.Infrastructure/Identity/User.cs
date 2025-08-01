using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public User ToDomainUser() => new User
    {
        Id = Id,
        CreatedAt = CreatedAt,
        IsDeleted = IsDeleted,
        Email = Email
    };

    public static User FromDomainUser(User user) => new User
    {
        Id = user.Id,
        CreatedAt = user.CreatedAt,
        IsDeleted = user.IsDeleted,
        Email = user.Email,
        UserName = user.Email // or generate username from email
    };
}


//namespace FinanceAdvisor.Infrastructure.Identity
//{
//    public class ApplicationUser : IdentityUser<Guid>
//    {
//        public ApplicationUser()
//        {
//            // ReSharper disable once VirtualMemberCallInConstructor
//            Id = Guid.NewGuid();
//        }
//        // Infrastructure doesnt own Domain logic!
//        //public ICollection<Consultation>? Consultations { get; set; }
//        //public ICollection<CreditConsultationCycle>? CreditConsultationCycles { get; set; }
//        public DateTime CreatedAt { get; set; }
//        public bool IsDeleted { get; set; }
//    }
//}

