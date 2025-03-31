using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class ChinhSachConfiguration : IEntityTypeConfiguration<ChinhSach>
{
    public void Configure(EntityTypeBuilder<ChinhSach> builder)
    {
        builder.ToTable(nameof(ChinhSach), b =>
        {
            b.HasCheckConstraint("CK_ChinhSach_PhanTramGiam", "[PhanTramGiam] > 0 AND [PhanTramGiam] <= 0.1");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Ten)
            .HasMaxLength(30);
        builder.Property(x => x.Mota)
            .HasMaxLength(200);
    }
}
