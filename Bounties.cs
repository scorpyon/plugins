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
    [Info("Bounties", "Scorpyon", "1.0.1")]
    public class Bounties : ReignOfKingsPlugin
    {
        private Collection<string[]> bountyList = new Collection<string[]>();
        void Log(string msg) => Puts($"{Title} : {msg}");

        // SAVE DATA ===============================================================================================

		private void LoadBountyData()
		{
            bountyList = Interface.GetMod().DataFileSystem.ReadObject<Collection<string[]>>("SavedBountyList");
		}

        private void SaveBountyListData()
        {
            Interface.GetMod().DataFileSystem.WriteObject("SavedBountyList", bountyList);
        }
        
        void Loaded()
        {            
            LoadBountyData();
		}


        // ===========================================================================================================

        //[ChatCommand("testremove")]
        //private void TestRemovalScript(Player player, string cmd)
        //{
        //    var inventory = player.CurrentCharacter.Entity.GetContainerOfType(CollectionTypes.Inventory);
        //    var item = "Wood";
        //    var amount = 600;
        //    RemoveItemsFromInventory(player, item, amount);
        //}
		
		// THIS CONTROLS WHEN A PLAYER IS KILLED
		private void OnEntityDeath(EntityDeathEvent deathEvent)
        {
            if (deathEvent.Entity.IsPlayer)
            {
                var killer = deathEvent.KillingDamage.DamageSource.Owner;
                var player = deathEvent.Entity.Owner;
				
				// Check for bounties
				var reward = GetBountyOnPlayer(player);
				if (reward.Count < 1) return;
				
				// Get the inventory
				var inventory = player.GetInventory();
				
				// Give the rewards to the player
				foreach(var bounty in reward)
				{
					var resource = bounty.Key;
					var amount = bounty.Value;
					// Create a blueprint
					var blueprintForName = InvDefinitions.Instance.Blueprints.GetBlueprintForName(resource, true, true);
					// Create item stack
					var invGameItemStack = new InvGameItemStack(blueprintForName, amount, null);
					// Add the reward to the inventory
					ItemCollection.AutoMergeAdd(inventory.Contents, invGameItemStack);
				}
				
				// Notify everyone
				PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : [00FF00]" + killer.DisplayName + " has ended + [FF00FF]" + player.DisplayName + "'s life and has secured the bounty on their head!");
            }
        }
		
		private Dictionary<string,int> GetBountyOnPlayer(Player player)
		{
			var reward = new Dictionary<string,int>();
			
			foreach(var bounty in bountyList)
			{
				if(bounty[1].ToLower() == player.Name.ToLower() && bounty[4] == "active")
				{
					// Add this bounty to the list of rewards
					reward.Add(bounty[2],Int32.Parse(bounty[3]));
				}
			}
			
			return reward;
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

        // SEE THE CURRENT BOUNTY LIST
        [ChatCommand("bounties")]
        private void ViewTheCurrentBounties(Player player, string cmd)
        {
            var title = "Active Bounties";
            var message = "";

            foreach(var found in bountyList)
            {
                PrintToChat(found[0] + " " + found[1] + " " + found[2] + " " + found[3] + " " + found[4]);
            }

            if(bountyList.Count <= 0)
            {
                message = "There are currently no bounties available.";
            }
            else
            {
                var count = 0;
                for(var i=0; i<bountyList.Count;i++)
                {
                    if(bountyList[i][4] == "active") 
                    {
                        count++;
                        message = message + "[FF0000]" + bountyList[i][1] + "[FFFFFF] - [00FF00]" + bountyList[i][2] + " " + bountyList[i][3] + "[FFFFFF]\n (Set by [FF00FF]" + bountyList[i][0] + "[FFFFFF]) \n";
                    }
                }
                if(count == 0) message = "There are currently no bounties available.";
            }

            player.ShowPopup(title,message);
        }


        [ChatCommand("setbounty")]
        private void SetTheFinalBountyOnThePlayer(Player player, string cmd)
        {
            var playerName = player.Name;

            // Check if there is a bounty waiting to be set
            foreach(var bounty in bountyList)
            {
                if(bounty[0] == playerName.ToLower())
                {
                    // Is it awaiting the active state?
                    if(bounty[4] != "active")
                    {
                        //Check all fields are filled in
                        if(bounty[1] == "")
                        {
                            PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : You must use [00FF00]/setbountyname [FF00FF]<playername> [FFFFFF]to choose who to set your bounty on.");
                            return;
                        }

                        if(bounty[2] == "")
                        {
                            PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : You must use [00FF00]/setbountyresource [FF00FF]<resource> [FFFFFF]to choose what resource to offer as a reward.");
                            return;
                        }

                        if(bounty[3] == "")
                        {
                            PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : You must use [00FF00]/setbountyamount [FF00FF]<amount> [FFFFFF]to choose how much of the reward resource you want to offer.");
                            return;
                        }

                        // Make sure the player has enough resource for this!
                        if(!PlayerHasTheResources(player, bounty[2], bounty[3])) 
                        {
                            PrintToChat("[FF0000]Assassin's Guild[FFFFFF] : You do not have the resources for this bounty in your inventory!");
                            return;
                        }

                        // Remove the resource
                        int bountyAmount = Int32.Parse(bounty[3]); 
                        RemoveItemsFromInventory(player, bounty[2], bountyAmount);

                        PrintToChat("[FF0000]Assassin's Guild[FFFFFF] : Setting up bounty...");
                        SaveBountyListData();
                        AskThePlayerToConfirmTheBounty(player, bounty[0], bounty[1], bounty[2], bounty[3]);
                        return;
                    }
                }
            }

            PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : To set a bounty, use the following commands: [00FF00]/setbountyname [FF00FF]<player_name>[FFFFFF], [00FF00]/setbountyresource [FF00FF]<resource>[FFFFFF], [00FF00]/setbountyamount [FF00FF]<amount> [00FFFF](Max 1k), [00FF00]/setbounty [00FFFF](Confirms the bounty to begin)");

            // Save the data
            SaveBountyListData();
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

        private bool PlayerHasTheResources(Player player, string resource, string amountAsString)
        {
            // Convert the amount to int
            var amount = Int32.Parse(amountAsString);

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

        private void AskThePlayerToConfirmTheBounty(Player player, string playerName, string bountyName, string bountyResource, string bountyAmount)
        {
            var title = "Bounty Declared!";
            var message = "[FFFFFF]You have set a bounty reward of [FF0000]" + bountyAmount + " " + bountyResource + "[FFFFFF] for the death of [00FF00]" + Capitalise(bountyName) + "[FFFFFF]!";
            var confirmText = "Make it so!";
            var cancelText = "Actually, no...";
            bool interupt = false;
            bool broadcast = false;

            player.ShowPopup(title,message);
            //PlayerExtensions.ShowConfirmPopup(player,title,message,"Let's do it!","Nope!",ConfirmTheBounty(player, playerName, bountyName, bountyResource, bountyAmount));

            // TEMPORARY FIX 
            ConfirmTheBounty(player, playerName, bountyName, bountyResource, bountyAmount);
        }

        private void ConfirmTheBounty(Player player, string playerName, string bountyName, string bountyResource, string bountyAmount)
        {
            var guild = PlayerExtensions.GetGuild(player).Name;
            PrintToChat("[FF0000]Assassin's Guild[FFFFFF] : [00FF00]" + player.DisplayName + "[FFFFFF] of [FF00FF]" + Capitalise(guild) + "[FFFFFF] has set a bounty reward of [FF0000]" + bountyAmount + " " + bountyResource + "[FFFFFF] for the death of [00FF00]" + Capitalise(bountyName) + "[FFFFFF]!");

            // Confirm the bonty in the list
            foreach(var bounty in bountyList)
            {
                if(bounty[0] == playerName.ToLower() && bounty[1] == bountyName.ToLower() && bounty[2] == bountyResource && bounty[3] == bountyAmount)
                {
                    bounty[4] = "active";
                }
            }
        }
        
        [ChatCommand("resetbounty")]
        private void ResetAllBounties(Player player, string cmd)
        {
            if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only an admin can reset all bounties.");
                return;
            }

            // Reset the list
            bountyList = new Collection<string[]>();
            
            PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : You have removed all bounties on all players.");

            // Save the data
            SaveBountyListData();
        }



        [ChatCommand("seeallbounty")]
        private void SeeAllBountiesOnServer(Player player, string cmd)
        {
            if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only an admin can use this command.");
                return;
            }

            if(bountyList.Count < 1)
            {
                PrintToChat(player, "[FF0000]Bounty[FFFFFF] : There are no active bounties at this time.");
                return;
            }

            // Reset the list
            var bountyText = "";
            foreach(var bounty in bountyList)
            {
                foreach(var word in bounty)
                {
                    bountyText = string.Format(bountyText + " {0}", word);
                }
            }

            PrintToChat(player, "[FF0000]Bounty[FFFFFF] : " + bountyText);
        }

        
        //SETTING A BOUNTY AMOUNT
        [ChatCommand("setbountyamount")]
        private void SetBountyAmountOfResource(Player player, string cmd, string[] input)
        {
            var playerName = player.Name.ToLower();

            //Check player has entered the commands correctly
            if (input.Length == 0)
            {
                PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : To set a bounty, use the following commands: [00FF00]/setbountyname [FF00FF]<player_name>[FFFFFF], [00FF00]/setbountyresource [FF00FF]<resource>[FFFFFF], [00FF00]/setbountyamount [FF00FF]<amount> [00FFFF](Max 1k), [00FF00]/setbounty [00FFFF](Confirms the bounty to begin)");
                return;
            }

            // Convert to a single string (in case of many args)
            var amountEntered = ConvertArrayToString(input);

            // Make sure that a Number was entered
            int amount;
            bool acceptable = Int32.TryParse(amountEntered, out amount);
            if(!acceptable)
            {
                PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : That amount was not recognised.");
                PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : To set a bounty, use the following commands: [00FF00]/setbountyname [FF00FF]<player_name>[FFFFFF], [00FF00]/setbountyresource [FF00FF]<resource>[FFFFFF], [00FF00]/setbountyamount [FF00FF]<amount> [00FFFF](Max 1k), [00FF00]/setbounty [00FFFF](Confirms the bounty to begin)");
                return;
            }

            //If the number is too little or too much
            if(amount <= 0 || amount > 1000)
            {
                PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : The amount entered must be between 1 and 1000.");
                return;
            }
            
            // Check if I have already set an amiount
            foreach(var bounty in bountyList)
            {
                if(bounty[0] == playerName.ToLower())
                {
                    if(bounty[4] != "active")
                    {
                        // Add the resource to the existing bounty request
                        bounty[3] = amount.ToString();
                        PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : You have set the amount to [00FF00]" + amount.ToString() + "[FFFFFF] for the bounty you are creating. If you have added a name and amount, use [00FF00]/setbounty [FFFFFF]to confirm it.");
                        SaveBountyListData();
                        return;
                    }
                }
            }

            // Create a new bounty to add this resource
            bountyList.Add(CreateEmptyBountyListing());

            // Add the player's name to the bounty listing
            var lastRecord = bountyList.Count - 1;
            bountyList[lastRecord][0] = playerName.ToLower();

            // Add the amount
            bountyList[lastRecord][3] = amount.ToString();

            // Tell the player
             PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : You have set the amount to [00FF00]" + amount.ToString() + "[FFFFFF] for the bounty you are creating. If you have added a name and amount, use [00FF00]/setbounty [FFFFFF]to confirm it.");

            //// Save the data
            SaveBountyListData();
        }



        //SETTING A BOUNTY RESOURCE
        [ChatCommand("setbountyresource")]
        private void SetBountyResourceType(Player player, string cmd, string[] input)
        {
            //Check player has entered the commands correctly
            if (input.Length == 0)
            {
                PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : To set a bounty, use the following commands: [00FF00]/setbountyname [FF00FF]<player_name>[FFFFFF], [00FF00]/setbountyresource [FF00FF]<resource>[FFFFFF], [00FF00]/setbountyamount [FF00FF]<amount> [00FFFF](Max 1k), [00FF00]/setbounty [00FFFF](Confirms the bounty to begin)");
                return;
            }

            // Check who is setting the bounty
            var playerName = player.Name;

            // Get resource
            var resourceName = ConvertArrayToString(input);

            if(resourceName != "Wood" && 
                resourceName != "Stone" && 
                resourceName != "Iron" && 
                resourceName != "Flax" && 
                resourceName != "Iron Ingot" && 
                resourceName != "Steel Ingot" && 
                resourceName != "Water" && 
                resourceName != "Lumber" && 
                resourceName != "Wool" && 
                resourceName != "Bone" && 
                resourceName != "Sticks" && 
                resourceName != "Hay" && 
                resourceName != "Dirt" && 
                resourceName != "Clay" && 
                resourceName != "Oil")
            {
                PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : I am afraid that you cannot use that item as a bounty reward at this time. Please remember that resource names are [ff0000]case sensitive[ffffff] and must start with a capital letter.");
                return;
            }

            // Check if I have already set a resource
            foreach(var bounty in bountyList)
            {
                if(bounty[0] == playerName.ToLower())
                {
                    if(bounty[4] != "active")
                    {
                        // Add the resource to the existing bounty request
                        bounty[2] = resourceName;
                        PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : You have added the resource [00FF00]" + resourceName + "[FFFFFF] to the bounty you are creating. If you have added a name and amount, use [00FF00]/setbounty [FFFFFF]to confirm it.");
                        SaveBountyListData();
                        return;
                    }
                }
            }

            // Create a new bounty to add this resource
            bountyList.Add(CreateEmptyBountyListing());

            // Add the player's name to the bounty listing
            var lastRecord = bountyList.Count - 1;
            bountyList[lastRecord][0] = playerName.ToLower();

            // Add the resource
            bountyList[lastRecord][2] = resourceName;

            // Tell the player
            PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : You have added the resource [00FF00]" + resourceName + "[FFFFFF] to the bounty you are creating. If you have added a name and amount, use [00FF00]/setbounty [FFFFFF]to confirm it.");

            // Save the data
            SaveBountyListData();
        }



        // SETTING A BOUNTY NAME
        [ChatCommand("setbountyname")]
        private void SetBountyPlayerName(Player player, string cmd, string[] input)
        {
            var bountyPlayerName = "";

            //Check player has entered the commands correctly
            if (input.Length == 0)
            {
                PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : To set a bounty, use the following commands: [00FF00]/setbountyname [FF00FF]<player_name>[FFFFFF], [00FF00]/setbountyresource [FF00FF]<resource>[FFFFFF], [00FF00]/setbountyamount [FF00FF]<amount> [00FFFF](Max 1k), [00FF00]/setbounty [00FFFF](Confirms the bounty to begin)");
                return;
            }

            // Check who is setting the bounty
            var playerName = player.Name;

            // Get target's name
            bountyPlayerName = ConvertArrayToString(input);

            // Check that the bounty target is online
            Player bountyPlayer = Server.GetPlayerByName(bountyPlayerName.ToLower());

            //Check that this player can be found
            if (bountyPlayer == null)
            {
                PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : That person is not currently available. You must wait until they awaken to set a bounty on their head, my Lord.");
                return;
            }

            // Check if I have already set a bounty on this player's head.
            foreach(var bounty in bountyList)
            {
                if(bounty[0] == playerName.ToLower())
                {   
                    if(bounty[1] == bountyPlayerName.ToLower())
                    {
                        if(bounty[4] == "active")
                        {
                            PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : You have already set an active bounty on that person's head, my Lord!");
                            return;
                        }
                        else
                        {
                            PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : You have already added that person's name to the bounty you are creating. If you have added a resource and amount, use [00FF00]/setbounty [FFFFFF]to confirm it.");
                            return;
                        }
                    }
                    if(bounty[1] == "")
                    {
                        //Add targets name here
                        bounty[1] = bountyPlayerName.ToLower();

                        // Tell the player
                        PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : You have added [00FF00]" + Capitalise(bountyPlayerName) + "[FFFFFF]'s name to the bounty you are creating. If you have added a resource and amount, use [00FF00]/setbounty [FFFFFF]to confirm it.");

                        // Save the data
                        SaveBountyListData();

                        return;
                    }
                }
            }

            // Create a new bounty listing
            bountyList.Add(CreateEmptyBountyListing());

            // Add the player's name to the bounty listing
            var lastRecord = bountyList.Count - 1;
            bountyList[lastRecord][0] = playerName.ToLower();

            // Add the target's name to the bounty
            bountyList[lastRecord][1] = bountyPlayerName.ToLower();

            // Tell the player
            PrintToChat(player, "[FF0000]Assassin's Guild[FFFFFF] : You have added [00FF00]" + Capitalise(bountyPlayerName) + "[FFFFFF]'s name to the bounty you are creating. If you have added a resource and amount, use [00FF00]/setbounty [FFFFFF]to confirm it.");

            // Save the data
            SaveBountyListData();
        }

        
        private string[] CreateEmptyBountyListing()
        {
            string[] newBounty = new string[5] { "","","","","" };
            return newBounty;
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
	}
}
