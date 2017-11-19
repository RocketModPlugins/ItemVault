using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fr34kyn01535.ItemVault
{
    internal class CommandVault : IRocketCommand
    {
        public List<string> Aliases
        {
            get
            {
                return new List<string>();
            }
        }

        public string Help
        {
            get
            {
                return "Store items in your vault";
            }
        }

        public string Name
        {
            get
            {
                return "vault";
            }
        }

        public string Syntax
        {
            get
            {
                return "";
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "itemvault.vault" };
            }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (!ItemVault.Instance.Configuration.Instance.Enabled)
            {
                return;
            }
            ItemVaultPlayerComponent component = player.Player.transform.GetComponent<ItemVaultPlayerComponent>();
            Items inventory = new Items(7);
            double totalSeconds = (DateTime.Now - component.LastUsed).TotalSeconds;
            if (totalSeconds < 10)
            {
                UnturnedChat.Say(caller, string.Concat("Please wait another ", 10 - (int)totalSeconds, " seconds before you use this command again"));
                return;
            }
            component.LastUsed = DateTime.Now;
            IEnumerable<string>  permissions = player.GetPermissions().Select(a => a.Name).Where(p => p.ToLower().Contains("vault.") && !p.Equals("vault.", StringComparison.InvariantCultureIgnoreCase));
            byte t;
            byte size = permissions.Select(p => p.Split('.')[1]).Where(p => byte.TryParse(p, out t)).Select(p => byte.Parse(p)).OrderByDescending(p => p).FirstOrDefault();
#if DEBUG
            Logger.Log("Found Permissions: "+String.Join(", ",permissions.ToArray()));
            Logger.Log("Vaultsize taken: "+ size);
#endif
            inventory.resize(8, size);

            List<ItemJar> items = Database.RetrieveItems(player.CSteamID);
            foreach (ItemJar item in items)
            {
                ItemAsset itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, item.item.id);
                bool failed = false;
                try
                {

                    if ((itemAsset.size_y+item.y-1) > size)
                    {
                        byte x;
                        byte y;
                        byte rot;
                        failed = !inventory.tryFindSpace(item.x,item.y,out x,out y,out rot);
                        if(!failed)
                            inventory.addItem(x, y, rot, item.item);
                    }
                    else
                    {
                        inventory.addItem(item.x, item.y, item.rot, item.item);
                    }

                }
                catch (Exception)
                {
                    failed = true;
                }
                if(failed)
                    UnturnedChat.Say(caller, string.Concat("Failed to retrieve ", itemAsset.itemName, ", free some space and try again!"));
            }

            inventory.onItemAdded = (byte page, byte index, ItemJar jar) => {
                Database.AddItem(player.CSteamID, jar);
            };
            
            inventory.onItemRemoved = (byte page, byte index, ItemJar jar) => {
                Database.DeleteItem(player.CSteamID, jar);
            };


            inventory.onItemUpdated = (byte page, byte index, ItemJar jar) =>
            {
                Database.UpdateItem(player.CSteamID, jar);
            };

            player.Player.inventory.updateItems(7, inventory);
            player.Player.inventory.sendStorage();
        }

        public static bool TryParse(string value, out EUseableType result)
        {
            bool flag;
            try
            {
                result = (EUseableType)Enum.Parse(typeof(EUseableType), value);
                flag = true;
            }
            catch
            {
                result = 0;
                flag = false;
            }
            return flag;
        }
    }
}
