using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Pairing.Domain.Model.CloudDevices;
using Pairing.Domain.Model.FourthPartyGateways;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.Model.ShopGateways;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using Pairing.Domain.SeedWork;
using Pairing.Infrastructure.EntityConfigurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pairing.Infrastructure
{
    public class PairingContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "pairing";

        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;

        public DbSet<QrCode> QrCodes { get; set; }
        public DbSet<QrCodeOrder> QrCodeOrders { get; set; }

        public DbSet<ShopSettings> ShopSettings { get; set; }
        public DbSet<OrderAmountOption> OrderAmountOptions { get; set; }

        public DbSet<ShopGateway> ShopGateways { get; set; }
        public DbSet<FourthPartyGateway> FourthPartyGateways { get; set; }

        public DbSet<CloudDevice> CloudDevices { get; set; }


        public DbSet<QrCodeType> QrCodeTypes { get; set; }
        public DbSet<PaymentChannel> PaymentChannels { get; set; }
        public DbSet<PaymentScheme> PaymentSchemes { get; set; }
        public DbSet<QrCodeStatus> QrCodeStatus { get; set; }
        public DbSet<PairingStatus> PairingStatus { get; set; }

        public DbSet<ShopGatewayType> ShopGatewayTypes { get; set; }

        public DbSet<CloudDeviceStatus> CloudDeviceStatus { get; set; }


        public PairingContext(DbContextOptions<PairingContext> options) : base(options) { }

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public PairingContext(DbContextOptions<PairingContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


            System.Diagnostics.Debug.WriteLine("PairingContext::ctor ->" + this.GetHashCode());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientRequestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CloudDeviceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CloudDeviceStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderAmountOptionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PairingStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentChannelEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentSchemeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new QrCodeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new QrCodeOrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new QrCodeStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new QrCodeTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ShopGatewayEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ShopGatewayTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ShopSettingsEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FourthPartyGatewayEntityTypeConfiguration());
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DispatchDomainEventsAsync()
        {
            // Dispatch Domain Events collection. 
            await _mediator.DispatchDomainEventsAsync(this);

            return true;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task CommitTransactionOnlyAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            //await SaveChangesAsync();
            transaction.Commit();
        }

        public void RollbackTransaction(bool clearEvent = true)
        {
            //The Web Worker Service doens't have mediator to inject, it doesn't need to resolve domain events.
            if (clearEvent)
            {
                _mediator.ClearDomainEventsAsync(this);
            }

            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }

            var changedEntries = this.ChangeTracker.Entries().ToList();

            foreach (var entry in changedEntries)
            {
                /*switch (entry.State)
                {
                    case EntityState.Modified:
                        //entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Detached;
                        break;
                }*/

                entry.CurrentValues.SetValues(entry.OriginalValues);
                //Detach All Entities.
                entry.State = EntityState.Detached;
            }
        }

        public void ClearTrackedChangeOnAllEntityEntries()
        {
            var changedEntries = this.ChangeTracker.Entries().ToList();

            foreach (var entry in changedEntries)
            {
                entry.CurrentValues.SetValues(entry.OriginalValues);
                //Detach All Entities.
                entry.State = EntityState.Detached;
            }
        }

        public void DisposeTransaction()
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public class PairingContextDesignFactory : IDesignTimeDbContextFactory<PairingContext>
    {
        public PairingContextDesignFactory() : base()
        {
            //Debugger.Launch();
        }
        public PairingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PairingContext>()
                .UseSqlServer("Server=sql.data;Database=richpay.pairing.db;User Id=sa;Password=1Secure*Password1;",
                                  sqlServerOptionsAction: sqlOptions =>
                                  {
                                      sqlOptions.MigrationsAssembly("WebMVC");
                                      sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                      sqlOptions.CommandTimeout(3 * 60);
                                  });

            return new PairingContext(optionsBuilder.Options, new NoMediator());
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
