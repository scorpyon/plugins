using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
//using System.Timers;
using CodeHatch.Engine.Networking;
//using CodeHatch.Engine.Core.Networking;
//using CodeHatch.Thrones.SocialSystem;
using CodeHatch.Common;
//using CodeHatch.Permissions;
using Oxide.Core;
using CodeHatch.Networking.Events;
using CodeHatch.Networking.Events.Entities;
using CodeHatch.Networking.Events.Entities.Players;
using CodeHatch.Networking.Events.Players;
//using CodeHatch.ItemContainer;
//using CodeHatch.UserInterface.Dialogues;
//using CodeHatch.Inventory.Blueprints.Components;

namespace Oxide.Plugins
{
    [Info("Rank Tracker", "Scorpyon", "1.0.1")]
    public class RankTracker : ReignOfKingsPlugin
    {
        void Log(string msg) => Puts($"{Title} : {msg}");
		
		private Dictionary<string,string> rankList = new Dictionary<string,string>();
		
#region User Commands

        // Teleport to another location /teletoloc <xcoord> <ycoord> <zcoord>
        // [ChatCommand("teletoloc")]
        // private void AdminTeleportToLocation(Player player, string cmd, string[] input)
        // {
            // TeleportToLocation(player, cmd, input);
        // }

#endregion


#region Save and Load Data Methods

        // SAVE DATA ===============================================================================================
        private void LoadRankData()
        {
            tradeDefaults = Interface.GetMod().DataFileSystem.ReadObject<Collection<string[]>>("SavedTradeDefaults");
        }

        private void SaveRankData()
        {
            Interface.GetMod().DataFileSystem.WriteObject("SavedTradeDefaults", tradeDefaults);
        }
		
		private void OnPlayerConnected(Player player)
		{
			CheckRankExists(player);
			
			// Save the trade data
            SaveTradeData();
		}
		
		
		private void CheckRankExists(Player player)
		{
			//Check if the player has a wallet yet
			if(rankList.Count < 1) rank.Add(player.Name.ToLower(),GetRank(0));
			if(!rankList.ContainsKey(player.Name.ToLower()))
			{
				rankList.Add(player.Name.ToLower(),GetRank(0));
			}
		}
		
        void Loaded()
        {
            LoadRankData();
            
            // Save the trade data
            SaveRankData();
        }
        // ===========================================================================================================
		
#endregion

#region Private Methods

//        private void OnPlayerChat(PlayerChatEvent chatEvent)
        // private void OnPlayerChat(PlayerEvent chatEvent)
        // {
            // if(chatEvent != null && chatEvent.Player != null)
            // {
                // chatEvent.Player.DisplayNameFormat = "[00FF00](KING) [FFFF00]%name%[FFFFFF]";
				
            // }
        // }

        private void GetRank(int xp)
        {
			
        }

#endregion

    }
}
