using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class BaiKiemtraConfiguration : IEntityTypeConfiguration<BaiKiemTra>
{
    public void Configure(EntityTypeBuilder<BaiKiemTra> builder)
    {
        builder.ToTable(nameof(BaiKiemTra));
        builder.HasKey(x=>x.Id);
        builder.Property(kq => kq.Ten)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(kq => kq.UrlFile)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(kq => kq.TrangThai)
            .IsRequired()
            .HasMaxLength(20);
    }
}
