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
//using CodeHatch.ItemContainer;
using CodeHatch.UserInterface.Dialogues;
//using CodeHatch.Inventory.Blueprints.Components;


namespace Oxide.Plugins
{
    [Info("Trade Tracker", "Scorpyon", "1.0.3")]
    public class TradeTracker : ReignOfKingsPlugin
    {
        private Collection<string[]> tradeDefaults = new Collection<string[]>();
        // 0 - Resource name
        // 1 - Original Price
        // 2 - Max Stack size
        private Collection<string[]> tradeList = new Collection<string[]>();
        // 0 - Resource name
        // 1 - Original Price
        // 2 - Max Stack size
        // 3 - Price
        private Dictionary<string, int> wallet = new Dictionary<string, int>();

        void Log(string msg) => Puts($"{Title} : {msg}");


        // SAVE DATA ===============================================================================================
        private void LoadTradeData()
        {
            tradeDefaults = Interface.GetMod().DataFileSystem.ReadObject<Collection<string[]>>("SavedTradeDefaults");
            tradeList = Interface.GetMod().DataFileSystem.ReadObject<Collection<string[]>>("SavedTradeList");
        }

        private void SaveTradeData()
        {
            Interface.GetMod().DataFileSystem.WriteObject("SavedTradeDefaults", tradeDefaults);
            Interface.GetMod().DataFileSystem.WriteObject("SavedTradeList", tradeList);
        }

