using System;
//using System.Linq;
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
//using CodeHatch.Networking.Events.Entities;
//using CodeHatch.Networking.Events.Entities.Players;
using CodeHatch.Networking.Events.Players;
using CodeHatch.ItemContainer;
using CodeHatch.UserInterface.Dialogues;
//using CodeHatch.Inventory.Blueprints.Components;


namespace Oxide.Plugins
{
    [Info("Trade Tracker", "Scorpyon", "1.0.3")]
    public class TradeTracker : ReignOfKingsPlugin
    {
		private const int inflation = 1; // This is the value of inflation (Percentage value 1-100) - as in, the amount prices change per stack.



        private Collection<string[]> tradeDefaults = new Collection<string[]>();
        // 0 - Resource name
        // 1 - Original Price
        // 2 - Max Stack size
        private Collection<string[]> tradeList = new Collection<string[]>();
        // 0 - Resource name
        // 1 - Original Price
        // 2 - Max Stack size
        // 3 - Buy Price
        // 4 - Sell Price
        private Dictionary<string, int> wallet = new Dictionary<string, int>();

		private const int priceModifier = 1000; // Best not to change this unless you have to! I don't know what would happen to prices! 


        void Log(string msg) => Puts($"{Title} : {msg}");
		private const int maxPossibleGold = 21000000; // DO NOT RAISE THIS ANY HIGHER - 32-bit INTEGER FLOOD WARNING	


        // SAVE DATA ===============================================================================================
        private void LoadTradeData()
        {
            tradeDefaults = Interface.GetMod().DataFileSystem.ReadObject<Collection<string[]>>("SavedTradeDefaults");
            tradeList = Interface.GetMod().DataFileSystem.ReadObject<Collection<string[]>>("SavedTradeList");
            wallet = Interface.GetMod().DataFileSystem.ReadObject<Dictionary<string,int>>("SavedTradeWallet");
        }

        private void SaveTradeData()
        {
            Interface.GetMod().DataFileSystem.WriteObject("SavedTradeDefaults", tradeDefaults);
            Interface.GetMod().DataFileSystem.WriteObject("SavedTradeList", tradeList);
            Interface.GetMod().DataFileSystem.WriteObject("SavedTradeWallet", wallet);
        }
		
		private void OnPlayerConnected(Player player)
		{
			CheckWalletExists(player);
			
			// Save the trade data
            SaveTradeData();
		}
		
		private void CheckWalletExists(Player player)
		{
			//Check if the player has a wallet yet
			if(!wallet.ContainsKey(player.Name.ToLower()))
			{
				wallet.Add(player.Name.ToLower(),0);
			}
		}
		
        void Loaded()
        {
            LoadTradeData();
			
			// ================================================================================================================================================================================
			// ================================================================================================================================================================================
			// ================================================================================================================================================================================
			tradeList = new Collection<string[]>();  //                    <<< ======================================================== REMOVE ME BEFORE RELEASING!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			// ================================================================================================================================================================================
			// ================================================================================================================================================================================
			// ================================================================================================================================================================================

			
            //If there's no trade data stored, then set up the new trade data from the defaults
            if(tradeList.Count < 1)
            {
                var defaultList = LoadDefaultTradeValues();
                foreach(var item in defaultList)
                {
                    var newItem = new string[5]{ item[0], item[1], item[2], item[1], item[1] };
                    tradeList.Add(newItem);
                }
            }

            // Save the trade data
            SaveTradeData();

            // Run testing
            RunSanityCheckTests();
        }
        // ===========================================================================================================

        private void RunSanityCheckTests()
        {
            // Test the item store
            //ViewTheExchangeStore(null,null);
        }
		
		private void AdjustMarketPrices(string type, string resource, int amount)
		{
			
		}
		
		// Buying an item from the exchange
        [ChatCommand("admincredits")]
        private void AdminGiveCredits(Player player, string cmd,string[] input)
        {
			if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "This is not for you. Don't even try it, thieving scumbag!");
                return;
            }
			
