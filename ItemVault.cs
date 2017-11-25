using I18N.West;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using System;
using System.Linq;
using System.Reflection;

namespace fr34kyn01535.ItemVault
{
    public class ItemVault : RocketPlugin<ItemVaultConfiguration>
    {
        public static ItemVault Instance;

        protected override void Load()
        {
            ConsoleColor r = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Logger.Log("################# IMPORTANT #################");
            Console.ForegroundColor = ConsoleColor.White;
            Logger.Log(((AssemblyConfigurationAttribute)(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false).FirstOrDefault())).Configuration);
            Console.ForegroundColor = ConsoleColor.Red;
            Logger.Log("################# IMPORTANT #################");
            Console.ForegroundColor = r;
            ItemVault.Instance = this;
            if (Configuration.Instance.Enabled)
            {
                CP1250 cP1250 = new CP1250();
                Database.CheckSchema();
            }
        }
    }
}