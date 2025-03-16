using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class GiaoVienConfiguration : IEntityTypeConfiguration<GiaoVien>
{
    public void Configure(EntityTypeBuilder<GiaoVien> builder)
    {
        builder.ToTable(nameof(GiaoVien));
        builder.HasKey(x=>x.Code);
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.Property(x => x.Code)
            .HasMaxLength(20);
        builder.Property(x => x.Ten)
            .HasMaxLength(30);
        builder.Property(x => x.GioiTinh)
            .HasMaxLength(10);
        builder.HasAnnotation($"CheckConstraint:CK_GiaoVien_GioiTinh", "[GioiTinh] IN ('Male', 'Female')");
        builder.Property(x => x.DiaChi)
            .HasMaxLength(200);
        builder.Property(x => x.TruongDangDay)
            .HasMaxLength(100);
        builder.Property(x => x.SoDienThoai)
            .HasMaxLength(11);
        builder.Property(x => x.Email)
            .HasMaxLength(50);
        builder.HasAnnotation($"CheckConstraint:CK_GiaoVien_SoDienThoai", "[SoDienThoai] LIKE '0%' AND [SoDienThoai] NOT LIKE '%[^0-9]%'");
        builder.HasAnnotation($"CheckConstraint:CK_GiaoVien_Email", "[Email] LIKE '%_@_%._%' AND [Email] NOT LIKE '%[^a-zA-Z0-9.@_%+-]%'");
    }
}
