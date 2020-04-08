using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Roles
{
    public static class Roles
    {
        //Base roles
        public const string Manager = "Manager";
        public const string Trader = "Trader";
        public const string TraderAgent = "TraderAgent";
        public const string Shop = "Shop";
        public const string ShopAgent = "ShopAgent";

        //Additional roles
        public const string TraderAgentWithGrantRight = "TraderAgentWithGrantRight";
        public const string ShopAgentWithGrantRight = "ShopAgentWithGrantRight";
        public const string UserReviewed = "UserReviewed";

        //Inital Created Roles
        public const string Admin = "Admin";

        public static bool IsPanelRole(string role)
        {
            if (role != null)
            {
                if (role.Equals(Manager, StringComparison.InvariantCultureIgnoreCase) ||
                    role.Equals(Trader, StringComparison.InvariantCultureIgnoreCase) ||
                    role.Equals(TraderAgent, StringComparison.InvariantCultureIgnoreCase) ||
                    role.Equals(Shop, StringComparison.InvariantCultureIgnoreCase) ||
                    role.Equals(ShopAgent, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool IsManagerRole(string roleName)
        {
            if (roleName != null)
            {
                if (roleName.Equals(Manager, StringComparison.InvariantCultureIgnoreCase) ||
                    roleName.Equals(Trader, StringComparison.InvariantCultureIgnoreCase) ||
                    roleName.Equals(TraderAgent, StringComparison.InvariantCultureIgnoreCase) ||
                    roleName.Equals(Shop, StringComparison.InvariantCultureIgnoreCase) ||
                    roleName.Equals(ShopAgent, StringComparison.InvariantCultureIgnoreCase) ||

                    roleName.Equals(TraderAgentWithGrantRight, StringComparison.InvariantCultureIgnoreCase) ||
                    roleName.Equals(ShopAgentWithGrantRight, StringComparison.InvariantCultureIgnoreCase) ||
                    roleName.Equals(UserReviewed, StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        public static bool GetManagerRole(List<string> roleNames, out string managerRoleName)
        {
            foreach (var roleName in roleNames)
            {
                if (IsManagerRole(roleName))
                {
                    managerRoleName = roleName;
                    return true;
                }
            }
            managerRoleName = null;
            return false;
        }

    }
}
