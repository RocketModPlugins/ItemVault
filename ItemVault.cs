using I18N.West;
using Rocket.Core.Plugins;

namespace fr34kyn01535.ItemVault
{
    public class ItemVault : RocketPlugin<ItemVaultConfiguration>
    {
        public static ItemVault Instance;

        protected override void Load()
        {
            ItemVault.Instance = this;
            if (Configuration.Instance.Enabled)
            {
                CP1250 cP1250 = new CP1250();
                Database.CheckSchema();
            }
        }
    }
}