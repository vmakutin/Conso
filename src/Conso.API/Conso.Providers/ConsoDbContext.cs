using Conso.Providers.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conso.Providers
{
    public class ConsoDbContext : DbContext
    {
        public ConsoDbContext(DbContextOptions<ConsoDbContext> options)
           : base(options)
        { }

        public DbSet<ClassEntity> ClassEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("ConsoApi");

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
