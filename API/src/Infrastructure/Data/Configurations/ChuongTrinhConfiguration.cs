using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class ChuongTrinhConfiguration : IEntityTypeConfiguration<ChuongTrinh>
{
    public void Configure(EntityTypeBuilder<ChuongTrinh> builder)
    {
        builder.ToTable(nameof(ChuongTrinh));
        builder.HasKey(t => t.Id);
        builder.Property(x => x.TieuDe)
            .HasMaxLength(200);
        builder.Property(x => x.MoTa)
            .HasMaxLength(300);
        builder.HasAnnotation($"CheckConstraint:CK_ChuongTrinh_TrangThai", "[TrangThai] IN ('use', 'notuse')");
    }
}
