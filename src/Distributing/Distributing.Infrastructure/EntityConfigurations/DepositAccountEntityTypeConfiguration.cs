using Distributing.Domain.Model.Banks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class DepositAccountEntityTypeConfiguration : IEntityTypeConfiguration<DepositAccount>
    {
        public void Configure(EntityTypeBuilder<DepositAccount> depositAccountConfiguration)
        {
            depositAccountConfiguration.ToTable("depositAccounts", DistributingContext.DEFAULT_SCHEMA);

            depositAccountConfiguration.HasKey(d => d.Id);

            depositAccountConfiguration.Ignore(d => d.DomainEvents);

            depositAccountConfiguration.Property(d => d.Id)
                .UseHiLo("depositaccountseq", DistributingContext.DEFAULT_SCHEMA);

            //Address value object persisted as owned entity type supported since EF Core 2.0
            depositAccountConfiguration
                .OwnsOne(d => d.BankAccount, b =>
                {
                    b.WithOwner();
                });

            /*depositAccountConfiguration.Property(d => d.DepositAccountId)
                .HasMaxLength(200)
                .IsRequired();

            depositAccountConfiguration.HasIndex("DepositAccountId")
              .IsUnique(true);*/

            depositAccountConfiguration.Property(d => d.DateCreated).IsRequired();
        }
    }
}
