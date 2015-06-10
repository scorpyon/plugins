using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Timers;
using CodeHatch.Engine.Networking;
using CodeHatch.Engine.Core.Networking;
using CodeHatch.Thrones.SocialSystem;
using CodeHatch.Common;
using CodeHatch.Permissions;
using Oxide.Core;
using CodeHatch.Networking.Events.Entities;
using CodeHatch.Networking.Events.Entities.Players;
using CodeHatch.Networking.Events.Players;

namespace Oxide.Plugins
{
    [Info("GuildInfo", "Scorpyon", "1.0.2")]
    public class GuildInfo : ReignOfKingsPlugin
    {
        private Collection<Collection<string>> guildInformation = new Collection<Collection<string>>();
        
        void Log(string msg) => Puts($"{Title} : {msg}");

        // SAVE DATA ===============================================================================================

        private void LoadGuildInfoData()
        {
            guildInformation = Interface.GetMod().DataFileSystem.ReadObject<Collection<Collection<string>>>("GuildInformationSave");
        }

        private void SaveGuildInfoData()
        {
            Log("Saving data for guildinfo");
            Interface.GetMod().DataFileSystem.WriteObject("GuildInformationSave", guildInformation);
        }

        void Loaded()
        {
            guildInformation = new Collection<Collection<string>>();
            LoadGuildInfoData();
        }


        // ===========================================================================================================

        
        private void OnPlayerConnected(Player player)
        {
            if(player == null) return;
            UpdateGuildStatusForPlayer(player);
        }

        private void UpdateGuildStatusForPlayer(Player player)
        {
            if(player == null) return;
            var playerName = player.Name.ToLower();
            var thisGuild = PlayerExtensions.GetGuild(player);
            if(thisGuild == null) return;
            var guildName = PlayerExtensions.GetGuild(player).Name;

            if(guildInformation == null) return; 

            foreach(var guild in guildInformation)
            {
                // Is the player in here?
                foreach(var playerFound in guild)
                {
                    if(playerFound == playerName)
                    {
                        // Are they in the right guild
                        if(guild[0] == guildName) 
                        {
                            SaveGuildInfoData();
                            return;
                        }
                        //Otherwise remove them from this guild
                        else {
                            guild.Remove(playerFound);
                            SaveGuildInfoData();
                            UpdateGuildStatusForPlayer(player);
                            return;
                        }
                    }
                }
            }

            // We didn't find the player so add him.

            //Does his guild already exist?
            foreach(var guild in guildInformation)
            {
                foreach(var member in guild)
                {
                    if(member == guildName)
                    {
                        guild.Add(playerName);
                        SaveGuildInfoData();
                        return;
                    }
                }
            }

            //No guild and no record so create one
            var newMemberList = new Collection<string>();
            newMemberList.Add(guildName);
            newMemberList.Add(playerName);
            guildInformation.Add(newMemberList);
            SaveGuildInfoData();

            //foreach(var a in guildInformation){
            //    foreach(var b in a){
            //        PrintToChat(b);
            //    }
            //}
            
        }

        [ChatCommand("guildinfo")]
        private void ListGuildMembers(Player player, string cmd, string[] playerArray)
        {
            var guildName = "";
            
            // Check the player has specified a player name
            if (PlayerHasNotenteredAPlayerName(playerArray))
            {
                PrintToChat(player, "[FF0000]Guild Master[FFFFFF] : To find out about a player's guild, type [00FF00]/guildinfo [FF00FF]<playername>.");
                return;
            }

            // Get the full player name
            var playerName = ConvertArrayToString(playerArray).ToLower();

            // Find the chosen target player
            Player targetPlayer = Server.GetPlayerByName(playerName);

            //Update if player is online
            if(targetPlayer != null)
            {
                UpdateGuildStatusForPlayer(targetPlayer);
            }

            // Search the info list for the guild info
            foreach(var guild in guildInformation)
            {
                foreach(var playerFound in guild)
                {
                    // If we find the player
                    if(playerFound == playerName)
                    {
                        guildName = guild[0];
                        var playerList = guild;
//                        playerList.RemoveAt(0);
                        if(playerList.Count > 0)
                        {
                            //  Output the members of the guild.
                            PrintToChat(player, "[FF0000]Guild Master[FFFFFF] : [00FF00]" + guildName + "[FFFFFF] has " + (playerList.Count-1) + " members:");
                            for (var i=1; i<playerList.Count; i++)
                            {
                                var onlineStatus = "[008888] (Offline)";
                                if (Server.GetPlayerByName(playerList[i]) != null) onlineStatus = "[FF0000] (Online)";
                                PrintToChat(player, "[00FF00]" + playerList[i] + onlineStatus);
                            }
                        }
                        return;
                    }
                }
            }

            // Save data
            SaveGuildInfoData();
        }


        private string ConvertArrayToString(string[] guildArray)
        {
            var guildName = guildArray[0];
            if (guildArray.Length > 1)
            {
                for (var i = 1; i < guildArray.Length; i++)
                {
                    guildName = guildName + " " + guildArray[i];
                }
            }
            return guildName;
        }

        private bool PlayerHasNotenteredAPlayerName(string[] playerArray)
        {
            if (playerArray.Length < 1)
            {
                return true;
            }
            return false;
        }

	}
}
