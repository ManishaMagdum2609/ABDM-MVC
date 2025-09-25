using Asp.netWebAPP.Core.Domain.Model;
using Microsoft.EntityFrameworkCore;


namespace Asp.netWebAPP.Infrastructure.Data
{
    public class DanpheDbContext : DbContext
    {
        public DanpheDbContext(DbContextOptions<DanpheDbContext> options) : base(options) { }
        
             public DbSet<PatientModel> Patient { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map PatientModel to the real table name in DB
            modelBuilder.Entity<PatientModel>().ToTable("PAT_Patient");

            base.OnModelCreating(modelBuilder);
        }


    }  
    
}
