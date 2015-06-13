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
using CodeHatch.ItemContainer;
using CodeHatch.UserInterface.Dialogues;
//using CodeHatch.Inventory.Blueprints.Components;


namespace Oxide.Plugins
{
    [Info("Trade Tracker", "Scorpyon", "1.0.9")]
    public class TradeTracker : ReignOfKingsPlugin
    {
		private const double inflation = 1; // This is the inflation modifier. More means bigger jumps in price changes (Currently raises at approx 1%
		private const double maxDeflation = 0; // This is the deflation modifier. This is the most that a price can drop below its average price to buy and above it's price to sell(Percentage)
		private const int priceDeflationTime = 3600; // This dictates the number of seconds for each tick which brings the prices back towards their original values
		private const int goldRewardForPvp = 10000; // This is the maximum amount of gold that can be stolen from a player for killing them.
		private const int goldRewardForPve = 100; // This is the maximum amount rewarded to a player for killing monsters, etc. (When harvesting the dead body)
		private bool allowPvpGold = true; // Turns on/off gold for PVP
		private bool allowPveGold = true; // Turns on/off gold for PVE
		

        private Collection<string[]> LoadDefaultTradeValues()
        {
            var defaultTradeList = new Collection<string[]>();
			
			// Default list prices ::: "Resource Name" ; "Price (x priceModifier)" ; "Maximum stack size"
			// YOU CAN EDIT THESE PRICES, BUT TO SEE THEM IN GAME YOU WILL EITHER NEED TO USE /restoredefaultprices WHICH WILL RESET ALL PRICES TO THE ONES HERE, OR USE /removestoreitem TO REMOVE THE OLD VERSION FROM THE EXISTING TRADE LIST AND THEN /addstoreitem TO INCLUDE THE NEW ONE. MAKE SURE YOU PAY ATTENTION TO THE ALPHABETICAL ORDER HERE, TOO!
			
            defaultTradeList.Add(new string[3] { "Apple", "25000", "25" });
            defaultTradeList.Add(new string[3] { "Baked Clay", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Ballista", "10000000", "1" });
            defaultTradeList.Add(new string[3] { "Ballista Bolt", "500000", "100" });
            defaultTradeList.Add(new string[3] { "Bandage", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Bat Wing", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Bear Hide", "250000", "1000" });
            defaultTradeList.Add(new string[3] { "Bent Horn", "2000000", "1000" });
            defaultTradeList.Add(new string[3] { "Berries", "20000", "25" });
            defaultTradeList.Add(new string[3] { "Blood", "200000", "1000" });
            defaultTradeList.Add(new string[3] { "Bone", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Bone Axe", "1000000", "1" });
            defaultTradeList.Add(new string[3] { "Bone Dagger", "1000000", "1" });
            defaultTradeList.Add(new string[3] { "Bone Horn", "1500000", "1" });
            defaultTradeList.Add(new string[3] { "Bone Spiked Club", "1000000", "1" });
            defaultTradeList.Add(new string[3] { "Bread", "250000", "25" });
            defaultTradeList.Add(new string[3] { "Brown Beans", "250000", "25" });
            defaultTradeList.Add(new string[3] { "Cabbage", "250000", "25" });
            defaultTradeList.Add(new string[3] { "Candlestand", "1000000", "1" });
            defaultTradeList.Add(new string[3] { "Carrot", "200000", "25" });
            defaultTradeList.Add(new string[3] { "Chandelier", "2000000", "1" });
            defaultTradeList.Add(new string[3] { "Charcoal", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Chicken", "250000", "25" });
            defaultTradeList.Add(new string[3] { "Clay", "500000", "1000" });
            defaultTradeList.Add(new string[3] { "Clay Block", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Clay Ramp", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Clay Stairs", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Cobblestone Block", "500000", "1000" });
            defaultTradeList.Add(new string[3] { "Cobblestone Ramp", "500000", "1000" });
            defaultTradeList.Add(new string[3] { "Cobblestone Stairs", "500000", "1000" });
            defaultTradeList.Add(new string[3] { "Cooked Bird", "300000", "25" });
            defaultTradeList.Add(new string[3] { "Cooked Meat", "350000", "25" });
            defaultTradeList.Add(new string[3] { "Crossbow", "5000000", "1" });
            defaultTradeList.Add(new string[3] { "Deer Leg Club", "250000", "1" });
            defaultTradeList.Add(new string[3] { "Diamond", "50000", "500000" });
            defaultTradeList.Add(new string[3] { "Dirt", "50000", "20000" });
            defaultTradeList.Add(new string[3] { "Driftwood Club", "10000", "1" });
            defaultTradeList.Add(new string[3] { "Duck Feet", "200000", "25" });
            defaultTradeList.Add(new string[3] { "Fang", "20000", "1000" });
            defaultTradeList.Add(new string[3] { "Fat", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Feather", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Fire Water", "5000000", "25" });
            defaultTradeList.Add(new string[3] { "Firepit", "1000000", "1" });
            defaultTradeList.Add(new string[3] { "Great FirePlace", "10000000", "1" });
            defaultTradeList.Add(new string[3] { "Stone FirePlace", "7500000", "1" });
            defaultTradeList.Add(new string[3] { "Flax", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Flowers", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Fluffy Bed", "1000000", "1" });
            defaultTradeList.Add(new string[3] { "Fuse", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Grain", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Granary", "1000000", "1" });
            defaultTradeList.Add(new string[3] { "Guillotine", "2000000", "1" });
            defaultTradeList.Add(new string[3] { "Ground Torch", "1500000", "1" });
            defaultTradeList.Add(new string[3] { "Hanging Lantern", "2500000", "1" });
            defaultTradeList.Add(new string[3] { "Hanging Torch", "1500000", "1" });
            defaultTradeList.Add(new string[3] { "Hay", "30000", "1000" });
            defaultTradeList.Add(new string[3] { "Hay Bale Target", "1000000", "1" });
            defaultTradeList.Add(new string[3] { "Heart", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Holdable Candle", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Holdable Lantern", "800000", "1" });
            defaultTradeList.Add(new string[3] { "Holdable Torch", "700000", "1" });
            defaultTradeList.Add(new string[3] { "Iron", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Iron Axe", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Bar Window", "1500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Battle Axe", "1500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Chest", "2000000", "10" });
            defaultTradeList.Add(new string[3] { "Iron Crest", "4000000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Door", "2500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Flanged Mace", "2000000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Floor Torch", "1500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Gate", "5000000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Halberd", "2000000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Hatchet", "1750000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Ingot", "250000", "1000" });
            defaultTradeList.Add(new string[3] { "Iron Javelin", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Pickaxe", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Boots", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Gauntlets", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Helmet", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Pants", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Plate Vest", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Shackles", "600000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Star Mace", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Sword", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Iron Tipped Arrow", "50000", "100" });
            defaultTradeList.Add(new string[3] { "Iron Wood Cutters Axe", "400000", "1" });
            defaultTradeList.Add(new string[3] { "Large Gallows", "3000000", "1" });
            defaultTradeList.Add(new string[3] { "Large Iron Cage", "5000000", "1" });
            defaultTradeList.Add(new string[3] { "Large Iron Hanging Cage", "5000000", "1" });
            defaultTradeList.Add(new string[3] { "Leather Crest", "150000", "1" });
            defaultTradeList.Add(new string[3] { "Leather Hide", "40000", "1000" });
            defaultTradeList.Add(new string[3] { "Light Leather Boots", "300000", "1" });
            defaultTradeList.Add(new string[3] { "Light Leather Bracers", "300000", "1" });
            defaultTradeList.Add(new string[3] { "Light Leather Helmet", "300000", "1" });
            defaultTradeList.Add(new string[3] { "Light Leather Pants", "300000", "1" });
            defaultTradeList.Add(new string[3] { "Light Leather Vest", "300000", "1" });
            defaultTradeList.Add(new string[3] { "Liver", "20000", "25" });
            defaultTradeList.Add(new string[3] { "Lockpick", "400000", "25" });
            defaultTradeList.Add(new string[3] { "Log Block", "60000", "1000" });
            defaultTradeList.Add(new string[3] { "Log Ramp", "600000", "1000" });
            defaultTradeList.Add(new string[3] { "Log Stairs", "600000", "1000" });
            defaultTradeList.Add(new string[3] { "Long Horn", "250000", "1" });
            defaultTradeList.Add(new string[3] { "Lumber", "250000", "1000" });
            defaultTradeList.Add(new string[3] { "Meat", "20000", "25" });
            defaultTradeList.Add(new string[3] { "Medium Banner", "150000", "1" });
            defaultTradeList.Add(new string[3] { "Oil", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Pillory", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Potion Of Antidote", "500000", "25" });
            defaultTradeList.Add(new string[3] { "Potion Of Appearance", "500000", "25" });
            defaultTradeList.Add(new string[3] { "Rabbit Pelt", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Raw Bird", "30000", "25" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood (Iron) Block", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood (Iron) Door", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood (Iron) Gate", "200000", "1" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood (Iron) Ramp", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood (Iron) Stairs", "100000", "1000" });
            defaultTradeList.Add(new string[3] { "Reinforced Wood (Iron) Door", "150000", "1" });
            defaultTradeList.Add(new string[3] { "Repair Hammer", "150000", "1" });
            defaultTradeList.Add(new string[3] { "Rope", "80000", "1" });
            defaultTradeList.Add(new string[3] { "Roses", "50000", "25" });
            defaultTradeList.Add(new string[3] { "Sharp Rock", "10000", "100" });
            defaultTradeList.Add(new string[3] { "Small Banner", "100000", "1" });
            defaultTradeList.Add(new string[3] { "Small Gallows", "1200000", "1" });
            defaultTradeList.Add(new string[3] { "Small Iron Cage", "2000000", "1" });
            defaultTradeList.Add(new string[3] { "Small Iron Hanging Cage", "2000000", "1" });
            defaultTradeList.Add(new string[3] { "Small Wall Lantern", "1000000", "1" });
            defaultTradeList.Add(new string[3] { "Small Wall Torch", "700000", "1" });
            defaultTradeList.Add(new string[3] { "Sod Block", "80000", "1000" });
            defaultTradeList.Add(new string[3] { "Sod Ramp", "80000", "1000" });
            defaultTradeList.Add(new string[3] { "Sod Stairs", "80000", "1000" });
            defaultTradeList.Add(new string[3] { "Splintered Club", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Spruce Branches Block", "80000", "1000" });
            defaultTradeList.Add(new string[3] { "Spruce Branches Ramp", "80000", "1000" });
            defaultTradeList.Add(new string[3] { "Spruce Branches Stairs", "80000", "1000" });
            defaultTradeList.Add(new string[3] { "Standing Iron Torch", "800000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Axe", "2500000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Battle Axe", "2500000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Bolt", "250000", "100" });
            defaultTradeList.Add(new string[3] { "Steel Cage", "4000000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Chest", "3000000", "10" });
            defaultTradeList.Add(new string[3] { "Steel Compound", "180000", "1000" });
            defaultTradeList.Add(new string[3] { "Steel Crest", "7500000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Dagger", "2500000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Flanged Mace", "2500000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Greatsword", "3000000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Halberd", "3000000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Hatchet", "2500000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Ingot", "200000", "1000" });
            defaultTradeList.Add(new string[3] { "Steel Javelin", "1500000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Pickaxe", "2500000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Boots", "3000000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Gauntlets", "3000000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Helmet", "3000000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Pants", "3000000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Plate Vest", "3000000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Star Mace", "2500000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Sword", "3000000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Tipped Arrow", "120000", "100" });
            defaultTradeList.Add(new string[3] { "Steel War Hammer", "2500000", "1" });
            defaultTradeList.Add(new string[3] { "Steel Wood Cutters Axe", "2500000", "1" });
            defaultTradeList.Add(new string[3] { "Sticks", "10000", "1000" });
            defaultTradeList.Add(new string[3] { "Stiff Bed", "100000", "1" });
            defaultTradeList.Add(new string[3] { "Stone", "50000", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Arch", "500000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Arrow", "30000", "100" });
            defaultTradeList.Add(new string[3] { "Stone Block", "400000", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Cutter", "100000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Dagger", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Hatchet", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Javelin", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Pickaxe", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Ramp", "400000", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Slab", "250000", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Slit Window", "450000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Stairs", "400000", "1000" });
            defaultTradeList.Add(new string[3] { "Stone Sword", "30000", "1" });
            defaultTradeList.Add(new string[3] { "Stone Wood Cutters Axe", "40000", "1" });
            defaultTradeList.Add(new string[3] { "Tears Of The Gods", "150000", "10" });
            defaultTradeList.Add(new string[3] { "Thatch Block", "80000", "1000" });
            defaultTradeList.Add(new string[3] { "Thatch Ramp", "80000", "1000" });
            defaultTradeList.Add(new string[3] { "Thatch Stairs", "80000", "1000" });
            defaultTradeList.Add(new string[3] { "Throwing Stone", "30000", "100" });
            defaultTradeList.Add(new string[3] { "Tinker", "250000", "1" });
            defaultTradeList.Add(new string[3] { "Trebuchet", "12000000", "1" });
            defaultTradeList.Add(new string[3] { "Trebuchet Stone", "1000000", "50" });
            defaultTradeList.Add(new string[3] { "Wall Lantern", "1230000", "1" });
            defaultTradeList.Add(new string[3] { "Wall Torch", "1200000", "1" });
            defaultTradeList.Add(new string[3] { "Water", "20000", "1000" });
            defaultTradeList.Add(new string[3] { "Whip", "100000", "1" });
            defaultTradeList.Add(new string[3] { "Wood", "10000", "1000" });
            defaultTradeList.Add(new string[3] { "Wolf Pelt", "20000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Arrow", "40000", "100" });
            defaultTradeList.Add(new string[3] { "Wood Block", "30000", "1000" });
            defaultTradeList.Add(new string[3] { "Wood Bracers", "40000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Chest", "40000", "10" });
            defaultTradeList.Add(new string[3] { "Wood Door", "50000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Gate", "80000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Helmet", "40000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Ramp", "30000", "1000" });
            defaultTradeList.Add(new string[3] { "Wood Sandals", "40000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Shutters", "40000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Skirt", "40000", "1" });
            defaultTradeList.Add(new string[3] { "Wood Stairs", "40000", "1000" });
            defaultTradeList.Add(new string[3] { "Wood Vest", "40000", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Cage", "100000", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Flute", "80000", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Javelin", "40000", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Mace", "60000", "1" });
            defaultTradeList.Add(new string[3] { "Wooden Short Bow", "150000", "1" });
            defaultTradeList.Add(new string[3] { "Wool", "40000", "1000" });

			SaveTradeData();
            return defaultTradeList;
        }

		
		// ================================================================================================================================
		// ================================================================================================================================
		// YOU SHOULDN'T NEED TO EDIT ANYTHING BELOW HERE =================================================================================
		// ================================================================================================================================
		// ================================================================================================================================
		
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
		private System.Random random = new System.Random();

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
			tradeDefaults = LoadDefaultTradeValues();
            
			//If there's no trade data stored, then set up the new trade data from the defaults
            if(tradeList.Count < 1)
            {
                foreach(var item in tradeDefaults)
                {
                    var newItem = new string[5]{ item[0], item[1], item[2], item[1], item[1] };
                    tradeList.Add(newItem);
                }
            }
			
			// Start deflation timer
			timer.Repeat(priceDeflationTime,0,DeflatePrices);

            // Save the trade data
            SaveTradeData();
        }
        // ===========================================================================================================
		
		
		// Give credits when a player is killed
		private void OnEntityDeath(EntityDeathEvent deathEvent)
        {
			if(allowPvpGold)
			{
				if (deathEvent.Entity.IsPlayer)
				{
					var killer = deathEvent.KillingDamage.DamageSource.Owner;
					var player = deathEvent.Entity.Owner;

					// Make sure player didn't kill themselves
					if(player == killer) return;
					
					// Make sure the player is not in the same guild
					if(player.GetGuild().Name == killer.GetGuild().Name)
					{
						PrintToChat(player, "[FF0000]Grand Exchange[FFFFFF] : There is no honour - or more importantly, gold! - in killing a member of your own guild!");
						return;
					}
					
					// Make sure it was a player that killed them
					if(deathEvent.KillingDamage.DamageSource.IsPlayer)
					{
						// Get the inventory
						var inventory = killer.GetInventory();
						
						// Check victims wallet
						CheckWalletExists(player);
						// Check the player has a wallet
						CheckWalletExists(killer);
						// Give the rewards to the player
						var goldAmount = random.Next(50,goldRewardForPvp);
						var victimGold = wallet[player.Name.ToLower()];
						if(goldAmount > victimGold) goldAmount = victimGold;
						GiveGold(killer,goldAmount);
						RemoveGold(player,goldAmount);
						
						if(goldAmount == 0)
						{
							PrintToChat(killer, "[FF00FF]" + player.DisplayName + "[FFFFFF] had no gold for you to steal!");
						}
						else
						{
							// Notify everyone
							PrintToChat("[FF00FF]" + killer.DisplayName + "[FFFFFF] has stolen [00FF00]" + goldAmount.ToString() + "[FFFF00] gold [FFFFFF] from the dead body of [00FF00]" + player.DisplayName + "[FFFFFF]!");
						}
						
					}
				}
			}
			
			
			//Save the data
			SaveTradeData();
        }
		
		
		private void OnEntityHealthChange(EntityDamageEvent damageEvent) 
		{
			if(allowPveGold)
			{
				if (!damageEvent.Entity.IsPlayer)
				{
					var victim = damageEvent.Entity;
					Health h = victim.TryGet<Health>();
					if (!h.IsDead) return;
					
					var hunter = damageEvent.Damage.DamageSource.Owner;
					
					// Give the rewards to the player
					var goldAmount = random.Next(2,goldRewardForPve);
					GiveGold(hunter,goldAmount);
					
					// Notify everyone
					PrintToChat(hunter, "[00FF00]" + goldAmount.ToString() + "[FFFF00] gold[FFFFFF] collected.");
				}
			}
			SaveTradeData();
		}
		
		private void DeflatePrices()
		{
			int newBuyPrice = 0;
			int newSellPrice = 0;
			int priceBottomShelf = 0;
			int priceTopShelf = 0;
			string resource = "";
			
			foreach(var item in tradeList)
			{
				var buyPrice = Int32.Parse(item[3]);
				var sellPrice = Int32.Parse(item[4]);
				var maxStackSize = Int32.Parse(item[2]);
				var originalPrice = Int32.Parse(item[1]);
				
				double inflationModifier = inflation / 100;
				double deflationModifier = maxDeflation / 100;
				double stackModifier = 1;
				newBuyPrice = (int)(buyPrice - ((originalPrice * inflationModifier) * stackModifier));
				newSellPrice = (int)(sellPrice + ((originalPrice * inflationModifier) * stackModifier));
				
				// Make sure it doesn't fall below expected levels
				priceBottomShelf = (int)(originalPrice - ((originalPrice * deflationModifier) * stackModifier));
				priceTopShelf = (int)(originalPrice + ((originalPrice * deflationModifier) * stackModifier));
				
				if(newBuyPrice < priceBottomShelf) newBuyPrice = priceBottomShelf;
				if(newSellPrice > priceTopShelf) newSellPrice = priceTopShelf;
				
				//Update the current price
				item[3] = newBuyPrice.ToString();
				item[4] = newSellPrice.ToString();
				
				resource = item[0];
			}
			
			PrintToChat(resource + " BottomShelf = " + priceBottomShelf.ToString());
			PrintToChat(resource + " TopShelf = " + priceTopShelf.ToString());
			PrintToChat("Buy = " + newBuyPrice.ToString());
			PrintToChat("Sell = " + newSellPrice.ToString());

			// Save the trade data
            SaveTradeData();
		}
		
		private void AdjustMarketPrices(string type, string resource, int amount)
		{
			var recordNumber = 0;
			var newResource = new string[5];
			double inflationModifier = 0;
			double buyPrice = 0;
			double sellPrice = 0;
			double stackModifier = 0;
			
			for(var i=0;i<tradeList.Count;i++)
			{
				if(tradeList[i][0].ToLower() == resource.ToLower())
				{	
					var originalPrice = Int32.Parse(tradeList[i][1]);
					var newBuyPrice = Int32.Parse(tradeList[i][3]);
					var newSellPrice = Int32.Parse(tradeList[i][4]);
					var maxStackSize = Int32.Parse(tradeList[i][2]);
					recordNumber = i;
					
					//Update for "Buy"
					if(type == "buy")
					{
						//When resource is bought, increase buy price and decrease sell price for EVERY single item bought
						inflationModifier = inflation / 100;
						sellPrice = (double)newSellPrice;
						buyPrice = (double)newBuyPrice;
						stackModifier = (double)amount / (double)maxStackSize;
						newBuyPrice = (int)(buyPrice + ((originalPrice * inflationModifier) * stackModifier));
						newSellPrice = (int)(sellPrice + ((originalPrice * inflationModifier) * stackModifier));
						if(newSellPrice > originalPrice) newSellPrice = originalPrice;
						// PrintToChat("Original Price = " + originalPrice);
						// PrintToChat("NewSellPrice = " + newSellPrice);
					}
					
					//Update for "Sell"
					if(type == "sell")
					{
						//When resource is sold, increase sell price and decrease buy price for EVERY single item bought
						inflationModifier = inflation / 100;
						sellPrice = (double)newSellPrice;
						buyPrice = (double)newBuyPrice;
						stackModifier = (double)amount / (double)maxStackSize;
						newSellPrice = (int)(sellPrice - ((originalPrice * inflationModifier) * stackModifier));
						newBuyPrice = (int)(buyPrice - ((originalPrice * inflationModifier) * stackModifier));
						if(newBuyPrice < originalPrice) newBuyPrice = originalPrice;
						// PrintToChat("Original Price = " + originalPrice);
						// PrintToChat("NewSellPrice = " + newBuyPrice);
					}
					
					// Make sure prices don't go below 1gp!
					if(newBuyPrice <= (1 * priceModifier)) newBuyPrice = (1 * priceModifier);
					if(newSellPrice <= (1 * priceModifier)) newSellPrice = (1 * priceModifier);
					
					newResource = new string[5]{ tradeList[i][0],tradeList[i][1],tradeList[i][2],newBuyPrice.ToString(),newSellPrice.ToString() };
				}
			}
			
			if(newResource.Length < 1) return;
			tradeList.RemoveAt(recordNumber);
			tradeList.Insert(recordNumber,newResource);
			
			// Save the data
			SaveTradeData();
		}
		
		// Buying an item from the exchange
        [ChatCommand("givecredits")]
        private void AdminGiveCredits(Player player, string cmd,string[] input)
        {
			if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "This is not for you. Don't even try it, thieving scumbag!");
                return;
            }
			
			int amount;
			if(Int32.TryParse(input[1], out amount) == false)
			{
				PrintToChat(player, "That was not a recognised amount!");
                return;
			}
			
			var playerName = input[0];
			var target = Server.GetPlayerByName(playerName);
			
			if(target == null)
			{
				PrintToChat(player, "That player doesn't appear to be online right now!");
                return;
			}
			
			PrintToChat(player, "Giving " + amount.ToString() + " gold to " + playerName);
			PrintToChat(target, "You have received an Admin gift of " + amount.ToString() + " gold.");
			
			CheckWalletExists(target);
			GiveGold(target,amount);
			
			// Save the trade data
            SaveTradeData();
		}
		
		// Check a player's credits
        [ChatCommand("checkcredits")]
        private void AdminCheckPlayerCredits(Player player, string cmd,string[] input)
        {
			if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only admins can use this command.");
                return;
            }
			
			var playerName = input.JoinToString(" ");

			var target = Server.GetPlayerByName(playerName);
			if(target == null)
			{
				PrintToChat(player, "That player doesn't appear to be online right now!");
                return;
			}
			
			CheckWalletExists(target);
			var goldAmount = wallet[target.Name.ToLower()];
			PrintToChat(player, target.Name + " currently has " + goldAmount.ToString() + " gold.");
		}
		
		// Remove an item from the store
        [ChatCommand("removestoreitem")]
        private void AdminRemoveItemFromStore(Player player, string cmd,string[] input)
        {
			if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only admins can use this command.");
                return;
            }
			
			var resource = input.JoinToString(" ");
			int position = -1;
			for(var i=0; i<tradeList.Count; i++)
			{
				if(tradeList[i][0].ToLower() == resource.ToLower())
				{
					position = i;
					break;
				}
			}
			if(position >= 0)
			{
				tradeList.RemoveAt(position);
				PrintToChat(player, resource + "  has been removed from the store.");
			}
			else PrintToChat(player, "Could not find that item in the store to remove it.");
			

			//Save the data
			SaveTradeData();
		}

		// Enable the PvP gold stealing
        [ChatCommand("pvpgold")]
        private void AllowGoldForPvP(Player player, string cmd)
        {
			if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only admins can use this command.");
                return;
            }
			if(allowPvpGold)
			{	
				allowPvpGold = false;
				PrintToChat(player,"PvP gold stealing is now [FF0000]OFF");
				return;
			}
			allowPvpGold = true;
			PrintToChat(player,"PvP gold stealing is now [00FF00]ON");
			return;
		}
			
		// Enable the PvE gold farming
        [ChatCommand("pvegold")]
        private void AllowGoldForPvE(Player player, string cmd)
        {
			if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only admins can use this command.");
                return;
            }
			if(allowPveGold)
			{	
				allowPveGold = false;
				PrintToChat(player,"PvE gold farming is now [FF0000]OFF");
				return;
			}
			allowPveGold = true;
			PrintToChat(player,"PvP gold farming is now [00FF00]ON");
			return;
		}
			
		// Remove an item from the store
        [ChatCommand("restoredefaultprices")]
        private void RevertAllPricesToDefaultValues(Player player, string cmd)
        {
			if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only admins can use this command.");
                return;
            }
			
			tradeList = new Collection<string[]>();
			foreach(var item in tradeDefaults)
			{
				var newItem = new string[5]{ item[0], item[1], item[2], item[1], item[1] };
				tradeList.Add(newItem);
			}
			PrintToChat(player, "Grand Exchange prices have been reset to default values");
			

			//Save the data
			SaveTradeData();
		}

		// Add an item to the store
        [ChatCommand("addstoreitem")]
        private void AdminAddItemToStore(Player player, string cmd,string[] input)
        {
			if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only admins can use this command.");
                return;
            }
			
			// Check the current store
			var resource = input.JoinToString(" ");
			var found = false;
			for(var i=0; i<tradeList.Count; i++)
			{
				if(tradeList[i][0].ToLower() == resource.ToLower())
				{
					found = true;
					break;
				}
			}
			if(found)
			{
				PrintToChat(player, resource + "  already exists in the store!");
				return;
			}
			
			found = false;
			// Check if the item exists in the defaults
			var previousItem = new string[5];
			for(var i=0; i<tradeDefaults.Count; i++)
			{
				if(tradeDefaults[i][0].ToLower() == resource.ToLower())
				{
					found = true;
					previousItem = new string[5]{ tradeDefaults[i][0],tradeDefaults[i][1],tradeDefaults[i][2],tradeDefaults[i][1],tradeDefaults[i][1] };
					tradeList.Insert(i,previousItem);
					PrintToChat(player, resource + " has been added to the store!");
					break;
				}
			}
			if(!found)
			{
				PrintToChat(player, resource + " does not appear in the original defaults list. If you want this item added, please add it to the defaults list first. (In the plugin) Note - It MUST be a real item or the system may crash!");
				return;
			}
			
			//Save the data
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
			var message = "Of course!\n[00FF00]" + Capitalise(resourceDetails[0]) + "[FFFFFF] is currently selling for [00FFFF]" + (Int32.Parse(resourceDetails[3])/1000).ToString() + "[FFFF00]g[FFFFFF] per item.\nIt can be bought in stacks of up to [00FF00]" + resourceDetails[2].ToString() + "[FFFFFF].\n How much would you like to buy?";
			
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
				PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : That does not appear to be a valid amount. Please enter a number between 1 and the maximum stack size.");
				return;
			}
			
			//Check if the amount is within the correct limits
			if(amount < 1 || amount > Int32.Parse(resourceDetails[2]))
			{
				PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : You can only purchase an amount between 1 and the maximum stack size.");
				return;
			}
			
			var totalValue = GetPriceForThisItem("buy", resourceDetails[0],amount);
			
			var message = "Very good!\n[00FFFF]" + amount.ToString() + " [00FF00]" + Capitalise(resourceDetails[0]) + "[FFFFFF] will cost you a total of \n[FF0000]" + totalValue + " [FFFF00]gold.[FFFFFF]\n Do you want to complete the purchase?";
			
			// Get the player's wallet contents
			CheckWalletExists(player);
			var credits = wallet[player.Name.ToLower()];
			message = message + "\n\n[FF0000]Gold Available[FFFFFF] : [00FF00]" + credits.ToString();
			
			//Show Popup with the final price
			player.ShowConfirmPopup("Grand Exchange", message, "Submit", "Cancel", (options, dialogue1, data) => CheckIfThePlayerCanAffordThis(player, options, dialogue, data, resourceDetails, totalValue, amount));
		}
				
		private void CheckIfThePlayerHasTheResourceToSell(Player player, Options selection, Dialogue dialogue, object contextData, string[] resourceDetails, int totalValue, int amount)
		{
			if (selection != Options.Yes)
            {
                //Leave
                return;
            }
			
			if(!CanRemoveResource(player, resourceDetails[0], amount))
			{
				PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : It looks like you don't have the goods! What are you trying to pull here?");
				return;
			}
			
			// Take the item!
			RemoveItemsFromInventory(player, resourceDetails[0], amount);
			
			// Give the payment
			GiveGold(player, totalValue);
			
			// Fix themarket price adjustment
			AdjustMarketPrices("sell", resourceDetails[0] ,amount);
			
			// Tell the player
			PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : " + amount.ToString() + " " + resourceDetails[0] + "has been removed from your inventory and your wallet has been credited for the sale.");
			PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : Thanks for your custom, friend! Please come again!");
			
			//Save the data
			SaveTradeData();
		}
		
		private bool CanRemoveResource(Player player, string resource, int amount)
		{
            // Check player's inventory
            var inventory = player.CurrentCharacter.Entity.GetContainerOfType(CollectionTypes.Inventory);

            // Check how much the player has
            var foundAmount = 0;
            foreach (var item in inventory.Contents.Where(item => item != null))
            {
                if(item.Name == resource)
                {
                    foundAmount = foundAmount + item.StackAmount;
                }
            }

            if(foundAmount >= amount) return true;
            return false;
		}
		
		public void RemoveItemsFromInventory(Player player, string resource, int amount)
        {
            var inventory = player.GetInventory().Contents;

            // Check how much the player has
            var amountRemaining = amount;
            var removeAmount = amountRemaining;
            foreach (InvGameItemStack item in inventory.Where(item => item != null))
            {
                if(item.Name == resource)
                {
                    removeAmount = amountRemaining;

                    //Check if there is enough in the stack
                    if (item.StackAmount < amountRemaining)
                    {
                        removeAmount = item.StackAmount;
                    }

                    amountRemaining = amountRemaining - removeAmount;

                    inventory.SplitItem(item, removeAmount, true);
                    if (amountRemaining <= 0) return;
                }
            }
        }
		
		private void CheckIfThePlayerCanAffordThis(Player player, Options selection, Dialogue dialogue, object contextData, string[] resourceDetails, int totalValue, int amount)
		{
			if (selection != Options.Yes)
            {
                //Leave
                return;
            }
			
			if(!CanRemoveGold(player,totalValue))
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
			PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : " + amount.ToString() + " " + resourceDetails[0] + " has been added to your inventory and your wallet has been debited the appropriate amount.");
			PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : Congratulations on your purchase. Please come again!");
			
			//Save the data
			SaveTradeData();
		}
		
		
		private int GetPriceForThisItem(string type, string resource, int amount)
		{
			var position = 3;
			if(type == "sell") position = 4;
			
			var total = 0;
			foreach(var item in tradeList)
			{
				if(item[0].ToLower() == resource.ToLower())
				{
					total = (int)(amount * (double)(Int32.Parse(item[position]) / priceModifier));
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
				PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : I don't think I am currently able to take that item at this time.'");
				return;
			}
			
			// Open a popup with the resource details
			var message = "Hmmm!\nI believe that [00FF00]" + Capitalise(resourceDetails[0]) + "[FFFFFF] is currently being purchased for [00FFFF]" + (Int32.Parse(resourceDetails[4])/1000).ToString() + "[FFFF00]g[FFFFFF] per item.\nI'd be happy to buy this item in stacks of up to [00FF00]" + resourceDetails[2].ToString() + "[FFFFFF].\n How much did you want to sell?";
			
			// Get the player's wallet contents
			CheckWalletExists(player);
			var credits = wallet[player.Name.ToLower()];
			message = message + "\n\n[FF0000]Gold Available[FFFFFF] : [00FF00]" + credits.ToString();
			
			player.ShowInputPopup("Grand Exchange", message, "", "Submit", "Cancel", (options, dialogue1, data) => SelectAmountToBeSold(player, options, dialogue1, data, resourceDetails));
		}
		
		private void SelectAmountToBeSold(Player player, Options selection, Dialogue dialogue, object contextData, string[] resourceDetails)
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
				PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : That does not appear to be a valid amount. Please enter a number between 1 and the maximum stack size.");
				return;
			}
			
			//Check if the amount is within the correct limits
			if(amount < 1 || amount > Int32.Parse(resourceDetails[2]))
			{
				PrintToChat(player,"[FF0000]Grand Exchange[FFFFFF] : You can only sell an amount of items between 1 and the maximum stack size for that item.");
				return;
			}
			
			var totalValue = GetPriceForThisItem("sell", resourceDetails[0],amount);
			
			var message = "I suppose I can do that.\n[00FFFF]" + amount.ToString() + " [00FF00]" + Capitalise(resourceDetails[0]) + "[FFFFFF] will give you a total of \n[FF0000]" + totalValue + " [FFFF00]gold.[FFFFFF]\n Do you want to complete the sale?";
			
			// Get the player's wallet contents
			CheckWalletExists(player);
			var credits = wallet[player.Name.ToLower()];
			message = message + "\n\n[FF0000]Gold Available[FFFFFF] : [00FF00]" + credits.ToString();
			
			//Show Popup with the final price
			player.ShowConfirmPopup("Grand Exchange", message, "Submit", "Cancel", (options, dialogue1, data) => CheckIfThePlayerHasTheResourceToSell(player, options, dialogue, data, resourceDetails, totalValue, amount));
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
				buyIcon = "[008888]";
				sellIcon = "[008888]";
                var resource = tradeList[i][0];
                var originalPrice = Int32.Parse(tradeList[i][1]) / priceModifier;
                var stackLimit = Int32.Parse(tradeList[i][2]);
                var buyPrice = Int32.Parse(tradeList[i][3]) / priceModifier;
                var sellPrice = Int32.Parse(tradeList[i][4]) / priceModifier;
				
				if(buyPrice >= originalPrice) buyIcon = "[00FF00]";
				if(buyPrice > originalPrice + (originalPrice * 0.1)) buyIcon = "[888800]";
				if(buyPrice > originalPrice + (originalPrice * 0.2)) buyIcon = "[FF0000]";
				if(sellPrice <= originalPrice) sellIcon = "[00FF00]";
				if(sellPrice < originalPrice - (originalPrice * 0.1)) sellIcon = "[888800]";
				if(sellPrice < originalPrice - (originalPrice * 0.2)) sellIcon = "[FF0000]";
				// if(buyPrice < originalPrice + (originalPrice * 0.3)) buyIcon = "[00FF00]";
				// if(sellPrice > originalPrice) sellIcon = "[FF0000]";
				// if(sellPrice < originalPrice) sellIcon = "[00FF00]";
				var buyPriceText = buyPrice.ToString();
				var sellPriceText = sellPrice.ToString();
				
				
                itemText = itemText + "[888800]" + Capitalise(resource) + "[FFFFFF]; Buy: " + buyIcon + buyPriceText + "[FFFF00]g  [FFFFFF]Sell: " + sellIcon + sellPriceText + "[FFFF00]g\n";
            }
			
			itemText = itemText + "\n\n[FF0000]Gold Available[FFFFFF] : [00FF00]" + credits.ToString();
			
            //Display the Popup with the price
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
				buyIcon = "[008888]";
				sellIcon = "[008888]";
                var resource = tradeList[i][0];
                var originalPrice = Int32.Parse(tradeList[i][1]) / priceModifier;
                var stackLimit = Int32.Parse(tradeList[i][2]);
                var buyPrice = Int32.Parse(tradeList[i][3]) / priceModifier;
                var sellPrice = Int32.Parse(tradeList[i][4]) / priceModifier;
				
				if(buyPrice >= originalPrice) buyIcon = "[00FF00]";
				if(buyPrice > originalPrice + (originalPrice * 0.1)) buyIcon = "[888800]";
				if(buyPrice > originalPrice + (originalPrice * 0.2)) buyIcon = "[FF0000]";
				if(sellPrice <= originalPrice) sellIcon = "[00FF00]";
				if(sellPrice < originalPrice - (originalPrice * 0.1)) sellIcon = "[888800]";
				if(sellPrice < originalPrice - (originalPrice * 0.2)) sellIcon = "[FF0000]";
				// if(buyPrice > originalPrice) buyIcon = "[FF0000]";
				// if(buyPrice < originalPrice) buyIcon = "[00FF00]";
				// if(sellPrice > originalPrice) sellIcon = "[FF0000]";
				// if(sellPrice < originalPrice) sellIcon = "[00FF00]";
				var buyPriceText = buyPrice.ToString();
				var sellPriceText = sellPrice.ToString();
				

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
		
		// Buying an item from the exchange
        [ChatCommand("listtradedefaults")]
        private void ListTheDefaultTrades(Player player, string cmd,string[] input)
        {
			if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "This is not for you. Don't even try it, thieving scumbag!");
                return;
            }
			foreach(var item in tradeDefaults)
			{
				PrintToChat(player,item[0]);
			}
		}
		
		// Buying an item from the exchange
        [ChatCommand("setprice")]
        private void AdminSetResourcePrice(Player player, string cmd,string[] input)
        {
			if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "This is not for you. Don't even try it, thieving scumbag!");
                return;
            }
			
			if(input.Length == 0)
			{
			    PrintToChat(player, "Usage: Type /setprice 'Resource_in_Quotes' <amount>");
                return;
            }
			
			var resource = input[0];
			var priceText = input[1];
			int price;
			if(Int32.TryParse(priceText, out price) == false)
			{
				PrintToChat(player, "Bad amount value entered!");
				return;
			}
			
			priceText = (price * 1000).ToString();
			
			
			foreach(var item in tradeList)
			{
				if(item[0].ToLower() == resource.ToLower())
				{
					item[1] = priceText;
					item[3] = priceText;
					item[4] = priceText;
				}
			}
			
			PrintToChat(player, "Changing price of " + resource + " to " + priceText);
			
			// Save the trade data
            SaveTradeData();
		}
		
		// Check my wallet
        [ChatCommand("wallet")]
        private void CheckHowMuchMoneyAPlayerhas(Player player, string cmd)
        {
			CheckWalletExists(player);
			var walletAmount = wallet[player.Name.ToLower()];
			PrintToChat(player,"You currently have [00FF00]" + walletAmount.ToString() + "[FFFF00]g[FFFFFF].");
		}
	}
}
