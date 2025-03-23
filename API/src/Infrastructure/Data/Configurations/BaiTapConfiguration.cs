using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class BaiTapConfiguration : IEntityTypeConfiguration<BaiTap>
{
    public void Configure(EntityTypeBuilder<BaiTap> builder)
    {
        builder.ToTable(nameof(BaiTap));
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Ngay, x.LichHocId })
       .IsUnique();
        builder.Property(kq => kq.TieuDe)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(kq => kq.NoiDung)
            .IsRequired()
            .HasMaxLength(750);
        builder.Property(kq => kq.UrlFile)
            .HasMaxLength(200);
        builder.Property(kq => kq.TrangThai)
            .IsRequired()
            .HasMaxLength(10);
        builder.HasAnnotation($"CheckConstraint:CK_BaiTap_TrangThai", "[TrangThai] IN ('open', 'close')");
    }
}
