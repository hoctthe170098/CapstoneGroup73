using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class PhongConfiguration : IEntityTypeConfiguration<Phong>
{
    public void Configure(EntityTypeBuilder<Phong> builder)
    {
        builder.ToTable(nameof(Phong));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Ten)
           .HasMaxLength(20);
        builder.HasAnnotation($"CheckConstraint:CK_Phong_TrangThai", "[TrangThai] IN ('Use', 'NonUse')");
    }
}
