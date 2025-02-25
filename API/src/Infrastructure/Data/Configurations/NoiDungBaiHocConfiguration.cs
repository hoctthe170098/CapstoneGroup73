using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyFlow.Domain.Entities;

namespace StudyFlow.Infrastructure.Data.Configurations;
public class NoiDungBaiHocConfiguration : IEntityTypeConfiguration<NoiDungBaiHoc>
{
    public void Configure(EntityTypeBuilder<NoiDungBaiHoc> builder)
    {
        builder.ToTable(nameof(NoiDungBaiHoc));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TieuDe)
            .HasMaxLength(50);
        builder.Property(x => x.Mota)
            .HasMaxLength(200);
    }
}
