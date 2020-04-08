using Distributing.Domain.Model.Commissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class UserTypeEntityTypeConfiguration
    : IEntityTypeConfiguration<UserType>
    {
        public void Configure(EntityTypeBuilder<UserType> userTypesConfiguration)
        {
            userTypesConfiguration.ToTable("usertypes", DistributingContext.DEFAULT_SCHEMA);

            userTypesConfiguration.HasKey(o => o.Id);

            userTypesConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            userTypesConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
