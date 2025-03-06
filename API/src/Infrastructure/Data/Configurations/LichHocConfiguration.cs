using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class LichHocConfiguration : IEntityTypeConfiguration<LichHoc>
{
    public void Configure(EntityTypeBuilder<LichHoc> builder)   
    {
        builder.ToTable(nameof(LichHoc));
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new {x.Thu,x.Phong,x.SlotId,x.TrangThai,x.CoSoId}).IsUnique();
        builder.HasAnnotation($"CheckConstraint:CK_LichHoc_Thu", "[Thu] >= 2 AND [Thu] <= 8");
        builder.Property(x => x.Phong)
           .HasMaxLength(20);
        builder.Property(x => x.TenLop)
           .HasMaxLength(20);
        builder.HasAnnotation($"CheckConstraint:CK_LichHoc_TrangThai", "[TrangThai] IN ('NotYet' , 'Open', 'Close')");          
    }
}
