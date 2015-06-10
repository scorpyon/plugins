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
using CodeHatch.ItemContainer;


namespace Oxide.Plugins
{
    [Info("DailyHelper", "Scorpyon", "1.0.1")]
    public class DailyHelper : ReignOfKingsPlugin
    {
        private Collection<string> dailyGivers = new Collection<string>();
        void Log(string msg) => Puts($"{Title} : {msg}");


        // SAVE DATA ===============================================================================================

        private void LoadDailyData()
        {
            dailyGivers = Interface.GetMod().DataFileSystem.ReadObject<Collection<string>>("DailyGiversList");
        }

        private void SaveDailyData()
        {
            Interface.GetMod().DataFileSystem.WriteObject("DailyGiversList", dailyGivers);
        }

        void Loaded()
        {
            LoadDailyData();
        }


        // ===========================================================================================================

        private bool PlayerIsOnTheList(string playerName)
        {
            foreach (var name in dailyGivers)
            {
                if (playerName.ToLower() == name.ToLower()) return true;
            }
            return false;
        }

        private string ConvertArrayToString(string[] textArray)
        {
            var newText = textArray[0];
            if (textArray.Length > 1)
            {
                for (var i = 1; i < textArray.Length; i++)
                {
                    newText = newText + " " + textArray[i];
                }
            }
            return newText;
        }

        [ChatCommand("addgiver")]
        private void AddDailyRewardsGiver(Player player, string cmd, string[] input)
        {
            if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only an admin can assign a new daily resource giver.");
                return;
            }

            var playerName = ConvertArrayToString(input);

            //Player is already a daily giver
            var playerIsGiver = false;
            foreach (var giver in dailyGivers)
            {
                if (giver.ToLower() == playerName.ToLower())
                {
                    playerIsGiver = true;
                }
            }
            if (playerIsGiver)
            {
                PrintToChat(player, "That player is already a daily resource giver.");
                return;
            }

            // Find the chosen target player 
            Player targetPlayer = Server.GetPlayerByName(playerName);

            if (targetPlayer == null)
            {
                PrintToChat(player, "The player will need to be online to add him to the list of daily givers.");
                return;
            }

            //Add the player
            dailyGivers.Add(playerName.ToLower());
            PrintToChat(player, "[00FF00]" + playerName + "[FFFFFF] has been added to the list of daily givers.");

            SaveDailyData();
        }

        [ChatCommand("removegiver")]
        private void RemoveDailyRewardsGiver(Player player, string cmd, string[] input)
        {
            if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only an admin can assign a new daily resource giver.");
                return;
            }

            var playerName = ConvertArrayToString(input);

            // Find the chosen target player 
            foreach (var person in dailyGivers)
            {
                if (playerName.ToLower() == person.ToLower())
                {
                    dailyGivers.Remove(playerName);
                    PrintToChat(player, "[00FF00]" + playerName + "[FFFFFF] has been removed from the list of daily givers.");
                    SaveDailyData();
                    return;
                }
            }
            PrintToChat(player, "That player does not appear to be a resource giver on the list.");
            SaveDailyData();
        }


        [ChatCommand("showgivers")]
        private void ShowDailyRewardsGiver(Player player, string cmd, string[] input)
        {
            if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only an admin can view the resource giver list.");
                return;
            }

            if (dailyGivers.Count <= 0)
            {
                PrintToChat(player, "There are no daily givers currently.");
                return;
            }
            PrintToChat(player, "[FF0000]Server: [FFFFFF]The current list of givers assigned are:");
            foreach (var giver in dailyGivers)
            {
                PrintToChat(player, giver);
            }
        }

        [ChatCommand("givedaily")]
        private void GiveDailyRewards(Player player, string cmd, string[] playerString)
        {
            if (!player.HasPermission("admin") && !PlayerIsOnTheList(player.Name)) { PrintToChat(player, "Only admins or designated vote-reward givers can give daily awards."); return; }
            if (playerString.Length <= 0) { PrintToChat(player, "You need to specifiy a player to give the rewards to."); return; }
            
            // Convert the player name string array to a string             
            var targetPlayerName = playerString[0];             
            if (playerString.Length > 1) 
            {
                for (var i = 1; i < playerString.Length; i++)
                {
                    targetPlayerName = string.Format(targetPlayerName + " {0}", playerString[i]);
                }
            }

            // Find the chosen target player 
            Player targetPlayer = Server.GetPlayerByName(targetPlayerName);

            if (targetPlayer == null)
            {
                PrintToChat(player, "That player doesn't appear to be online right now.");
                return;
            }

            if (targetPlayerName.ToLower() == player.Name.ToLower())
            {
                // Trying to give resources to self!
                if (!player.HasPermission("admin"))
                {
                    PrintToChat(player, "Only admins can give daily resources to themselves!");
                    return;
                }
            }

            var inventory = targetPlayer.GetInventory();

            if (inventory.Contents.FreeSlotCount < 5)
            {
                PrintToChat(player, "That player doesn't have enough space to receive the rewards! Give 'em hell!");
                return;
            }

            // GIVE WOOD 
            var blueprintForName = InvDefinitions.Instance.Blueprints.GetBlueprintForName("Wood", true, true);
            // STACK OF 1000 
            var invGameItemStack = new InvGameItemStack(blueprintForName, 1000, null);
            ItemCollection.AutoMergeAdd(inventory.Contents, invGameItemStack);
            // STACK OF 500 
            invGameItemStack = new InvGameItemStack(blueprintForName, 500, null);
            ItemCollection.AutoMergeAdd(inventory.Contents, invGameItemStack);

            // GIVE STONE 
            blueprintForName = InvDefinitions.Instance.Blueprints.GetBlueprintForName("Stone", true, true);
            // STACK OF 1000 
            invGameItemStack = new InvGameItemStack(blueprintForName, 1000, null);
            ItemCollection.AutoMergeAdd(inventory.Contents, invGameItemStack);
            // STACK OF 500 
            invGameItemStack = new InvGameItemStack(blueprintForName, 500, null);
            ItemCollection.AutoMergeAdd(inventory.Contents, invGameItemStack);

            // GIVE IRON 
            blueprintForName = InvDefinitions.Instance.Blueprints.GetBlueprintForName("Iron", true, true);
            // STACK OF 1000 
            invGameItemStack = new InvGameItemStack(blueprintForName, 1000, null);
            ItemCollection.AutoMergeAdd(inventory.Contents, invGameItemStack);

            // GIVE DIAMOND 
            blueprintForName = InvDefinitions.Instance.Blueprints.GetBlueprintForName("Diamond", true, true);
            // STACK OF 2 
            invGameItemStack = new InvGameItemStack(blueprintForName, 2, null);
            ItemCollection.AutoMergeAdd(inventory.Contents, invGameItemStack);

            PrintToChat("[FF0000]Vote Rewards [FFFFFF]: The daily rewards have been given to [00FF00]" + targetPlayerName + "[FFFFFF] for voting.");
        }
    }
}
