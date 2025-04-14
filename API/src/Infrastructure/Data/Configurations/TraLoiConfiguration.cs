using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class TraLoiConfiguration : IEntityTypeConfiguration<TraLoi>
{
    public void Configure(EntityTypeBuilder<TraLoi> builder)
    {
        builder.ToTable(nameof(TraLoi));
        builder.HasKey(t => t.Id);
        builder.Property(x => x.NoiDung)
          .HasMaxLength(750);
        builder.Property(x => x.UrlFile)
          .HasMaxLength(200);
        builder.Property(x=>x.NhanXet)
          .HasMaxLength(200);
    }
}
