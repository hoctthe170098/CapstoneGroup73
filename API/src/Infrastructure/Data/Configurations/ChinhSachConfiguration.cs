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
        builder.ToTable(nameof(ChinhSach));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Ten)
            .HasMaxLength(30);
        builder.Property(x => x.Mota)
            .HasMaxLength(20);
        builder.HasAnnotation($"CheckConstraint:CK_ChinhSach_PhanTramGiam", "[PhanTramGiam] > 0.8 AND [PhanTramGiam] < 1");
    }
}
