using Excel.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Excel.Data.Mappings
{
    public class EXCELMapping : IEntityTypeConfiguration<EXCEL>
    {
        public void Configure(EntityTypeBuilder<EXCEL> builder)
        {
            builder.ToTable("EXCEL");
            builder.HasKey(t => new { t.DataEntrega });
        }
    }
}