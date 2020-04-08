using Distributing.Domain.Model.Commissions;
using Distributing.Domain.SeedWork;
using Distributing.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Distributing.Infrastructure
{
    public class InMemoryDistributingContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "distributing_memory";

        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;

        public DbSet<Commission> Commissions { get; set; }

        public DbSet<UserType> UserTypes { get; set; }

        public InMemoryDistributingContext(DbContextOptions<InMemoryDistributingContext> options) : base(options) { }

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientRequestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CommissionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserTypeEntityTypeConfiguration());
        }

        public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DispatchDomainEventsAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class InMemoryDistributingContextDesignFactory : IDesignTimeDbContextFactory<InMemoryDistributingContext>
    {
        public InMemoryDistributingContextDesignFactory() : base()
        {
            //Debugger.Launch();
        }
        public InMemoryDistributingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InMemoryDistributingContext>()
                .UseSqlServer("Server=sql.data;Database=richpay.distributing.db.memory;User Id=sa;Password=1Secure*Password1;",
                                  sqlServerOptionsAction: sqlOptions =>
                                  {
                                      sqlOptions.MigrationsAssembly("WebMVC");
                                      sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                      sqlOptions.CommandTimeout(3 * 60);
                                  });

            return new InMemoryDistributingContext(optionsBuilder.Options);
        }

        class NoMediator : IMediator
        {
            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
            {
                return Task.FromResult<TResponse>(default(TResponse));
            }

        }
    }
}
