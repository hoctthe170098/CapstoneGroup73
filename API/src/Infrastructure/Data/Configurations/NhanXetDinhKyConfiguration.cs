using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class NhanXetDinhKyConfiguration : IEntityTypeConfiguration<NhanXetDinhKy>
{
    public void Configure(EntityTypeBuilder<NhanXetDinhKy> builder)
    {
        builder.ToTable(nameof(NhanXetDinhKy));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.NoiDungNhanXet)
            .HasMaxLength(200);
    }
}
