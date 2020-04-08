using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Permissions
{
    public static class Permissions
    {
        public static class Additional
        {
            public const string Reviewed = "Permissions.Additional.Reviewed";
        }

        public static class Dashboards
        {
            public const string View = "Permissions.Dashboards.View";
        }
        public static class Hubs
        {
            public const string DepositVoice = "Permissions.Hubs.DepositVoice";
            public const string OrderStatistic = "Permissions.Hubs.OrderStatistic";
            public const string WithdrawalVoice = "Permissions.Hubs.WithdrawalVoice";
        }

        public static class Personal
        {
            public const string View = "Permissions.Personal.View";
            public const string Edit = "Permissions.Personal.Edit";

            public static class TwoFactorAuth
            {
                public const string View = "Permissions.Personal.TwoFactorAuth.View";
                public const string Enable = "Permissions.Personal.TwoFactorAuth.Enable";
            }
            public static class Manager
            {
                public const string View = "Permissions.Personal.Manager.View";
                public const string Edit = "Permissions.Personal.Manager.Edit";
            }
            public static class Trader
            {
                public const string View = "Permissions.Personal.Trader.View";
                public const string Edit = "Permissions.Personal.Trader.Edit";
            }
            public static class TraderAgent
            {
                public const string View = "Permissions.Personal.TraderAgent.View";
                public const string Edit = "Permissions.Personal.TraderAgent.Edit";
            }
            public static class Shop
            {
                public const string View = "Permissions.Personal.Shop.View";
                public const string Edit = "Permissions.Personal.Shop.Edit";
            }
            public static class ShopAgent
            {
                public const string View = "Permissions.Personal.ShopAgent.View";
                public const string Edit = "Permissions.Personal.ShopAgent.Edit";
            }
        }

        public static class Administration
        {
            public static class Managers
            {
                public const string View = "Permissions.Administration.Managers.View";
                public const string Create = "Permissions.Administration.Managers.Create";
                public const string Edit = "Permissions.Administration.Managers.Edit";
                public const string Delete = "Permissions.Administration.Managers.Delete";
            }
            public static class Roles
            {
                public const string View = "Permissions.Administration.Roles.View";
                public const string Create = "Permissions.Administration.Roles.Create";
                public const string Edit = "Permissions.Administration.Roles.Edit";
                public const string Delete = "Permissions.Administration.Roles.Delete";
            }
            /*public static class Permissions
            {
                public const string View = "Permissions.Administration.Permissions.View";
                public const string Create = "Permissions.Administration.Permissions.Create";
                public const string Edit = "Permissions.Administration.Permissions.Edit";
                public const string Delete = "Permissions.Administration.Permissions.Delete";
            }*/
        }

        public static class SystemConfiguration
        {
            public static class SiteBaseInfo
            {
                public const string View = "Permissions.SystemConfiguration.SiteBaseInfo.View";
                public const string Edit = "Permissions.SystemConfiguration.SiteBaseInfo.Edit";
            }

            public static class Payment
            {
                public const string View = "Permissions.SystemConfiguration.Payment.View";
                public const string Edit = "Permissions.SystemConfiguration.Payment.Edit";
            }

            public static class SystemNotificationSound
            {
                public const string View = "Permissions.SystemConfiguration.SystemNotificationSound.View";
                public const string Edit = "Permissions.SystemConfiguration.SystemNotificationSound.Edit";
            }

            public static class UserNotification
            {
                public const string View = "Permissions.SystemConfiguration.UserNotification.View";
                public const string Edit = "Permissions.SystemConfiguration.UserNotification.Edit";
            }

            public static class WithdrawalAndDeposit
            {
                public const string View = "Permissions.SystemConfiguration.WithdrawalAndDeposit.View";
                public const string Edit = "Permissions.SystemConfiguration.WithdrawalAndDeposit.Edit";
            }

            public static class PaymentChannel
            {
                public const string View = "Permissions.SystemConfiguration.PaymentChannel.View";
                public const string Edit = "Permissions.SystemConfiguration.PaymentChannel.Edit";
            }

            public static class QrCodeConf
            {
                public const string View = "Permissions.SystemConfiguration.QrCodeConf.View";
                public const string Edit = "Permissions.SystemConfiguration.QrCodeConf.Edit";
            }
        }

        public static class Organization
        {
            public static class TraderAgents
            {
                public const string View = "Permissions.Organization.TraderAgents.View";
                public const string Create = "Permissions.Organization.TraderAgents.Create";
                public const string Edit = "Permissions.Organization.TraderAgents.Edit";
                public const string Delete = "Permissions.Organization.TraderAgents.Delete";
                public static class Downlines
                {
                    public const string View = "Permissions.Organization.TraderAgents.Downlines.View";
                    public const string Create = "Permissions.Organization.TraderAgents.Downlines.Create";
                    public const string Edit = "Permissions.Organization.TraderAgents.Downlines.Edit";
                    public const string Delete = "Permissions.Organization.TraderAgents.Downlines.Delete";
                }

                public static class PendingReview
                {
                    public const string View = "Permissions.Organization.TraderAgents.PendingReview.View";
                    public const string Review = "Permissions.Organization.TraderAgents.PendingReview.Review";
                }

                public static class BankBook
                {
                    public const string View = "Permissions.Organization.TraderAgents.BankBook.View";
                }

                public static class FrozenRecord
                {
                    public const string View = "Permissions.Organization.TraderAgents.FrozenRecord.View";
                }

                public static class Transfer
                {
                    public const string Create = "Permissions.Organization.TraderAgents.Transfer.Create";
                }

                public static class ChangeBalance
                {
                    public const string Create = "Permissions.Organization.TraderAgents.ChangeBalance.Create";
                }
            }

            public static class Traders
            {
                public const string View = "Permissions.Organization.Traders.View";
                public const string Create = "Permissions.Organization.Traders.Create";
                public const string Edit = "Permissions.Organization.Traders.Edit";
                public const string Delete = "Permissions.Organization.Traders.Delete";

                public static class PendingReview
                {
                    public const string View = "Permissions.Organization.Traders.PendingReview.View";
                    public const string Review = "Permissions.Organization.Traders.PendingReview.Review";
                }

                public static class BankBook
                {
                    public const string View = "Permissions.Organization.Traders.BankBook.View";
                }

                public static class FrozenRecord
                {
                    public const string View = "Permissions.Organization.Traders.FrozenRecord.View";
                }

                public static class Transfer
                {
                    public const string Create = "Permissions.Organization.Traders.Transfer.Create";
                }

                public static class ChangeBalance
                {
                    public const string Create = "Permissions.Organization.Traders.ChangeBalance.Create";
                }
            }
        }

        public static class ShopManagement
        {
            public static class ShopAgents
            {
                public const string View = "Permissions.ShopManagement.ShopAgents.View";
                public const string Create = "Permissions.ShopManagement.ShopAgents.Create";
                public const string Edit = "Permissions.ShopManagement.ShopAgents.Edit";
                public const string Delete = "Permissions.ShopManagement.ShopAgents.Delete";
                public static class Downlines
                {
                    public const string View = "Permissions.ShopManagement.ShopAgents.Downlines.View";
                    public const string Create = "Permissions.ShopManagement.ShopAgents.Downlines.Create";
                    public const string Edit = "Permissions.ShopManagement.ShopAgents.Downlines.Edit";
                    public const string Delete = "Permissions.ShopManagement.ShopAgents.Downlines.Delete";
                }

                public static class PendingReview
                {
                    public const string View = "Permissions.ShopManagement.ShopAgents.PendingReview.View";
                    public const string Review = "Permissions.ShopManagement.ShopAgents.PendingReview.Review";
                }

                public static class BankBook
                {
                    public const string View = "Permissions.ShopManagement.ShopAgents.BankBook.View";
                }

                public static class FrozenRecord
                {
                    public const string View = "Permissions.ShopManagement.ShopAgents.FrozenRecord.View";
                }

                public static class ChangeBalance
                {
                    public const string Create = "Permissions.ShopManagement.ShopAgents.ChangeBalance.Create";
                }
            }

            public static class Shops
            {
                public const string View = "Permissions.ShopManagement.Shops.View";
                public const string Create = "Permissions.ShopManagement.Shops.Create";
                public const string Edit = "Permissions.ShopManagement.Shops.Edit";
                public const string Delete = "Permissions.ShopManagement.Shops.Delete";

                public static class PendingReview
                {
                    public const string View = "Permissions.ShopManagement.Shops.PendingReview.View";
                    public const string Review = "Permissions.ShopManagement.Shops.PendingReview.Review";
                }

                public static class BankBook
                {
                    public const string View = "Permissions.ShopManagement.Shops.BankBook.View";
                }

                public static class FrozenRecord
                {
                    public const string View = "Permissions.ShopManagement.Shops.FrozenRecord.View";
                }

                public static class ChangeBalance
                {
                    public const string Create = "Permissions.ShopManagement.Shops.ChangeBalance.Create";
                }

                public static class ShopGateway
                {
                    public const string View = "Permissions.ShopManagement.Shops.ShopGateway.View";
                    public const string Create = "Permissions.ShopManagement.Shops.ShopGateway.Create";
                    public const string Edit = "Permissions.ShopManagement.Shops.ShopGateway.Edit";
                    public const string Delete = "Permissions.ShopManagement.Shops.ShopGateway.Delete";
                }

                public static class AmountOption
                {
                    public const string View = "Permissions.ShopManagement.Shops.AmountOption.View";
                    public const string Create = "Permissions.ShopManagement.Shops.AmountOption.Create";
                    public const string Delete = "Permissions.ShopManagement.Shops.AmountOption.Delete";
                }

                public static class ApiKey
                {
                    public const string Create = "Permissions.ShopManagement.Shops.ApiKey.Create";
                }
            }
        }

        public static class WithdrawalManagement
        {
            public static class Withdrawals
            {
                public const string View = "Permissions.WithdrawalManagement.Withdrawals.View";
                public const string Create = "Permissions.WithdrawalManagement.Withdrawals.Create";
                public const string SearchUser = "Permissions.WithdrawalManagement.Withdrawals.SearchUser";
            }
            public static class PendingReview
            {
                public const string View = "Permissions.WithdrawalManagement.PendingReview.View";
                public const string Approve = "Permissions.WithdrawalManagement.PendingReview.Approve";
                public const string ForceSuccess = "Permissions.WithdrawalManagement.PendingReview.ForceSuccess";
                public const string ConfirmPayment = "Permissions.WithdrawalManagement.PendingReview.ConfirmPayment";
                public const string Cancel = "Permissions.WithdrawalManagement.PendingReview.Cancel";
                public const string ApproveCancellation = "Permissions.WithdrawalManagement.PendingReview.ApproveCancellation";
            }
            public static class BankOptions
            {
                public const string View = "Permissions.WithdrawalManagement.BankOptions.View";
                public const string Create = "Permissions.WithdrawalManagement.BankOptions.Create";
                public const string Delete = "Permissions.WithdrawalManagement.BankOptions.Delete";
            }
        }

        public static class DepositManagement
        {
            public static class Deposits
            {
                public const string View = "Permissions.DepositManagement.Deposits.View";
                public const string Create = "Permissions.DepositManagement.Deposits.Create";
                public const string SearchUser = "Permissions.DepositManagement.Deposits.SearchUser";
                public const string SearchTraderUser = "Permissions.DepositManagement.Deposits.SearchTraderUser";
            }
            public static class PendingReview
            {
                public const string View = "Permissions.DepositManagement.PendingReview.View";
                public const string Verify = "Permissions.DepositManagement.PendingReview.Verify";
                public const string Cancel = "Permissions.DepositManagement.PendingReview.Cancel";
            }
            public static class DepositBankAccounts
            {
                public const string View = "Permissions.DepositManagement.DepositBankAccounts.View";
                public const string Create = "Permissions.DepositManagement.DepositBankAccounts.Create";
                public const string Delete = "Permissions.DepositManagement.DepositBankAccounts.Delete";
            }
        }

        public static class QrCodeManagement
        {
            public static class Manual
            {
                public const string View = "Permissions.QrCodeManagement.Manual.View";
                public const string Create = "Permissions.QrCodeManagement.Manual.Create";
                public const string Edit = "Permissions.QrCodeManagement.Manual.Edit";

                public const string SearchTrader = "Permissions.QrCodeManagement.Manual.SearchTrader";
                public const string SearchShop = "Permissions.QrCodeManagement.Manual.SearchShop";

                public const string Enable = "Permissions.QrCodeManagement.Manual.Enable";
                public const string Disable = "Permissions.QrCodeManagement.Manual.Disable";
                public const string StartPairing = "Permissions.QrCodeManagement.Manual.StartPairing";
                public const string StopPairing = "Permissions.QrCodeManagement.Manual.StopPairing";
                public const string ResetRiskControlData = "Permissions.QrCodeManagement.Manual.ResetRiskControlData";


                public static class PendingReview
                {
                    public const string View = "Permissions.QrCodeManagement.Manual.PendingReview.View";
                    public const string Approve = "Permissions.QrCodeManagement.Manual.PendingReview.Approve";
                    public const string Reject = "Permissions.QrCodeManagement.Manual.PendingReview.Reject";
                }

                public static class CodeData
                {
                    public const string View = "Permissions.QrCodeManagement.Manual.CodeData.View";
                    public const string Edit = "Permissions.QrCodeManagement.Manual.CodeData.Edit";
                }
            }
        }

        public static class OrderManagement
        {
            public static class PlatformOrders
            {
                public const string View = "Permissions.OrderManagement.PlatformOrders.View";
                public const string Create = "Permissions.OrderManagement.PlatformOrders.Create";
                public const string ConfirmPayment = "Permissions.OrderManagement.PlatformOrders.ConfirmPayment";
            }

            public static class RunningAccountRecords
            {
                public const string View = "Permissions.OrderManagement.RunningAccountRecords.View";
            }
        }


        public static bool IsPermissionExist(string permission)
        {
            var fieldInfos = new List<System.Reflection.FieldInfo>();

            fieldInfos.AddRange(typeof(Permissions.Dashboards).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.Administration.Managers).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Administration.Roles).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.SiteBaseInfo).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.Payment).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.SystemNotificationSound).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.UserNotification).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.WithdrawalAndDeposit).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.PaymentChannel).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.QrCodeConf).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents.Downlines).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents.BankBook).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents.FrozenRecord).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents.Transfer).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents.ChangeBalance).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.Organization.Traders).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.Traders.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.Traders.BankBook).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.Traders.FrozenRecord).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.Traders.Transfer).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.Traders.ChangeBalance).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.ShopManagement.ShopAgents).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.ShopAgents.Downlines).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.ShopAgents.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.ShopAgents.BankBook).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.ShopAgents.FrozenRecord).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.ShopAgents.ChangeBalance).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.BankBook).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.FrozenRecord).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.ChangeBalance).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.ShopGateway).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.AmountOption).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.ApiKey).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.WithdrawalManagement.Withdrawals).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.WithdrawalManagement.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.WithdrawalManagement.BankOptions).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.DepositManagement.Deposits).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.DepositManagement.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.DepositManagement.DepositBankAccounts).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.QrCodeManagement.Manual).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.QrCodeManagement.Manual.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.QrCodeManagement.Manual.CodeData).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.OrderManagement.PlatformOrders).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.OrderManagement.RunningAccountRecords).GetFields().ToList());

            bool isExist = false;
            foreach (var fieldInfo in fieldInfos)
            {
                if ((string)fieldInfo.GetValue(null) == permission)
                {
                    isExist = true;
                    break;
                }
            }

            return isExist;
        }

        public static List<string> GetPermissions()
        {
            var fieldInfos = new List<System.Reflection.FieldInfo>();

            fieldInfos.AddRange(typeof(Permissions.Dashboards).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.Administration.Managers).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Administration.Roles).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.SiteBaseInfo).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.Payment).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.SystemNotificationSound).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.UserNotification).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.WithdrawalAndDeposit).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.PaymentChannel).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.SystemConfiguration.QrCodeConf).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents.Downlines).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents.BankBook).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents.FrozenRecord).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents.Transfer).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.TraderAgents.ChangeBalance).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.Organization.Traders).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.Traders.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.Traders.BankBook).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.Traders.FrozenRecord).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.Traders.Transfer).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.Organization.Traders.ChangeBalance).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.ShopManagement.ShopAgents).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.ShopAgents.Downlines).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.ShopAgents.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.ShopAgents.BankBook).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.ShopAgents.FrozenRecord).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.ShopAgents.ChangeBalance).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.BankBook).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.FrozenRecord).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.ChangeBalance).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.ShopGateway).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.AmountOption).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.ShopManagement.Shops.ApiKey).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.WithdrawalManagement.Withdrawals).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.WithdrawalManagement.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.WithdrawalManagement.BankOptions).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.DepositManagement.Deposits).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.DepositManagement.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.DepositManagement.DepositBankAccounts).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.QrCodeManagement.Manual).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.QrCodeManagement.Manual.PendingReview).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.QrCodeManagement.Manual.CodeData).GetFields().ToList());

            fieldInfos.AddRange(typeof(Permissions.OrderManagement.PlatformOrders).GetFields().ToList());
            fieldInfos.AddRange(typeof(Permissions.OrderManagement.RunningAccountRecords).GetFields().ToList());

            var result = new List<string>();

            foreach (var fieldInfo in fieldInfos)
            {
                result.Add((string)fieldInfo.GetValue(null));
            }

            return result;
        }
    }
}
