using Rocket.API;
using System;

namespace fr34kyn01535.ItemVault
{
    public class ItemVaultConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public string DatabasePort;
        public bool Enabled;

        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            DatabaseTableName = "itemvault";
            DatabasePort = "3306";
            Enabled = true;
        }
    }
}
