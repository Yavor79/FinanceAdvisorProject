using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Application.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceAdvisor.Infrastructure.Repository
{
    public class MeetingRepository : BaseRepository<Meeting, Guid>, IMeetingRepository
    {
        public MeetingRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
