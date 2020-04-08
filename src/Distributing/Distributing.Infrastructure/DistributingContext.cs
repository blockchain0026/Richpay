using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Banks;
using Distributing.Domain.Model.Chains;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Deposits;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Frozens;
using Distributing.Domain.Model.Transfers;
using Distributing.Domain.Model.Withdrawals;
using Distributing.Domain.SeedWork;
using Distributing.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distributing.Infrastructure
{
    public class DistributingContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "distributing";

        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;

        public DbSet<Balance> Balances { get; set; }
        public DbSet<BalanceWithdrawal> BalanceWithdrawals { get; set; }

        public DbSet<DepositAccount> DepositAccounts { get; set; }
        public DbSet<WithdrawalBank> WithdrawalBanks { get; set; }

        public DbSet<Commission> Commissions { get; set; }
        public DbSet<Chain> Chains { get; set; }

        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Distribution> Distributions { get; set; }
        public DbSet<Withdrawal> Withdrawals { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Frozen> Frozens { get; set; }


        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<DistributionType> DistributionTypes { get; set; }
        public DbSet<DepositType> DepositTypes { get; set; }
        public DbSet<WithdrawalType> WithdrawalTypes { get; set; }
        public DbSet<FrozenType> FrozenTypes { get; set; }
        public DbSet<InitiatedBy> InitiatedBys { get; set; }

        public DbSet<DepositStatus> DepositStatus { get; set; }
        public DbSet<WithdrawalStatus> WithdrawalStatus { get; set; }
        public DbSet<FrozenStatus> FrozenStatus { get; set; }


        public DistributingContext(DbContextOptions<DistributingContext> options) : base(options) { }

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public DistributingContext(DbContextOptions<DistributingContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


            System.Diagnostics.Debug.WriteLine("DistributingContext::ctor ->" + this.GetHashCode());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BalanceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BalanceWithdrawalEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ChainEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClientRequestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CommissionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DepositAccountEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DepositEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DepositStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DepositTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DistributionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DistributionTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FrozenEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FrozenStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FrozenTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new InitiatedByEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProcessingOrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TransferEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new WithdrawalBankEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new WithdrawalEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new WithdrawalStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new WithdrawalTypeEntityTypeConfiguration());
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
            //_currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);

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
                entry.CurrentValues.SetValues(entry.OriginalValues);
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

            /*
            var changedEntries = this.ChangeTracker.Entries().ToList();
            foreach (var entry in changedEntries)
            {
                entry.CurrentValues.SetValues(entry.OriginalValues);
                //Detach All Entities.
                entry.State = EntityState.Detached;
            }
            */
        }
    }

    public class DistributingContextDesignFactory : IDesignTimeDbContextFactory<DistributingContext>
    {
        public DistributingContextDesignFactory() : base()
        {
            //Debugger.Launch();
        }
        public DistributingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DistributingContext>()
                .UseSqlServer("Server=sql.data;Database=richpay.distributing.db;User Id=sa;Password=1Secure*Password1;",
                                  sqlServerOptionsAction: sqlOptions =>
                                  {
                                      sqlOptions.MigrationsAssembly("WebMVC");
                                      sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                      sqlOptions.CommandTimeout(3 * 60);
                                  });

            return new DistributingContext(optionsBuilder.Options, new NoMediator());
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