			var playerName = input.JoinToString(" ");
			PrintToChat(player, "Giving 100000 gold to " + playerName);
			
			var target = Server.GetPlayerByName(playerName);
			
			CheckWalletExists(target);
			GiveGold(target,1000000);
			
			// Save the trade data
            SaveTradeData();
		}

        // Buying an item from the exchange
        [ChatCommand("buy")]
        private void BuyAnItem(Player player, string cmd)
        {
			//Open up the buy screen
			player.ShowInputPopup("Grand Exchange", "What [00FF00]item [FFFFFF]would you like to buy on the [00FFFF]Grand Exchange[FFFFFF]?", "", "Submit", "Cancel", (options, dialogue1, data) => SelectItemToBeBought(player, options, dialogue1, data));
        }
		
		private void GiveGold(Player player,int amount)
		{
			var playerName = player.Name.ToLower();
			var currentGold = wallet[playerName];
			if(currentGold + amount > maxPossibleGold)
			{	
				PrintToChat(player, "[FF0000]Grand Exchange[FFFFFF] : You cannot gain any more gold than you now have. Congratulations. You are the richest player. Goodbye.");
				currentGold = maxPossibleGold;
			}
			else currentGold = currentGold + amount;
			
			wallet.Remove(playerName);
			wallet.Add(playerName,currentGold);
		}
		
		private bool CanRemoveGold(Player player,int amount)
		{
			var playerName = player.Name.ToLower();
			var currentGold = wallet[playerName];
			if(currentGold - amount < 0) return false;
			return true;
		}
		
		private void RemoveGold(Player player,int amount)
		{
			var playerName = player.Name.ToLower();
			var currentGold = wallet[playerName];
			currentGold = currentGold - amount;
			
			wallet.Remove(playerName);
			wallet.Add(playerName,currentGold);
		}
		
		private void SelectItemToBeBought(Player player, Options selection, Dialogue dialogue, object contextData)
		{
			if (selection == Options.Cancel)
            {
                //Leave
                return;
            }
			var requestedResource = dialogue.ValueMessage;
			var resourceFound = false;
			var resourceDetails = new string[5];
			
			// Get the resource's details
			foreach(var item in tradeList)
			{
				if(item[0] == Capitalise(requestedResource))
				{
					resourceDetails = new string[5]{ item[0],item[1],item[2],item[3],item[4] };
					resourceFound = true;
				}
			}
			
			// I couldn't find the resource you wanted!
			if(!resourceFound)
			{
				PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : That item does not appear to currently be for sale!");
				return;
			}
			
			// Open a popup with the resource details
			var message = "Of course!\n[00FF00]" + Capitalise(resourceDetails[0]) + "[FFFFFF] is currently selling for [00FFFF]" + (Int32.Parse(resourceDetails[4])/1000).ToString() + "[FFFF00]g[FFFFFF] per item.\nIt can be bought in stacks of up to [00FF00]" + resourceDetails[2].ToString() + "[FFFFFF].\n How much would you like to buy?";
			
			// Get the player's wallet contents
			CheckWalletExists(player);
			var credits = wallet[player.Name.ToLower()];
			message = message + "\n\n[FF0000]Gold Available[FFFFFF] : [00FF00]" + credits.ToString();
			
			player.ShowInputPopup("Grand Exchange", message, "", "Submit", "Cancel", (options, dialogue1, data) => SelectAmountToBeBought(player, options, dialogue1, data, resourceDetails));
		}
		
		private void SelectAmountToBeBought(Player player, Options selection, Dialogue dialogue, object contextData, string[] resourceDetails)
		{
			if (selection == Options.Cancel)
            {
                //Leave
                return;
            }
			var amountText = dialogue.ValueMessage;

			// Check if the amount is an integer
			int amount;
			if(Int32.TryParse(amountText,out amount) == false)
			{
				PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : That item does not appear to be a valid amount. Please enter a number between 1 and the maximum stack size.");
				return;
			}
			
			//Check if the amount is within the correct limits
			if(amount < 1 || amount > Int32.Parse(resourceDetails[2]))
			{
				PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : You can only purchase an amount between 1 and the maximum stack size.");
				return;
			}
			
			var totalValue = GetPriceForThisPurchase(resourceDetails[0],amount);
			
			var message = "Very good!\n[00FFFF]" + amount.ToString() + " [00FF00]" + Capitalise(resourceDetails[0]) + "[FFFFFF] will cost you a total of \n[FF0000]" + totalValue + " [FFFF00]gold.[FFFFFF]\n Do you want to complete the purchase?";
			
			// Get the player's wallet contents
			CheckWalletExists(player);
			var credits = wallet[player.Name.ToLower()];
			message = message + "\n\n[FF0000]Gold Available[FFFFFF] : [00FF00]" + credits.ToString();
			
			//Show Popup with the final price
			player.ShowConfirmPopup("Grand Exchange", message, "Submit", "Cancel", (options, dialogue1, data) => CheckIfThePlayerCanAffordThis(player, options, dialogue, data, resourceDetails, totalValue, amount));
		}
		
		private void CheckIfThePlayerCanAffordThis(Player player, Options selection, Dialogue dialogue, object contextData, string[] resourceDetails, int totalValue, int amount)
		{
			if (selection != Options.Yes)
            {
                //Leave
                return;
            }
			
			if(!CanRemoveGold(player,Int32.Parse(resourceDetails[3])))
			{
				PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : It looks like you don't have the gold for this transaction, I'm afraid!");
				return;
			}
			
			//Check if there is space in the player's inventory
			var inventory = player.GetInventory().Contents;
			if(inventory.FreeSlotCount < 1)
			{
				PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : You need a free inventory slot to purchase items, I'm afraid. Come back when you have made some space.");
				return;
			}
			
			// Give the item!
			var blueprintForName = InvDefinitions.Instance.Blueprints.GetBlueprintForName(resourceDetails[0], true, true);
            var invGameItemStack = new InvGameItemStack(blueprintForName, amount, null);
            ItemCollection.AutoMergeAdd(inventory, invGameItemStack);
			
			// Take the payment
			RemoveGold(player, totalValue);
			
			// Fix themarket price adjustment
			AdjustMarketPrices("buy", resourceDetails[0] ,amount);
			
			// Tell the player
			PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : Congratulations on your purchase. Please come again!");
			
			//Save the data
			SaveTradeData();
		}
		
		
		private int GetPriceForThisPurchase(string resource, int amount)
		{
			var total = 0;
			foreach(var item in tradeList)
			{
				if(item[0].ToLower() == resource.ToLower())
				{
					total = amount * (Int32.Parse(item[3]) / priceModifier);
				}
			}
			return total;
		}

        // Selling an item on the exchange
        [ChatCommand("sell")]
        private void SellAnItem(Player player, string cmd)
        {
			//Open up the sell screen
			player.ShowInputPopup("Grand Exchange", "What [00FF00]item [FFFFFF]would you like to sell on the [00FFFF]Grand Exchange[FFFFFF]?", "", "Submit", "Cancel", (options, dialogue1, data) => SelectItemToBeSold(player, options, dialogue1, data));
        }

		private void SelectItemToBeSold(Player player, Options selection, Dialogue dialogue, object contextData)
		{
			if (selection == Options.Cancel)
            {
                //Leave
                return;
            }
			var requestedResource = dialogue.ValueMessage;
			var resourceFound = false;
			var resourceDetails = new string[5];
			
			// Get the resource's details
			foreach(var item in tradeList)
			{
				if(item[0] == Capitalise(requestedResource))
				{
					resourceDetails = new string[5]{ item[0],item[1],item[2],item[3],item[4] };
					resourceFound = true;
				}
			}
			
			// I couldn't find the resource you wanted!
			if(!resourceFound)
			{
				PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : That item does not appear to currently be for sale!");
				return;
			}
			
			// Open a popup with the resource details
			var message = "Of course!\n[00FF00]" + Capitalise(resourceDetails[0]) + "[FFFFFF] is currently selling for [00FFFF]" + (Int32.Parse(resourceDetails[4])/1000).ToString() + "[FFFF00]g[FFFFFF] per item.\nIt can be bought in stacks of up to [00FF00]" + resourceDetails[2].ToString() + "[FFFFFF].\n How much would you like to buy?";
			
			// Get the player's wallet contents
			CheckWalletExists(player);
			var credits = wallet[player.Name.ToLower()];
			message = message + "\n\n[FF0000]Gold Available[FFFFFF] : [00FF00]" + credits.ToString();
			
			player.ShowInputPopup("Grand Exchange", message, "", "Submit", "Cancel", (options, dialogue1, data) => SelectAmountToBeBought(player, options, dialogue1, data, resourceDetails));
		}
        // View the prices of items on the exchange
        [ChatCommand("store")]
        private void ViewTheExchangeStore(Player player, string cmd)
        {
            // Are there any items on the store?
            if(tradeList.Count == 0)
            {
                Log("There appear to be no items in the store!");
                return;
            }
            Log("Trade: Prices have been found!");

			// Get the player's wallet contents
			CheckWalletExists(player);
			var credits = wallet[player.Name.ToLower()];
			
            // Check if player exists (For Unit Testing)
            var buyIcon = "[008888]";
            var sellIcon = "[008888]";
            var itemText = "";
            var itemsPerPage = 25;
            for(var i = 0; i<itemsPerPage;i++)
            {
                var resource = tradeList[i][0];
                var originalPrice = Int32.Parse(tradeList[i][1]) / priceModifier;
                var stackLimit = Int32.Parse(tradeList[i][2]);
                var buyPrice = Int32.Parse(tradeList[i][3]) / priceModifier;
                var sellPrice = Int32.Parse(tradeList[i][4]) / priceModifier;
				//PrintToChat(tradeList[i][0] + ", " + tradeList[i][1] + ", " + tradeList[i][2] + ", " + tradeList[i][3] + ", " + tradeList[i][4]);

                var buyDiff = originalPrice + (buyPrice - originalPrice);
                var buyPriceText = buyDiff.ToString();
                if(buyDiff == 0) buyPriceText = "";
                var sellDiff = sellPrice -  + (sellPrice - originalPrice);
                var sellPriceText = sellDiff.ToString();
                if(sellDiff == 0) sellPriceText = "";

                //Has the price modulated?
                if(buyDiff > originalPrice) buyIcon = "[FF0000]";
                if(buyDiff < originalPrice) buyIcon = "[00FF00]";
                if(sellDiff > originalPrice) sellIcon = "[FF0000]";
                if(sellDiff < originalPrice) sellIcon = "[00FF00]";


                itemText = itemText + "[888800]" + Capitalise(resource) + "[FFFFFF]; Buy: " + buyIcon + buyPriceText + "[FFFF00]g  [FFFFFF]Sell: " + sellIcon + sellPriceText + "[FFFF00]g\n";
            }
			
			itemText = itemText + "\n\n[FF0000]Gold Available[FFFFFF] : [00FF00]" + credits.ToString();
			
            //Display the Popup with the price
            //player.ShowPopup("Trade Prices", itemText,"Ok",  (selection, dialogue, data) => DoNothing(player, selection, dialogue, data));
            player.ShowConfirmPopup("Grand Exchange", itemText, "Next Page", "Exit", (selection, dialogue, data) => ContinueWithTradeList(player, selection, dialogue, data, itemsPerPage, itemsPerPage));
        }

        
		private void ContinueWithTradeList(Player player, Options selection, Dialogue dialogue, object contextData,int itemsPerPage, int currentItemCount)
		{
            if (selection != Options.Yes)
            {
                //Leave
                return;
            }
			
			if((currentItemCount + itemsPerPage) > tradeList.Count)
			{
				itemsPerPage = tradeList.Count - currentItemCount;
			}
            
			// Get the player's wallet contents
			CheckWalletExists(player);
			var credits = wallet[player.Name.ToLower()];
			
            var buyIcon = "[008888]";
            var sellIcon = "[008888]";
            var itemText = "";

			for(var i = currentItemCount; i<itemsPerPage + currentItemCount; i++)
            {
                var resource = tradeList[i][0];
                var originalPrice = Int32.Parse(tradeList[i][1]) / priceModifier;
                var stackLimit = Int32.Parse(tradeList[i][2]) / priceModifier;
                var buyPrice = Int32.Parse(tradeList[i][3]) / priceModifier;
                var sellPrice = Int32.Parse(tradeList[i][4]) / priceModifier;

                var buyDiff = originalPrice + (buyPrice - originalPrice);
                var buyPriceText = buyDiff.ToString();
                if(buyDiff == 0) buyPriceText = "";
                var sellDiff = sellPrice -  + (sellPrice - originalPrice);
                var sellPriceText = sellDiff.ToString();
                if(sellDiff == 0) sellPriceText = "";

                //Has the price modulated?
                if(buyDiff > originalPrice) buyIcon = "[FF0000]";
                if(buyDiff < originalPrice) buyIcon = "[00FF00]";
                if(sellDiff > originalPrice) sellIcon = "[FF0000]";
                if(sellDiff < originalPrice) sellIcon = "[00FF00]";

                itemText = itemText + "[888800]" + Capitalise(resource) + "[FFFFFF]; Buy: " + buyIcon + buyPriceText + "[FFFF00]g  [FFFFFF]Sell: " + sellIcon + sellPriceText + "[FFFF00]g\n";
            }
			
			itemText = itemText + "\n\n[FF0000]Gold Available[FFFFFF] : [00FF00]" + credits.ToString();

            currentItemCount = currentItemCount + itemsPerPage;

            // Display the Next page
            if(currentItemCount < tradeList.Count)
            {
                player.ShowConfirmPopup("Grand Exchange", itemText,  "Next Page", "Exit", (options, dialogue1, data) => ContinueWithTradeList(player, options, dialogue1, data, itemsPerPage, currentItemCount));
            }
            else
            {
                PlayerExtensions.ShowPopup(player,"Grand Exchange", itemText, "Yes",  (newselection, dialogue2, data) => DoNothing(player, newselection, dialogue2, data));
            }
		}

		private void DoNothing(Player player, Options selection, Dialogue dialogue, object contextData)
		{
			//Do nothing
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

        private Collection<string[]> LoadDefaultTradeValues()
        {
            var defaultTradeList = new Collection<string[]>();

            defaultTradeList.Add(new string[3] { "Apple", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Baked Clay", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Ballista", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Ballista Bolt", "50000", "100" });
            defaultTradeList.Add(new string[3] { "Bandage", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Bat Wing", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Bear Hide", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Bent Horn", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Berries", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Blood", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Bone", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Bone Axe", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Bone Dagger", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Bone Horn", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Bone Spiked Club", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Bread", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Brown Beans", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Cabbage", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Candlestand", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Carrot", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Chandelier", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Charcoal", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Chicken", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Clay", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Clay Block", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Clay Ramp", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Clay Stairs", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Cobblestone Block", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Cobblestone Ramp", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Cobblestone Stairs", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Cooked Bird", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Cooked Meat", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Crossbow", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Deer Leg Club", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Diamond", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Dirt", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Driftwood Club", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Duck Feet", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Fang", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Fat", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Feather", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Fire Water", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Firepit", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Great FirePlace", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone FirePlace", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Flax", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Flowers", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Fluffy Bed", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Fuse", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Grain", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Granary", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Guillotine", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Ground Torch", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Hanging Lantern", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Hanging Torch", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Hay", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Hay Bale Target", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Heart", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Holdable Candle", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Holdable Lantern", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Holdable Torch", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Iron Axe", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Bar Window", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Battle Axe", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Chest", "50000", "10" });
            defaultTradeList.Add(new string[3] { "Iron Crest", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Door", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Flanged Mace", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Floor Torch", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Gate", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Halberd", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Hatchet", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Ingot", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Iron Javelin", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Pickaxe", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Boots", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Gauntlets", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Helmet", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Pants", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Vest", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Shackles", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Star Mace", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Sword", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Tipped Arrow", "50000", "100" });
            defaultTradeList.Add(new string[3] { "Iron Wood Cutters Axe", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Large Gallows", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Large Iron Cage", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Large Iron Hanging Cage", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Leather Crest", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Leather Hide", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Light Leather Boots", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Light Leather Bracers", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Light Leather Helmet", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Light Leather Pants", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Light Leather Vest", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Liver", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Lockpick", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Log Block", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Log Ramp", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Log Stairs", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Long Horn", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Meat", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Medium Banner", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Oil", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Pillory", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Potion of Antidote", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Potion of Appearance", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Rabbit Pelt", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Raw Bird", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood Block", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood Door", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood Gate", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood Ramp", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood Stairs", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood Door", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Repair Hammer", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Rope", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Roses", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Sharp Rock", "50000", "100" });
            defaultTradeList.Add(new string[3] { "Small Banner", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Small Gallows", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Small Iron Cage", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Small Iron Hanging Cage", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Small Wall Lantern", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Small Wall Torch", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Sod Block", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Sod Ramp", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Sod Stairs", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Splintered Club", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Spruce Branches Block", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Spruce Branches Ramp", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Spruce Branches Stairs", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Standing Iron Torch", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Axe", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Battle Axe", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Bolt", "50000", "100" });
            defaultTradeList.Add(new string[3] { "Steel Cage", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Chest", "50000", "10" });
            defaultTradeList.Add(new string[3] { "Steel Compound", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Steel Crest", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Dagger", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Flanged Mace", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Great Sword", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Halberd", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Hatchet", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Ingot", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Steel Javelin", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Pickaxe", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Boots", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Gauntlets", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Helmet", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Pants", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Vest", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Star Mace", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Sword", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Tipped Arrow", "50000", "100" });
            defaultTradeList.Add(new string[3] { "Steel War Hammer", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Wood Cutters Axe", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Sticks", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Stiff Bed", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Arch", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Arrow", "50000", "100" });
            defaultTradeList.Add(new string[3] { "Stone Block", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Cutter", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Dagger", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Hatchet", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Javelin", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Pickaxe", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Ramp", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Slab", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Slit Window", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Stairs", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Sword", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Wood Cutters Axe", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Tears Of The Gods", "50000", "10" });
            defaultTradeList.Add(new string[3] { "Thatch Block", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Thatch Ramp", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Thatch Stairs", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Throwing Stone", "50000", "100" });
            defaultTradeList.Add(new string[3] { "Tinker", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Trebuchet", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Trebuchet Stone", "50000", "100" });
            defaultTradeList.Add(new string[3] { "Wall Lantern", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wall Torch", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Water", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Whip", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wood", "10000", "1000" });
            defaultTradeList.Add(new string[3] { "Wolf Pelt", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Arrow", "50000", "100" });
            defaultTradeList.Add(new string[3] { "Wood Block", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Wood Bracers", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Chest", "50000", "10" });
            defaultTradeList.Add(new string[3] { "Wood Door", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Gate", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Helmet", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Ramp", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Wood Sandals", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Shutters", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Skirt", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Stairs", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Wood Vest", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Cage", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Flute", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Javelin", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Mace", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Short Bow", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wool", "50000", "1000" });

            return defaultTradeList;
        }

	}
}
