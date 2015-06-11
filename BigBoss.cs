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
    [Info("BigBoss", "Scorpyon", "1.0.1")]
    public class BigBoss : ReignOfKingsPlugin
    {
		private Player theBoss;
	
		[ChatCommand("setboss")]
        private void CheckWarTimeCommand(Player player, string cmd, string[] input)
		{
			if (!player.HasPermission("admin")) 
			{
				PrintToChat("You must be an admin to use this command.");
				return;
			}
			
			// Get target's name
            var playerName = ConvertArrayToString(input);
			
			var targetPlayer = Server.GetPlayerByName(playerName);
			if(targetPlayer == null)
			{
				PrintToChat("That player does not appear to be online.");
				return;	
			}
			
			//Set the player to be the boss
			
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
