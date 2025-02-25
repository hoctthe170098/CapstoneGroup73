using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class KetQuaBaiKiemtraConfiguration : IEntityTypeConfiguration<KetQuaBaiKiemTra>
{
    public void Configure(EntityTypeBuilder<KetQuaBaiKiemTra> builder)
    {
       builder.ToTable(nameof(KetQuaBaiKiemTra));
       builder.HasKey(k => k.Id);
       builder.HasIndex(kq => new { kq.HocSinhCode, kq.BaiKiemTraId }).IsUnique();
       builder.HasAnnotation($"CheckConstraint:CK_KetQuaBaiKiemTra_Diem", "[Diem] > 0 AND [Diem] <= 10");
       builder.Property(x => x.NhanXet)
           .HasMaxLength(200);
       /* // Cấu hình các khóa ngoại và hành động xóa (delete behavior)
        builder.HasOne(kq => kq.HocSinh)
            .WithMany()
            .HasForeignKey(kq => kq.HocSinhCode)
            .OnDelete(DeleteBehavior.Restrict); // Thay đổi ON DELETE CASCADE thành ON DELETE RESTRICT

        builder.HasOne(kq => kq.BaiKiemTra)
            .WithMany()
            .HasForeignKey(kq => kq.BaiKiemTraId)
            .OnDelete(DeleteBehavior.Cascade); // Giữ nguyên ON DELETE CASCADE cho BaiKiemTra*/
    }
}
