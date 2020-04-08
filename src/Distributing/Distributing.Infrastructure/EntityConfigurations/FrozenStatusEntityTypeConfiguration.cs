using Distributing.Domain.Model.Frozens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class FrozenStatusEntityTypeConfiguration
    : IEntityTypeConfiguration<FrozenStatus>
    {
        public void Configure(EntityTypeBuilder<FrozenStatus> frozenStatusConfiguration)
        {
            frozenStatusConfiguration.ToTable("frozenstatus", DistributingContext.DEFAULT_SCHEMA);

            frozenStatusConfiguration.HasKey(o => o.Id);

            frozenStatusConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            frozenStatusConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
