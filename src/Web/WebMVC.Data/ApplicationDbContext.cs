using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using WebMVC.Data.EntityConfigurations;
using WebMVC.Models.Queries;

namespace WebMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private IDbContextTransaction _currentTransaction;
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
        public bool HasActiveTransaction => _currentTransaction != null;


        public DbSet<BankbookRecord> BankbookRecords { get; set; }
        public DbSet<TraderAgent> TraderAgents { get; set; }
        public DbSet<Trader> Traders { get; set; }
        public DbSet<ShopAgent> ShopAgents { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<WithdrawalEntry> WithdrawalEntrys { get; set; }
        public DbSet<DepositEntry> DepositEntrys { get; set; }

        public DbSet<QrCodeEntry> QrCodeEntrys { get; set; }
        public DbSet<ShopGatewayEntry> ShopGatewayEntrys { get; set; }

        public DbSet<OrderEntry> OrderEntrys { get; set; }
        public DbSet<RunningAccountRecord> RunningAccountRecords { get; set; }
        public DbSet<TempRunningAccountRecord> TempRunningAccountRecords { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            System.Diagnostics.Debug.WriteLine("ApplicationDbContext::ctor ->" + this.GetHashCode());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new BankbookRecordEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TraderAgentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TraderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ShopAgentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ShopEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DepositEntryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new WithdrawalEntryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new QrCodeEntryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ShopGatewayEntryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RunningAccountRecordEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TempRunningAccountRecordEntityTypeConfiguration());
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionOnlyAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            //await SaveChangesAsync();
            transaction.Commit();
        }

        public void RollbackTransaction()
        {
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

            var changedEntries = this.ChangeTracker.Entries().ToList();
            foreach (var entry in changedEntries)
            {
                entry.CurrentValues.SetValues(entry.OriginalValues);
                //Detach All Entities.
                entry.State = EntityState.Detached;
            }
        }

        public void DetachAllEntities()
        {
            var changedEntries = this.ChangeTracker.Entries().ToList();

            foreach (var entry in changedEntries)
            {
                entry.State = EntityState.Detached;
            }
        }
    }

    public class ApplicationDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContextDesignFactory() : base()
        {
            //Debugger.Launch();
        }
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            /*IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();*/

            /*var optionsBuilder = new DbContextOptionsBuilder<ExecutionContext>()
                .UseSqlServer("Server=.;Initial Catalog=CryptoArbitrage.Services.ExecutionDb;Integrated Security=true");*/
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=richpay.db;User Id=sa;Password=1Secure*Password1;",
                                  sqlServerOptionsAction: sqlOptions =>
                                  {
                                      sqlOptions.MigrationsAssembly("WebMVC");
                                      sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                      sqlOptions.CommandTimeout(3 * 60);
                                  });
            //.UseSqlServer("Server=.;Initial Catalog=richpay.db;User Id=sa;Password=1Secure*Password1;");

            return new ApplicationDbContext(optionsBuilder.Options);
        }

    }
}
