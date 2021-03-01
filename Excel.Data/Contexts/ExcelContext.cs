using Excel.Data.Mappings;
using Excel.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace Excel.Data.Context
{
    public partial class ExcelContext : DbContext
    {
        public ExcelContext(DbContextOptions<ExcelContext> options) : base(options) { }

        public virtual DbSet<EXCEL> EXCEL { get; set; }
        public virtual DbSet<IMPORTACAO_EXCEL> IMPORTACAO_EXCEL { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EXCEL>().ToTable("EXCEL");
            modelBuilder.Entity<IMPORTACAO_EXCEL>().ToTable("IMPORTACAO_EXCEL");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExcelContext).Assembly);

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new EXCELMapping());
            modelBuilder.ApplyConfiguration(new IMPORTACAO_EXCELMapping());
        }
    }
}