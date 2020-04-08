using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Roles;

namespace WebMVC.Areas.Identity.Pages.Account
{
    public static class LoginPanelViewData
    {
        public static string Manager => "管理员";

        public static string Trader => "交易员";

        public static string TraderAgent => "代理";

        public static string Shop => "商户";

        public static string ShopAgent => "商户代理";

        public static string GetLoginTitle(string panel)
        {
            string role = string.Empty;
            if (panel.Equals(Roles.Manager, StringComparison.InvariantCultureIgnoreCase))
            {
                role = Manager;
            }
            else if (panel.Equals(Roles.Trader, StringComparison.InvariantCultureIgnoreCase))
            {
                role = Trader;
            }
            else if (panel.Equals(Roles.TraderAgent, StringComparison.InvariantCultureIgnoreCase))
            {
                role = TraderAgent;
            }
            else if (panel.Equals(Roles.Shop, StringComparison.InvariantCultureIgnoreCase))
            {
                role = Shop;
            }
            else if (panel.Equals(Roles.ShopAgent, StringComparison.InvariantCultureIgnoreCase))
            {
                role = ShopAgent;
            }

            return "登入" + role + "后台";
        }
    }
}
