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
using CodeHatch.UserInterface.Dialogues;
//using CodeHatch.Inventory.Blueprints.Components;

namespace Oxide.Plugins
{
    [Info("Rank Tracker", "Scorpyon", "1.0.1")]
    public class RankTracker : ReignOfKingsPlugin
    {
        void Log(string msg) => Puts($"{Title} : {msg}");
		private Dictionary<string,int> rankList = new Dictionary<string,int>();
		
#region User Commands

        // View the server player ranks
        [ChatCommand("ranks")]
        private void ShowPlayerRanks(Player player, string cmd)
        {
            ShowPlayerRanksForServer(player, cmd);
        }
		
		// Give a player XP (Admin)
        [ChatCommand("giverankxp")]
        private void GivePlayerXp(Player player, string cmd, string[] input)
        {
            GivePlayerSomeXp(player, cmd, input);
        }
		
		

#endregion


#region Save and Load Data Methods

        // SAVE DATA ===============================================================================================
        private void LoadRankData()
        {
            rankList = Interface.GetMod().DataFileSystem.ReadObject<Dictionary<string,int>>("SavedRankList");
        }

        private void SaveRankData()
        {
            Interface.GetMod().DataFileSystem.WriteObject("SavedRankList", rankList);
        }
		
		private void OnPlayerConnected(Player player)
		{
			CheckRankExists(player);

			//Set player's rank
			SetPlayerRank(player);
			
			// Save the trade data
            SaveRankData();
		}
		
		
		private void CheckRankExists(Player player)
		{
			//Check if the player has a wallet yet
			if(rankList.Count < 1) rankList.Add(player.Name.ToLower(),0);
			if(!rankList.ContainsKey(player.Name.ToLower()))
			{
				rankList.Add(player.Name.ToLower(),0);
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

		private void GivePlayerSomeXp(Player player, string cmd, string[] input)
		{
			if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only admins can use this command.");
                return;
            }
			
			if(input.Length < 2)
			{
				PrintToChat(player, "Please enter a player name and a valid amount of XP to give.");
                return;
			}
			
			var playerName = input[0];
			var target = Server.GetPlayerByName(playerName);
			
			if(target == null)
			{	
				PrintToChat(player, "That player does not seem to be online.");
                return;
			}
			
			PrintToChat(input[1].ToString());
			int amount;
			if(Int32.TryParse(input[1], out amount) == false)
			{
				PrintToChat(player, "That was not a recognised amount!");
                return;
			}
			
			AddRankXp(target, amount);
			//PrintToChat(player, playerName + " has been given " + amount.ToString() + " XP!");
		}

		
		//        private void OnPlayerChat(PlayerChatEvent chatEvent)
        // private void OnPlayerChat(PlayerEvent chatEvent)
        // {
            // if(chatEvent != null && chatEvent.Player != null)
            // {
                // chatEvent.Player.DisplayNameFormat = "[00FF00](KING) [FFFF00]%name%[FFFFFF]";
				
            // }
        // }
		
		private string GetRank(int xp)
        {
			var rank = "[003333]Civilian[FFFFFF]";
			
			if(xp > 1000000) return "[003333]High Commander[FFFFFF]";
			if(xp > 500000) return "[003333]Commander[FFFFFF]";
			if(xp > 200000) return "[003333]Chancellor[FFFFFF]";
			if(xp > 100000) return "[003333]Baron[FFFFFF]";
			if(xp > 50000) return "[003333]Minor Baron[FFFFFF]";
			if(xp > 20000) return "[003333]Lord[FFFFFF]";
			if(xp > 10000) return "[003333]Minor Lord[FFFFFF]";
			if(xp > 5000) return "[003333]Knight[FFFFFF]";
			if(xp > 2000) return "[003333]Squire[FFFFFF]";
			if(xp > 1000) return "[003333]Knave[FFFFFF]";
			if(xp > 500) return "[003333]Manservant[FFFFFF]";
			if(xp > 250) return "[003333]Servant[FFFFFF]";
			if(xp > 100) return "[003333]Serf[FFFFFF]";
			
			return rank;
        }
		
		private void AddRankXp(Player player, int amount)
		{	
			CheckRankExists(player);
			var currentXP = rankList[player.Name.ToLower()];
			currentXP = currentXP + amount;
			if(currentXP > 21000000) currentXP = 21000000;
			rankList[player.Name.ToLower()] = currentXP;
			
			SetPlayerRank(player);
			SaveRankData();
		}
		
		private void SetPlayerRank(Player player)
		{
			CheckRankExists(player);
			var playerRank = Capitalise(GetRank(rankList[player.Name.ToLower()]));
			player.DisplayNameFormat = "(" + playerRank + ") %name%";
		}
		
		private void ShowPlayerRanksForServer(Player player, string cmd)
        {
			var singlePage = false;
			var itemText = "";
			var itemsPerPage = 30;
			var currentItemCount = 0;
			
             // Are there any items on the store?
            if(rankList.Count == 0)
            {
                PrintToChat(player, "[FF0000]Rank Master[FFFFFF] : No-one has acquired any ranks yet.");
                return;
            }

			// Check number of players with a rank
            if(itemsPerPage > rankList.Count) 
			{
				singlePage = true;
				itemsPerPage = rankList.Count;
			}
			
			var i=0;
			foreach(var person in rankList)
            {
				var message = "[FFFFFF]" + Capitalise(person.Key) + " - (" + Capitalise(GetRank(person.Value)) + ")\n";
                itemText = itemText + message;
				i++;
				if(i >= itemsPerPage + currentItemCount) break;
            }
			
			if(singlePage) 
			{
				player.ShowPopup("Duke's Castle Rankings", itemText, "Exit", (selection, dialogue, data) => DoNothing(player, selection, dialogue, data));
				return;
			}
			
            //Display the Popup with the price
			player.ShowConfirmPopup("Duke's Castle Rankings", itemText, "Next Page", "Exit", (selection, dialogue, data) => ContinueWithRankList(player, selection, dialogue, data, itemsPerPage, itemsPerPage));
        }
		
		
		private void ContinueWithRankList(Player player, Options selection, Dialogue dialogue, object contextData,int itemsPerPage, int currentItemCount)
		{
            if (selection != Options.Yes)
            {
                //Leave
                return;
            }
			
			if((currentItemCount + itemsPerPage) > rankList.Count)
			{
				itemsPerPage = rankList.Count - currentItemCount;
			}
            
			var itemText = "";
			var i = currentItemCount;
			foreach(var person in rankList)
            {
			    string message = "[FFFFFF]" + Capitalise(person.Key) + " - (" + Capitalise(GetRank(person.Value)) + ")\n";
                itemText = itemText + message;
				i++;
				if(i >= itemsPerPage + currentItemCount) break;
            }

            currentItemCount = currentItemCount + itemsPerPage;

            // Display the Next page
            if(currentItemCount < rankList.Count)
            {
                player.ShowConfirmPopup("Duke's Castle Rankings", itemText,  "Next Page", "Exit", (options, dialogue1, data) => ContinueWithRankList(player, options, dialogue1, data, itemsPerPage, currentItemCount));
            }
            else
            {
                PlayerExtensions.ShowPopup(player,"Duke's Castle Rankings", itemText, "Yes",  (newselection, dialogue2, data) => DoNothing(player, newselection, dialogue2, data));
            }
		}

		// Capitalise the Starting letters
		private string Capitalise(string word)
		{
			var finalText = "";
			finalText = Char.ToUpper(word[0]).ToString();
			var spaceFound = 0;
			for(var i=1; i<word.Length;i++)
			{
				if(word[i] == ' ')
				{
					spaceFound = i + 1;
				}
				if(i == spaceFound)
				{
					finalText = finalText + Char.ToUpper(word[i]).ToString();
				}
				else finalText = finalText + word[i].ToString();
			}
			return finalText;
		}
		
		private void DoNothing(Player player, Options selection, Dialogue dialogue, object contextData)
		{
			//Do nothing
		}
		
#endregion

    }
}
