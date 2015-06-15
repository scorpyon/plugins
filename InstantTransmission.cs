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
    [Info("InstantTransmission Tracker", "Scorpyon", "1.0.1")]
    public class InstantTransmission : ReignOfKingsPlugin
    {
        void Log(string msg) => Puts($"{Title} : {msg}");
		
#region User Commands

        // Teleport to another location /teletoloc <xcoord> <ycoord> <zcoord>
        [ChatCommand("teletoloc")]
        private void AdminTeleportToLocation(Player player, string cmd, string[] input)
        {
            TeleportToLocation(player, cmd, input);
        }

#endregion


#region Private Methods

//        private void OnPlayerChat(PlayerChatEvent chatEvent)
        private void OnPlayerChat(PlayerEvent chatEvent)
        {
            if(chatEvent != null && chatEvent.Player != null)
            {
                chatEvent.Player.DisplayNameFormat = "[00FF00](KING) [FFFF00]%name%[FFFFFF]";
            }
        }

        private void TeleportToLocation(Player player, string cmd, string[] input)
        {
            //var xCoord = input[0];
            //var yCoord = input[1];
            //var zCoord = input[2];

            PrintToChat("NEW Scorpyon: Actually I want this message.");
            //PrintToChat(player,"Teleporting...");
            //PrintToChat("Teleporting...");
        }

#endregion

    }
}
