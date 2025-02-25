using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class DiemDanhConfiguration : IEntityTypeConfiguration<DiemDanh>
{
    public void Configure(EntityTypeBuilder<DiemDanh> builder)
    {
        builder.ToTable(nameof(DiemDanh));
        builder.HasKey(d => d.Id);
        builder.HasIndex(x => new { x.Ngay, x.ThamGiaLopHocId })
       .IsUnique();
        builder.Property(x => x.TrangThai)
           .HasMaxLength(10);
        builder.HasAnnotation($"CheckConstraint:CK_DiemDanh_TrangThai", "[TrangThai] IN ('comat', 'vangmat')");
        builder.HasAnnotation($"CheckConstraint:CK_DiemDanh_DiemBTVN", "[DiemBTVN] > 0 AND [DiemBTVN] <= 10");
        builder.HasAnnotation($"CheckConstraint:CK_DiemDanh_DiemTrenLop", "[DiemTrenLop] > 0 AND [DiemTrenLop] <= 10");
        builder.Property(x => x.NhanXet)
           .HasMaxLength(200);

    }
}
