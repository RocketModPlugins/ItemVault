using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace fr34kyn01535.ItemVault
{
    public class ItemVaultPlayerComponent : UnturnedPlayerComponent
    {
        public DateTime LastUsed = DateTime.MinValue;
        public List<Item> OldItems = new List<Item>();
        public Items VaultItems;
    }
}