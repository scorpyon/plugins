
using System;
using System.Collections.Generic;

using CodeHatch.Build;
using CodeHatch.Engine.Networking;

using CodeHatch.Engine.Core.Networking;
using CodeHatch.Blocks;
using CodeHatch.Blocks.Networking.Events;
using CodeHatch.Networking.Events.Entities;
using CodeHatch.Networking.Events.Entities.Players;
using CodeHatch.Networking.Events.Players;
using CodeHatch.Networking.Events.Social;

namespace Oxide.Plugins
{
    [Info("WarTime", "Fraccas", "0.1")]
    public class WarTime : ReignOfKingsPlugin {
	
        //#region Configuration Data
        //bool configChanged;
        //bool configCreated;
        //string admin1, admin2, admin3;
        //string punish = "kick";
        //string banTime = "1";
		
        //string chatPrefix = "WarTime";
        //string message;
        //bool warOn = false;
        //#endregion
		
		
        //void Loaded()
        //{
        //    LoadConfigData();
        //    cmd.AddChatCommand("wartime", this, "WarTimeCommand");
        //    cmd.AddChatCommand("showwartime", this, "CheckWarTimeCommand");
        //}
		
        //[ChatCommand("wartime")]
        //private void WarTimeCommand(Player player, string cmd, string[] args)
        //{
        //    PrintToChat(player.DisplayName + " - " + admin1);
        //    if (player.DisplayName == admin1 || player.DisplayName == admin2 || player.DisplayName == admin3) {
        //        if (warOn) {
        //            PrintToChat("It is now a time of Peace! Do not siege!");
        //            warOn = false;
        //        } else {
        //            PrintToChat("It is now a time of War! You may now siege!");
        //            warOn = true;
        //        }
        //    }
        //}
		
        //[ChatCommand("checkwartime")]
        //private void CheckWarTimeCommand(Player player, string cmd, string[] args)
        //{
        //    if (warOn) {
        //        PrintToChat(player, "It is now a time of War! You may siege!");
        //    } else {
        //        PrintToChat(player, "It is now a time of Peace! Do not siege!");
        //    }
        //}
	
        //void OnCubeTakeDamage(CubeDamageEvent e)
        //{
        //    if (warOn) {
        //        if (e.Damage.Amount > 50) {
        //            bool treb = e.Damage.Damager.name.ToString().Contains("Trebuchet");
        //            bool ballista = e.Damage.Damager.name.ToString().Contains("Ballista");
        //            if (treb || ballista) {
        //                if (e.Damage.DamageSource.Owner is Player && e.Entity.Owner is Player) {
        //                    message = e.Damage.DamageSource.Owner.Name + " has done " + e.Damage.Amount.ToString() + " to someone's property with a " + e.Damage.Damager.name.ToString() + "!";
        //                    message = chatPrefix + ": " + message;

        //                    PrintToChat(message);
        //                }
        //            }
        //        }
        //    } else { 
        //        if (e.Damage.Amount > 50 && e.Damage.DamageSource.Owner is Player) {
        //                bool treb = e.Damage.Damager.name.ToString().Contains("Trebuchet");
        //                bool ballista = e.Damage.Damager.name.ToString().Contains("Ballista");
        //                if (treb || ballista) {
        //                    message = "A base was sieged during Peace Time. The attacker was kicked from the server!";
        //                    message = chatPrefix + ": " + message;
        //                    PrintToChat(message); 
					
        //                    if (punish == "kick")
        //                        Server.Kick(e.Damage.DamageSource.Owner, "Sieging during Peace Times!");
        //                    if (punish == "ban") {
        //                        int time = Int32.Parse(banTime);
        //                        Server.Ban(e.Damage.DamageSource.Owner, time, "Sieging during Peace Times!");
        //                    }
        //                }
        //        }
        //    }
        //}
		
        //protected override void LoadDefaultConfig()
        //{
        //    configCreated = true;
        //    Warning("New configuration file created.");
        //}
		
        //void Warning(string msg) => PrintWarning($"{Title} : {msg}");
        //void LoadConfigData()
        //{
        //    // Plugin settings
        //    admin1 = Convert.ToString(GetConfigValue("Settings", "Admin1", "default"));
        //    admin2 = Convert.ToString(GetConfigValue("Settings", "Admin2", "default"));
        //    admin3 = Convert.ToString(GetConfigValue("Settings", "Admin3", "default"));
			
        //    punish = Convert.ToString(GetConfigValue("Settings", "Punish", "kick"));
        //    banTime = Convert.ToString(GetConfigValue("Settings", "BanTime", "1"));

        //    if (configChanged)
        //    {
        //        Warning("The configuration file was updated!");
        //        SaveConfig();
        //    }
        //}

        //object GetConfigValue(string category, string setting, object defaultValue)
        //{
        //    var data = Config[category] as Dictionary<string, object>;
        //    object value;
        //    if (data == null)
        //    {
        //        data = new Dictionary<string, object>();
        //        Config[category] = data;
        //        configChanged = true;
        //    }
        //    if (!data.TryGetValue(setting, out value))
        //    {
        //        value = defaultValue;
        //        data[setting] = value;
        //        configChanged = true;
        //    }
            
        //    return value;
        //}
    }
}
