using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class CoSoConfiguration : IEntityTypeConfiguration<CoSo>
{
    public void Configure(EntityTypeBuilder<CoSo> builder)
    {
        builder.ToTable(nameof(CoSo));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Ten)
            .HasMaxLength(30);
        builder.Property(x => x.DiaChi)
            .HasMaxLength(50);
        builder.Property(x => x.SoDienThoai)
            .HasMaxLength(11);
        builder.HasAnnotation($"CheckConstraint:CK_CoSo_SoDienThoai", "[SoDienThoai] LIKE '0%' AND [SoDienThoai] NOT LIKE '%[^0-9]%'");
        builder.Property(x => x.TrangThai)
            .HasMaxLength(10);
        builder.HasAnnotation($"CheckConstraint:CK_CoSo_TrangThai", "[TrangThai] IN ('open', 'close')");
    }
}
