using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;

namespace fr34kyn01535.ItemVault
{
    internal class Database
    {
        private static MySqlConnection createConnection()
        {
            MySqlConnection result = null;
            try
            {
                result = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", ItemVault.Instance.Configuration.Instance.DatabaseAddress, ItemVault.Instance.Configuration.Instance.DatabaseName, ItemVault.Instance.Configuration.Instance.DatabaseUsername, ItemVault.Instance.Configuration.Instance.DatabasePassword, ItemVault.Instance.Configuration.Instance.DatabasePort));
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return result;
        }
        public static void CheckSchema()
        {
            try
            {
                MySqlConnection mySqlConnection = Database.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = "show tables like '" + ItemVault.Instance.Configuration.Instance.DatabaseTableName + "'";
                mySqlConnection.Open();
                if (mySqlCommand.ExecuteScalar() == null)
                {
                    mySqlCommand.CommandText = "CREATE TABLE `" + ItemVault.Instance.Configuration.Instance.DatabaseTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`durability` int(3) NOT NULL,`stacksize` int(11) NULL,`x` int(11) NULL,`y` int(11) NULL,`rotation` int(11) NULL,`itemid` int(4) NOT NULL,`metadata` varchar(255) NOT NULL,`csteamid` varchar(32) NOT NULL,PRIMARY KEY (`id`)) ";
                    mySqlCommand.ExecuteNonQuery();
                }
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        internal static List<ItemJar> RetrieveItems(CSteamID csteamid)
        {
            List<ItemJar> list = new List<ItemJar>();
            try
            {
                int num = 0;
                MySqlConnection mySqlConnection = Database.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = string.Concat(new object[]
                {
                    "select count(*) from `",
                    ItemVault.Instance.Configuration.Instance.DatabaseTableName,
                    "` where `csteamid` = ",
                    csteamid,
                    ";"
                });
                mySqlConnection.Open();
                object obj = mySqlCommand.ExecuteScalar();
                if (obj != null)
                {
                    num = int.Parse(obj.ToString());
                }
                mySqlConnection.Close();
                if (num != 0)
                {
                    mySqlCommand.CommandText = string.Concat(new object[]
                    {
                        "select * from `",
                        ItemVault.Instance.Configuration.Instance.DatabaseTableName,
                        "` where `csteamid` = '",
                        csteamid,
                        "' order by id;"
                    });
                    mySqlConnection.Open();
                    MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
                    while (mySqlDataReader.Read())
                    {
                        list.Add(new ItemJar((byte)mySqlDataReader.GetInt32("x"), (byte)mySqlDataReader.GetInt32("y"), (byte)mySqlDataReader.GetInt32("rotation"), new Item(mySqlDataReader.GetUInt16("itemid"), true)
                        {
                            durability = (byte)mySqlDataReader.GetInt32("durability"),
                            metadata = Convert.FromBase64String(mySqlDataReader.GetString("metadata")),
                            amount = (byte)mySqlDataReader.GetInt32("stacksize")
                        }));
                    }
                    mySqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return list;
        }

        internal static void UpdateItem(CSteamID cSteamID, ItemJar item)
        {
            try
            {
                MySqlConnection mySqlConnection = Database.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                string text = (item.item.metadata == null) ? "" : Convert.ToBase64String(item.item.metadata);
                mySqlCommand.CommandText = string.Concat(new object[]
                {
                    "update `",
                    ItemVault.Instance.Configuration.Instance.DatabaseTableName,
                    "` set `metadata`='"+text+"' where `csteamid`='"+cSteamID+"' and `itemid` = "+ item.item.id+" limit 1;"
                });
                mySqlConnection.Open();
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        internal static bool AddItem(CSteamID csteamid, ItemJar item)
        {
            try
            {
                MySqlConnection mySqlConnection = Database.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                string text = (item.item.metadata == null) ? "" : Convert.ToBase64String(item.item.metadata);
                mySqlCommand.CommandText = string.Concat(new object[]
                {
                    "insert into `",
                    ItemVault.Instance.Configuration.Instance.DatabaseTableName,
                    "` (`csteamid`,`durability`,`x`,`y`,`rotation`,`metadata`,`itemid`,`stacksize`) values(",
                    csteamid,
                    ",",
                    item.item.durability,
                    ",",
                    item.x,
                    ",",
                    item.y,
                    ",",
                    item.rot,
                    ",'",
                    text,
                    "',",
                    item.item.id,
                    ",",
                    item.item.amount,
                    ");"
                });
                mySqlConnection.Open();
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return false;
        }
        internal static bool DeleteItem(CSteamID csteamid, ItemJar item)
        {
            try
            {
                MySqlConnection mySqlConnection = Database.createConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                string text = (item.item.metadata == null) ? "" : Convert.ToBase64String(item.item.metadata);
                mySqlCommand.CommandText = string.Concat(new object[]
                {
                    "delete from `",
                    ItemVault.Instance.Configuration.Instance.DatabaseTableName,
                    "` where csteamid = ",
                    csteamid,
                    " and durability = ",
                    item.item.durability,
                    " and metadata = '",
                    text,
                    "' and itemid = ",
                    item.item.id,
                    " and stacksize = ",
                    item.item.amount,
                    " limit 1;"
                });
                mySqlConnection.Open();
                mySqlCommand.ExecuteScalar();
                mySqlConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return false;
        }
    }
}
