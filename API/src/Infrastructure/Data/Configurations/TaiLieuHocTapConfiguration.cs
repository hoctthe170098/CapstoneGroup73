using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class TaiLieuHocTapConfiguration : IEntityTypeConfiguration<TaiLieuHocTap>
{
    public void Configure(EntityTypeBuilder<TaiLieuHocTap> builder)
    {
        builder.ToTable(nameof(TaiLieuHocTap));
        builder.HasKey(t => t.Id);
        builder.Property(x => x.Ten)
            .HasMaxLength(50);
        builder.Property(x => x.urlType)
            .HasMaxLength(20);
        builder.Property(x => x.urlFile)
            .HasMaxLength(200);
    }
}
