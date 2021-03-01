using Excel.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Excel.Data.Mappings
{
    public class IMPORTACAO_EXCELMapping : IEntityTypeConfiguration<IMPORTACAO_EXCEL>
    {
        public void Configure(EntityTypeBuilder<IMPORTACAO_EXCEL> builder)
        {
            builder.ToTable("IMPORTACAO_EXCEL");
            builder.HasKey(t => new { t.Id });
        }
    }
}