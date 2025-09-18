using Asp.netWebAPP.Core.Domain.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Asp.netWebAPP.Infrastructure.Data
{
  
    public class AbdmDbContext : DbContext
    {
        public AbdmDbContext(DbContextOptions<AbdmDbContext> options) : base(options) { }

        public DbSet<AbdmCoreParameters> AbdmCore_Parameters { get; set; }

    }
}
