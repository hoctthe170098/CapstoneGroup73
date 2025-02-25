using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class ThamGiaLopHocConfiguration : IEntityTypeConfiguration<ThamGiaLopHoc>
{
    public void Configure(EntityTypeBuilder<ThamGiaLopHoc> builder)
    {
        
        builder.ToTable(nameof(ThamGiaLopHoc));
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.HocSinhCode, x.LichHocId })
       .IsUnique();
        builder.HasAnnotation($"CheckConstraint:CK_ThamGiaLopHoc_TrangThai", "[TrangThai] IN ('hocthu', 'chinhthuc','hocbu')");
    }
}
