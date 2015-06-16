using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using CodeHatch.Engine.Networking;
using CodeHatch.Common;
using Oxide.Core;
using CodeHatch.Networking.Events;
using CodeHatch.Networking.Events.Entities;
using CodeHatch.Networking.Events.Entities.Players;
using CodeHatch.Networking.Events.Players;

namespace Oxide.Plugins
{
    [Info("TeleToMe", "Scorpyon", "1.0.1")]
    public class TeleportToMe : ReignOfKingsPlugin
    {
        void Log(string msg) => Puts($"{Title} : {msg}");
		
        // USAGE : /teletome "player Name"
        [ChatCommand("teletome")]
        private void AdminTeleportToLocation(Player player, string cmd, string[] input)
        {
            if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only admins can use this command.");
                return;
            }
            if(input.Length < 1) return;
            var playerName = input[0];
            TeleportPlayerToMe(player, playerName);
        }

        private void TeleportPlayerToMe(Player player, string playerName)
        {
            var target = Server.GetPlayerByName(playerName);
            if(target == null) return;
            double posX = target.Entity.Position.x;
			double posZ = target.Entity.Position.z;
			
			posX = player.Entity.Position.x + 2;
            posZ = player.Entity.Position.z;
			
			PrintToChat(player, "Player has been moved to you.");
        }
    }
}
