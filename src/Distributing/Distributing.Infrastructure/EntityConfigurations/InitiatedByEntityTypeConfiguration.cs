using Distributing.Domain.Model.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class InitiatedByEntityTypeConfiguration
     : IEntityTypeConfiguration<InitiatedBy>
    {
        public void Configure(EntityTypeBuilder<InitiatedBy> initiatedByConfiguration)
        {
            initiatedByConfiguration.ToTable("initiatedby", DistributingContext.DEFAULT_SCHEMA);

            initiatedByConfiguration.HasKey(o => o.Id);

            initiatedByConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            initiatedByConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
