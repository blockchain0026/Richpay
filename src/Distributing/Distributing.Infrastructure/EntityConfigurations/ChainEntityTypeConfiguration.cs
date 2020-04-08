using Distributing.Domain.Model.Chains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class ChainEntityTypeConfiguration : IEntityTypeConfiguration<Chain>
    {
        public void Configure(EntityTypeBuilder<Chain> chainConfiguration)
        {
            chainConfiguration.ToTable("chains", DistributingContext.DEFAULT_SCHEMA);

            chainConfiguration.HasKey(c => c.Id);

            chainConfiguration.Ignore(c => c.DomainEvents);

            chainConfiguration.Property(c => c.Id)
                .UseHiLo("chainseq", DistributingContext.DEFAULT_SCHEMA);
        }
    }
}
