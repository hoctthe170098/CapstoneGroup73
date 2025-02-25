using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class NhanVienConfiguration : IEntityTypeConfiguration<NhanVien>
{
    public void Configure(EntityTypeBuilder<NhanVien> builder)
    {
        builder.ToTable(nameof(NhanVien));
        builder.HasKey(x=>x.Code);
        builder.HasIndex(x=>x.UserId).IsUnique();
        builder.Property(x => x.Code)
            .HasMaxLength(20);
        builder.Property(x => x.Ten)
            .HasMaxLength(30);
        builder.Property(x => x.GioiTinh)
            .HasMaxLength(10);
        builder.HasAnnotation($"CheckConstraint:CK_NhanVien_GioiTinh", "[GioiTinh] IN ('Male', 'Female')");
        builder.Property(x => x.DiaChi)
            .HasMaxLength(50);
        builder.HasAnnotation($"CheckConstraint:CK_NhanVien_SoDienThoai", "[SoDienThoai] LIKE '0%' AND [SoDienThoai] NOT LIKE '%[^0-9]%'");
        builder.HasAnnotation($"CheckConstraint:CK_NhanVien_Email", "[Email] LIKE '%_@_%._%' AND [Email] NOT LIKE '%[^a-zA-Z0-9.@_%+-]%'");
    }
}
