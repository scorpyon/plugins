using System;
using System.Linq;
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
using CodeHatch.Networking.Events;
using CodeHatch.Networking.Events.Entities;
using CodeHatch.Networking.Events.Entities.Players;
using CodeHatch.Networking.Events.Players;
using CodeHatch.ItemContainer;
using CodeHatch.UserInterface.Dialogues;
using CodeHatch.Inventory.Blueprints.Components;


namespace Oxide.Plugins
{
    [Info("InventoryProtection", "Scorpyon", "1.0.1")]
    public class InventoryProtection : ReignOfKingsPlugin
    {
		private Dictionary<Player,Collection<string[]>> inventorySaveList = new Dictionary<Player,Collection<string[]>>();
		void Log(string msg) => Puts($"{Title} : {msg}");

		
        // SAVE DATA ===============================================================================================

        private void LoadInventoryData()
        {
            inventorySaveList = Interface.GetMod().DataFileSystem.ReadObject<Dictionary<Player,Collection<string[]>>>("InventoryInformationSave");
        }

        private void SaveInventoryData()
        {
            Log("Saving data for Player Inventory");
            Interface.GetMod().DataFileSystem.WriteObject("InventoryInformationSave", inventorySaveList);
        }

        void Loaded()
        {
            inventorySaveList = new Dictionary<Player,Collection<string[]>>();
            LoadInventoryData();
        }


        // ===========================================================================================================

		
		// private void OnPlayerConnected(Player player)
        // {
			// var inventoryIsWrong = false;
			
			// //Check if the player has an inventory saved
			// if(inventorySaveList.ContainsKey(player))
			// {
				// //The player's current inventory contents
				// var currentContents = GetInventoryContents(player);
				
				// //The saved contents
				// var savedContents = inventorySaveList[player];
				
				// for(var i=0; i<currentContents.Count;i++)
				// {
					// // If the resource is wrong
					// if(currentContents[i][0] != savedContents[i][0] || currentContents[i][1] != savedContents[i][1])
					// {
						// inventoryIsWrong = true;
						// break;
					// }
				// }
				// if(inventoryIsWrong)
				// {
					// OverwritePlayerInventory(player, savedContents);
				// }
				// Log("Loading Player Inventory Data for " + player.DisplayName);
			// }
			// else Log("No inventory save found for " + player.DisplayName);
        // }
		
		private void OverwritePlayerInventory(Player player, Collection<string[]> savedContents)
		{
			EmptyPlayerInventory(player);
			GetItemsFromSavedInventory(player,savedContents);
		}
		
		private void GetItemsFromSavedInventory(Player player, Collection<string[]> savedContents)
		{
			var inventory = player.GetInventory();
			foreach(var savedItem in savedContents)
			{
				var blueprintForName = InvDefinitions.Instance.Blueprints.GetBlueprintForName(savedItem[0], true, true);
				var invGameItemStack = new InvGameItemStack(blueprintForName, Int32.Parse(savedItem[1]), null);
				ItemCollection.AutoMergeAdd(inventory.Contents, invGameItemStack);
			}
		}

		private void EmptyPlayerInventory(Player player)
		{
			var inventory = player.GetInventory().Contents;
			
			foreach (InvGameItemStack item in inventory.Where(item => item != null))
            {
				inventory.SplitItem(item, item.StackAmount, true);
            }
		}
		
		// private void OnPlayerDisconnected(Player player)
        // {
			// Log("Saving Player Inventory Data for " + player.DisplayName);
			// if(player != null) StoreThePlayerInventory(player);
        // }
		
		private void StoreThePlayerInventory(Player player)
		{
			//Get the inventory contents
			var inventory = GetInventoryContents(player);
			if(inventory == null) return;

			// Check if the store exists
			if(inventorySaveList !=null)
			{
				//See if the player has an inventory stored here
				if(inventorySaveList.ContainsKey(player))
				{
					//Remove the old record
					inventorySaveList.Remove(player);
				}
				
				//Add the player's inventory record
				inventorySaveList.Add(player,inventory);
			}
			SaveInventoryData();
		}
		
		private Collection<string[]> GetInventoryContents(Player player)
		{
			var inventory = player.GetInventory().Contents;
			var inventoryContents = new Collection<string[]>();
			foreach (InvGameItemStack item in inventory.Where(item => item != null))
            {
				string[] tempStack = new string[]{ item.Name, item.StackAmount.ToString() };
				inventoryContents.Add(tempStack);
            }
			return inventoryContents;
		}
		
	}
}