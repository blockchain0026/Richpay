using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMVC.Data.Migrations.Distributing
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "distributing");

            migrationBuilder.CreateSequence(
                name: "balancewithdrawalseq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "processingorderseq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "balanceseq",
                schema: "distributing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "chainseq",
                schema: "distributing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "commissionseq",
                schema: "distributing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "depositaccountseq",
                schema: "distributing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "depositseq",
                schema: "distributing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "distributionseq",
                schema: "distributing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "frozenseq",
                schema: "distributing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "transferseq",
                schema: "distributing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "withdrawalbankseq",
                schema: "distributing",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "withdrawalseq",
                schema: "distributing",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "chains",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "depositAccounts",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    BankAccount_BankName = table.Column<string>(nullable: true),
                    BankAccount_AccountName = table.Column<string>(nullable: true),
                    BankAccount_AccountNumber = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_depositAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "depositstatus",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_depositstatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "deposittype",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deposittype", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "distributiontypes",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_distributiontypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "frozenstatus",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_frozenstatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "frozentype",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_frozentype", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "initiatedby",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_initiatedby", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "requests",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usertypes",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usertypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "withdrawalBanks",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    BankName = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_withdrawalBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "withdrawalstatus",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_withdrawalstatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "withdrawaltype",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_withdrawaltype", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "balances",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    UserTypeId1 = table.Column<int>(nullable: true),
                    AmountAvailable = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    WithdrawalLimit_DailyAmountLimit = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    WithdrawalLimit_DailyFrequencyLimit = table.Column<int>(nullable: true),
                    WithdrawalLimit_EachAmountUpperLimit = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    WithdrawalLimit_EachAmountLowerLimit = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    WithdrawalCommissionRate = table.Column<decimal>(type: "decimal(4,3)", nullable: false),
                    DepositCommissionRate = table.Column<decimal>(type: "decimal(4,3)", nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UserTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_balances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_balances_usertypes_UserTypeId1",
                        column: x => x.UserTypeId1,
                        principalSchema: "distributing",
                        principalTable: "usertypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_balances_usertypes_UserTypeId",
                        column: x => x.UserTypeId,
                        principalSchema: "distributing",
                        principalTable: "usertypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "commissions",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ChainNumber = table.Column<int>(nullable: false),
                    UserTypeId = table.Column<int>(nullable: false),
                    UplineCommissionId = table.Column<int>(nullable: true),
                    RateAlipay = table.Column<decimal>(type: "decimal(4,3)", nullable: false),
                    RateWechat = table.Column<decimal>(type: "decimal(4,3)", nullable: false),
                    RateRebateAlipay = table.Column<decimal>(type: "decimal(4,3)", nullable: false),
                    RateRebateWechat = table.Column<decimal>(type: "decimal(4,3)", nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_commissions_commissions_UplineCommissionId",
                        column: x => x.UplineCommissionId,
                        principalSchema: "distributing",
                        principalTable: "commissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_commissions_usertypes_UserTypeId",
                        column: x => x.UserTypeId,
                        principalSchema: "distributing",
                        principalTable: "usertypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deposits",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    DepositStatus = table.Column<int>(nullable: false),
                    DepositType = table.Column<int>(nullable: false),
                    BalanceId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    BankAccount_BankName = table.Column<string>(nullable: true),
                    BankAccount_AccountName = table.Column<string>(nullable: true),
                    BankAccount_AccountNumber = table.Column<string>(nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(4,3)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ActualAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    BalanceRecord_BalanceBefore = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    BalanceRecord_BalanceAfter = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Description = table.Column<string>(nullable: true),
                    VerifiedBy_AdminId = table.Column<string>(nullable: true),
                    VerifiedBy_Name = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateFinished = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deposits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_deposits_balances_BalanceId",
                        column: x => x.BalanceId,
                        principalSchema: "distributing",
                        principalTable: "balances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_deposits_depositstatus_DepositStatus",
                        column: x => x.DepositStatus,
                        principalSchema: "distributing",
                        principalTable: "depositstatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_deposits_deposittype_DepositType",
                        column: x => x.DepositType,
                        principalSchema: "distributing",
                        principalTable: "deposittype",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "frozens",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    FrozenStatusId = table.Column<int>(nullable: false),
                    FrozenTypeId = table.Column<int>(nullable: false),
                    BalanceId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    BalanceFrozenRecord_BalanceBefore = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    BalanceFrozenRecord_BalanceAfter = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    BalanceUnfrozenRecord_BalanceBefore = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    BalanceUnfrozenRecord_BalanceAfter = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    OrderTrackingNumber = table.Column<string>(nullable: true),
                    WithdrawalId = table.Column<int>(nullable: true),
                    ByAdmin_AdminId = table.Column<string>(nullable: true),
                    ByAdmin_Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateFroze = table.Column<DateTime>(nullable: false),
                    DateUnfroze = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_frozens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_frozens_balances_BalanceId",
                        column: x => x.BalanceId,
                        principalSchema: "distributing",
                        principalTable: "balances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_frozens_frozenstatus_FrozenStatusId",
                        column: x => x.FrozenStatusId,
                        principalSchema: "distributing",
                        principalTable: "frozenstatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_frozens_frozentype_FrozenTypeId",
                        column: x => x.FrozenTypeId,
                        principalSchema: "distributing",
                        principalTable: "frozentype",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transfers",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    FromBalanceId = table.Column<int>(nullable: false),
                    ToBalanceId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    InitiatedBy = table.Column<int>(nullable: false),
                    DateTransferred = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_transfers_balances_FromBalanceId",
                        column: x => x.FromBalanceId,
                        principalSchema: "distributing",
                        principalTable: "balances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transfers_balances_ToBalanceId",
                        column: x => x.ToBalanceId,
                        principalSchema: "distributing",
                        principalTable: "balances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transfers_initiatedby_InitiatedBy",
                        column: x => x.InitiatedBy,
                        principalSchema: "distributing",
                        principalTable: "initiatedby",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "withdrawals",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    WithdrawalStatus = table.Column<int>(nullable: false),
                    WithdrawalType = table.Column<int>(nullable: false),
                    BalanceId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    BankAccount_BankName = table.Column<string>(nullable: true),
                    BankAccount_AccountName = table.Column<string>(nullable: true),
                    BankAccount_AccountNumber = table.Column<string>(nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(4,3)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ActualAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ApprovedBy_AdminId = table.Column<string>(nullable: true),
                    ApprovedBy_Name = table.Column<string>(nullable: true),
                    CancellationApprovedBy_AdminId = table.Column<string>(nullable: true),
                    CancellationApprovedBy_Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateFinished = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_withdrawals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_withdrawals_balances_BalanceId",
                        column: x => x.BalanceId,
                        principalSchema: "distributing",
                        principalTable: "balances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_withdrawals_withdrawalstatus_WithdrawalStatus",
                        column: x => x.WithdrawalStatus,
                        principalSchema: "distributing",
                        principalTable: "withdrawalstatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_withdrawals_withdrawaltype_WithdrawalType",
                        column: x => x.WithdrawalType,
                        principalSchema: "distributing",
                        principalTable: "withdrawaltype",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "distributions",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    DistributionTypeId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    CommissionId = table.Column<int>(nullable: false),
                    Order_TrackingNumber = table.Column<string>(nullable: true),
                    Order_ShopOrderId = table.Column<string>(nullable: true),
                    Order_Amount = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Order_CommissionAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    Order_ShopId = table.Column<string>(nullable: true),
                    Order_TraderId = table.Column<string>(nullable: true),
                    Order_DateCreated = table.Column<DateTime>(nullable: true),
                    BalanceId = table.Column<int>(nullable: false),
                    DistributedAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DateDistributed = table.Column<DateTime>(nullable: true),
                    IsFinished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_distributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_distributions_commissions_CommissionId",
                        column: x => x.CommissionId,
                        principalSchema: "distributing",
                        principalTable: "commissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_distributions_distributiontypes_DistributionTypeId",
                        column: x => x.DistributionTypeId,
                        principalSchema: "distributing",
                        principalTable: "distributiontypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "processingOrders",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    TrackingNumber = table.Column<string>(nullable: false),
                    CommissionId = table.Column<int>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processingOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_processingOrders_commissions_CommissionId",
                        column: x => x.CommissionId,
                        principalSchema: "distributing",
                        principalTable: "commissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "balanceWithdrawals",
                schema: "distributing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    WithdrawalId = table.Column<int>(nullable: false),
                    BalanceId = table.Column<int>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DateWithdraw = table.Column<DateTime>(nullable: false),
                    IsFailed = table.Column<bool>(nullable: false),
                    IsSuccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_balanceWithdrawals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_balanceWithdrawals_balances_BalanceId",
                        column: x => x.BalanceId,
                        principalSchema: "distributing",
                        principalTable: "balances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_balanceWithdrawals_withdrawals_WithdrawalId",
                        column: x => x.WithdrawalId,
                        principalSchema: "distributing",
                        principalTable: "withdrawals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_balances_UserTypeId1",
                schema: "distributing",
                table: "balances",
                column: "UserTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_balances_UserTypeId",
                schema: "distributing",
                table: "balances",
                column: "UserTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_balanceWithdrawals_BalanceId",
                schema: "distributing",
                table: "balanceWithdrawals",
                column: "BalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_balanceWithdrawals_WithdrawalId",
                schema: "distributing",
                table: "balanceWithdrawals",
                column: "WithdrawalId");

            migrationBuilder.CreateIndex(
                name: "IX_commissions_UplineCommissionId",
                schema: "distributing",
                table: "commissions",
                column: "UplineCommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_commissions_UserId",
                schema: "distributing",
                table: "commissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_commissions_UserTypeId",
                schema: "distributing",
                table: "commissions",
                column: "UserTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_deposits_BalanceId",
                schema: "distributing",
                table: "deposits",
                column: "BalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_deposits_DepositStatus",
                schema: "distributing",
                table: "deposits",
                column: "DepositStatus");

            migrationBuilder.CreateIndex(
                name: "IX_deposits_DepositType",
                schema: "distributing",
                table: "deposits",
                column: "DepositType");

            migrationBuilder.CreateIndex(
                name: "IX_distributions_CommissionId",
                schema: "distributing",
                table: "distributions",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_distributions_UserId",
                schema: "distributing",
                table: "distributions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_distributions_DistributionTypeId",
                schema: "distributing",
                table: "distributions",
                column: "DistributionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_frozens_BalanceId",
                schema: "distributing",
                table: "frozens",
                column: "BalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_frozens_FrozenStatusId",
                schema: "distributing",
                table: "frozens",
                column: "FrozenStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_frozens_FrozenTypeId",
                schema: "distributing",
                table: "frozens",
                column: "FrozenTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_processingOrders_CommissionId",
                schema: "distributing",
                table: "processingOrders",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_transfers_FromBalanceId",
                schema: "distributing",
                table: "transfers",
                column: "FromBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_transfers_ToBalanceId",
                schema: "distributing",
                table: "transfers",
                column: "ToBalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_transfers_InitiatedBy",
                schema: "distributing",
                table: "transfers",
                column: "InitiatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_withdrawals_BalanceId",
                schema: "distributing",
                table: "withdrawals",
                column: "BalanceId");

            migrationBuilder.CreateIndex(
                name: "IX_withdrawals_WithdrawalStatus",
                schema: "distributing",
                table: "withdrawals",
                column: "WithdrawalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_withdrawals_WithdrawalType",
                schema: "distributing",
                table: "withdrawals",
                column: "WithdrawalType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "balanceWithdrawals",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "chains",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "depositAccounts",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "deposits",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "distributions",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "frozens",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "processingOrders",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "requests",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "transfers",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "withdrawalBanks",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "withdrawals",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "depositstatus",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "deposittype",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "distributiontypes",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "frozenstatus",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "frozentype",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "commissions",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "initiatedby",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "balances",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "withdrawalstatus",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "withdrawaltype",
                schema: "distributing");

            migrationBuilder.DropTable(
                name: "usertypes",
                schema: "distributing");

            migrationBuilder.DropSequence(
                name: "balancewithdrawalseq");

            migrationBuilder.DropSequence(
                name: "processingorderseq");

            migrationBuilder.DropSequence(
                name: "balanceseq",
                schema: "distributing");

            migrationBuilder.DropSequence(
                name: "chainseq",
                schema: "distributing");

            migrationBuilder.DropSequence(
                name: "commissionseq",
                schema: "distributing");

            migrationBuilder.DropSequence(
                name: "depositaccountseq",
                schema: "distributing");

            migrationBuilder.DropSequence(
                name: "depositseq",
                schema: "distributing");

            migrationBuilder.DropSequence(
                name: "distributionseq",
                schema: "distributing");

            migrationBuilder.DropSequence(
                name: "frozenseq",
                schema: "distributing");

            migrationBuilder.DropSequence(
                name: "transferseq",
                schema: "distributing");

            migrationBuilder.DropSequence(
                name: "withdrawalbankseq",
                schema: "distributing");

            migrationBuilder.DropSequence(
                name: "withdrawalseq",
                schema: "distributing");
        }
    }
}