        void Loaded()
        {
            LoadTradeData();

            //If there's no trade data stored, then set up the new trade data from the defaults
            if(tradeList.Count < 1)
            {
                var defaultList = LoadDefaultTradeValues();
                foreach(var item in defaultList)
                {
                    var newItem = new string[4]{ item[0], item[1], item[2], item[1]};
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

        // Buying an item from the exchange
        [ChatCommand("buy")]
        private void BuyAnItem(Player player, string cmd)
        {

        }

        // Selling an item on the exchange
        [ChatCommand("sell")]
        private void SellAnItem(Player player, string cmd)
        {

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

            // Check if player exists (For Unit Testing)
            var adjustorIcon = "";
            var itemText = "";
            var itemsPerPage = 30;
            for(var i = 0; i<itemsPerPage;i++)
            {
                var resource = tradeList[i][0];
                var originalPrice = Int32.Parse(tradeList[i][1]);
                var stackLimit = Int32.Parse(tradeList[i][2]);
                var currentPrice = Int32.Parse(tradeList[i][3]);

                var adjustedPrice = currentPrice - originalPrice;
                var adjustedPriceText = adjustedPrice.ToString();
                if(adjustedPrice == 0) adjustedPriceText = "";

                //Has the price modulated?
                if(currentPrice > originalPrice) adjustorIcon = "[FF0000]+";
                if(currentPrice < originalPrice) adjustorIcon = "[00FF00]";


                itemText = itemText + "[00FF00]" + Capitalise(resource) + "[FFFFFF] : " + "[FFFF00]$ " + currentPrice + " [FFFFFF]" + adjustorIcon + adjustedPriceText + "\n";
            }

            //Display the Popup with the price
            //player.ShowPopup("Trade Prices", itemText,"Ok",  (selection, dialogue, data) => DoNothing(player, selection, dialogue, data));
            player.ShowConfirmPopup("Trade Prices", itemText, "Next Page", "Exit", (selection, dialogue, data) => ContinueWithTradeList(player, selection, dialogue, data, itemsPerPage, itemsPerPage));
        }

        
		private void ContinueWithTradeList(Player player, Options selection, Dialogue dialogue, object contextData,int itemsPerPage, int currentItemCount)
		{
            if (selection != Options.Yes)
            {
                //Leave
                return;
            }
            
            var adjustorIcon = "";
            var itemText = "";

			for(var i = currentItemCount; i<itemsPerPage + currentItemCount; i++)
            {
                var resource = tradeList[i][0];
                var originalPrice = Int32.Parse(tradeList[i][1]);
                var stackLimit = Int32.Parse(tradeList[i][2]);
                var currentPrice = Int32.Parse(tradeList[i][3]);

                var adjustedPrice = currentPrice - originalPrice;
                var adjustedPriceText = adjustedPrice.ToString();
                if(adjustedPrice == 0) adjustedPriceText = "";

                //Has the price modulated?
                if(currentPrice > originalPrice) adjustorIcon = "[FF0000]+";
                if(currentPrice < originalPrice) adjustorIcon = "[00FF00]";


                itemText = itemText + "[00FF00]" + Capitalise(resource) + "[FFFFFF] : " + "[FFFF00]$ " + currentPrice + " [FFFFFF]" + adjustorIcon + adjustedPriceText + "\n";

            }

            currentItemCount = currentItemCount + itemsPerPage;

            // Display the Next page
            if((currentItemCount + itemsPerPage) <= tradeList.Count)
            {
                player.ShowConfirmPopup("Trade Prices", itemText,  "Next Page", "Exit", (options, dialogue1, data) => ContinueWithTradeList(player, options, dialogue1, data, itemsPerPage, currentItemCount));
            }
            else
            {
                PlayerExtensions.ShowPopup(player,"Trade Prices", itemText, "Yes",  (newselection, dialogue2, data) => DoNothing(player, newselection, dialogue2, data));
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

            defaultTradeList.Add(new string[3] { "Apple", "50", "25" });
            defaultTradeList.Add(new string[3] { "Baked Clay", "30", "1000" });
            defaultTradeList.Add(new string[3] { "Ballista", "50", "1" });
            defaultTradeList.Add(new string[3] { "Ballista Bolt", "50", "100" });
            defaultTradeList.Add(new string[3] { "Bandage", "50", "25" });
            defaultTradeList.Add(new string[3] { "Bat Wing", "50", "25" });
            defaultTradeList.Add(new string[3] { "Bear Hide", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Bent Horn", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Berries", "50", "25" });
            defaultTradeList.Add(new string[3] { "Blood", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Bone", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Bone Axe", "50", "1" });
            defaultTradeList.Add(new string[3] { "Bone Dagger", "50", "1" });
            defaultTradeList.Add(new string[3] { "Bone Horn", "50", "1" });
            defaultTradeList.Add(new string[3] { "Bone Spiked Club", "50", "1" });
            defaultTradeList.Add(new string[3] { "Bread", "50", "25" });
            defaultTradeList.Add(new string[3] { "Brown Beans", "50", "25" });
            defaultTradeList.Add(new string[3] { "Cabbage", "50", "25" });
            defaultTradeList.Add(new string[3] { "Candlestand", "50", "1" });
            defaultTradeList.Add(new string[3] { "Carrot", "50", "25" });
            defaultTradeList.Add(new string[3] { "Chandelier", "50", "1" });
            defaultTradeList.Add(new string[3] { "Charcoal", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Chicken", "50", "25" });
            defaultTradeList.Add(new string[3] { "Clay", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Clay Block", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Clay Ramp", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Clay Stairs", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Cobblestone Block", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Cobblestone Ramp", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Cobblestone Stairs", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Cooked Bird", "50", "25" });
            defaultTradeList.Add(new string[3] { "Cooked Meat", "50", "25" });
            defaultTradeList.Add(new string[3] { "Crossbow", "50", "1" });
            defaultTradeList.Add(new string[3] { "Deer Leg Club", "50", "1" });
            defaultTradeList.Add(new string[3] { "Diamond", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Dirt", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Driftwood Club", "50", "1" });
            defaultTradeList.Add(new string[3] { "Duck Feet", "50", "25" });
            defaultTradeList.Add(new string[3] { "Fang", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Fat", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Feather", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Fire Water", "50", "25" });
            defaultTradeList.Add(new string[3] { "Firepit", "50", "1" });
            defaultTradeList.Add(new string[3] { "Great FirePlace", "50", "1" });
            defaultTradeList.Add(new string[3] { "Stone FirePlace", "50", "1" });
            defaultTradeList.Add(new string[3] { "Flax", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Flowers", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Fluffy Bed", "50", "1" });
            defaultTradeList.Add(new string[3] { "Fuse", "50", "1" });
            defaultTradeList.Add(new string[3] { "Grain", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Granary", "50", "1" });
            defaultTradeList.Add(new string[3] { "Guillotine", "50", "1" });
            defaultTradeList.Add(new string[3] { "Ground Torch", "50", "1" });
            defaultTradeList.Add(new string[3] { "Hanging Lantern", "50", "1" });
            defaultTradeList.Add(new string[3] { "Hanging Torch", "50", "1" });
            defaultTradeList.Add(new string[3] { "Hay", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Hay Bale Target", "50", "1" });
            defaultTradeList.Add(new string[3] { "Heart", "50", "25" });
            defaultTradeList.Add(new string[3] { "Holdable Candle", "50", "1" });
            defaultTradeList.Add(new string[3] { "Holdable Lantern", "50", "1" });
            defaultTradeList.Add(new string[3] { "Holdable Torch", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Iron Axe", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Bar Window", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Battle Axe", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Chest", "50", "10" });
            defaultTradeList.Add(new string[3] { "Iron Crest", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Door", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Flanged Mace", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Floor Torch", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Gate", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Halberd", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Hatchet", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Ingot", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Iron Javelin", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Pickaxe", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Boots", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Gauntlets", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Helmet", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Pants", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Vest", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Shackles", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Star Mace", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Sword", "50", "1" });
            defaultTradeList.Add(new string[3] { "Iron Tipped Arrow", "50", "100" });
            defaultTradeList.Add(new string[3] { "Iron Wood Cutters Axe", "50", "1" });
            defaultTradeList.Add(new string[3] { "Large Gallows", "50", "1" });
            defaultTradeList.Add(new string[3] { "Large Iron Cage", "50", "1" });
            defaultTradeList.Add(new string[3] { "Large Iron Hanging Cage", "50", "1" });
            defaultTradeList.Add(new string[3] { "Leather Crest", "50", "1" });
            defaultTradeList.Add(new string[3] { "Leather Hide", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Light Leather Boots", "50", "1" });
            defaultTradeList.Add(new string[3] { "Light Leather Bracers", "50", "1" });
            defaultTradeList.Add(new string[3] { "Light Leather Helmet", "50", "1" });
            defaultTradeList.Add(new string[3] { "Light Leather Pants", "50", "1" });
            defaultTradeList.Add(new string[3] { "Light Leather Vest", "50", "1" });
            defaultTradeList.Add(new string[3] { "Liver", "50", "25" });
            defaultTradeList.Add(new string[3] { "Lockpick", "50", "25" });
            defaultTradeList.Add(new string[3] { "Log Block", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Log Ramp", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Log Stairs", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Long Horn", "50", "1" });
            defaultTradeList.Add(new string[3] { "Meat", "50", "25" });
            defaultTradeList.Add(new string[3] { "Medium Banner", "50", "1" });
            defaultTradeList.Add(new string[3] { "Oil", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Pillory", "50", "1" });
            defaultTradeList.Add(new string[3] { "Potion of Antidote", "50", "25" });
            defaultTradeList.Add(new string[3] { "Potion of Appearance", "50", "25" });
            defaultTradeList.Add(new string[3] { "Rabbit Pelt", "50", "25" });
            defaultTradeList.Add(new string[3] { "Raw Bird", "50", "25" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood Block", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood Door", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood Gate", "50", "1" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood Ramp", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood Stairs", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood Door", "50", "1" });
            defaultTradeList.Add(new string[3] { "Repair Hammer", "50", "1" });
            defaultTradeList.Add(new string[3] { "Rope", "50", "1" });
            defaultTradeList.Add(new string[3] { "Roses", "50", "25" });
            defaultTradeList.Add(new string[3] { "Sharp Rock", "50", "100" });
            defaultTradeList.Add(new string[3] { "Small Banner", "50", "1" });
            defaultTradeList.Add(new string[3] { "Small Gallows", "50", "1" });
            defaultTradeList.Add(new string[3] { "Small Iron Cage", "50", "1" });
            defaultTradeList.Add(new string[3] { "Small Iron Hanging Cage", "50", "1" });
            defaultTradeList.Add(new string[3] { "Small Wall Lantern", "50", "1" });
            defaultTradeList.Add(new string[3] { "Small Wall Torch", "50", "1" });
            defaultTradeList.Add(new string[3] { "Sod Block", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Sod Ramp", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Sod Stairs", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Splintered Club", "50", "1" });
            defaultTradeList.Add(new string[3] { "Spruce Branches Block", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Spruce Branches Ramp", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Spruce Branches Stairs", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Standing Iron Torch", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Axe", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Battle Axe", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Bolt", "50", "100" });
            defaultTradeList.Add(new string[3] { "Steel Cage", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Chest", "50", "10" });
            defaultTradeList.Add(new string[3] { "Steel Compound", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Steel Crest", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Dagger", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Flanged Mace", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Great Sword", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Halberd", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Hatchet", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Ingot", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Steel Javelin", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Pickaxe", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Boots", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Gauntlets", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Helmet", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Pants", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Vest", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Star Mace", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Sword", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Tipped Arrow", "50", "100" });
            defaultTradeList.Add(new string[3] { "Steel War Hammer", "50", "1" });
            defaultTradeList.Add(new string[3] { "Steel Wood Cutters Axe", "50", "1" });
            defaultTradeList.Add(new string[3] { "Sticks", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Stiff Bed", "50", "1" });
            defaultTradeList.Add(new string[3] { "Stone", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Arch", "50", "1" });
            defaultTradeList.Add(new string[3] { "Stone Arrow", "50", "100" });
            defaultTradeList.Add(new string[3] { "Stone Block", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Cutter", "50", "1" });
            defaultTradeList.Add(new string[3] { "Stone Dagger", "50", "1" });
            defaultTradeList.Add(new string[3] { "Stone Hatchet", "50", "1" });
            defaultTradeList.Add(new string[3] { "Stone Javelin", "50", "1" });
            defaultTradeList.Add(new string[3] { "Stone Pickaxe", "50", "1" });
            defaultTradeList.Add(new string[3] { "Stone Ramp", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Slab", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Slit Window", "50", "1" });
            defaultTradeList.Add(new string[3] { "Stone Stairs", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Sword", "50", "1" });
            defaultTradeList.Add(new string[3] { "Stone Wood Cutters Axe", "50", "1" });
            defaultTradeList.Add(new string[3] { "Tears Of The Gods", "50", "10" });
            defaultTradeList.Add(new string[3] { "Thatch Block", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Thatch Ramp", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Thatch Stairs", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Throwing Stone", "50", "100" });
            defaultTradeList.Add(new string[3] { "Tinker", "50", "1" });
            defaultTradeList.Add(new string[3] { "Trebuchet", "50", "1" });
            defaultTradeList.Add(new string[3] { "Trebuchet Stone", "50", "100" });
            defaultTradeList.Add(new string[3] { "Wall Lantern", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wall Torch", "50", "1" });
            defaultTradeList.Add(new string[3] { "Water", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Whip", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wood", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Wolf Pelt", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wood Arrow", "50", "100" });
            defaultTradeList.Add(new string[3] { "Wood Block", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Wood Bracers", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wood Chest", "50", "10" });
            defaultTradeList.Add(new string[3] { "Wood Door", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wood Gate", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wood Helmet", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wood Ramp", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Wood Sandals", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wood Shutters", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wood Skirt", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wood Stairs", "50", "1000" });
            defaultTradeList.Add(new string[3] { "Wood Vest", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Cage", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Flute", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Javelin", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Mace", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Short Bow", "50", "1" });
            defaultTradeList.Add(new string[3] { "Wool", "50", "1000" });

            return defaultTradeList;
        }

	}
}
