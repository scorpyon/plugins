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
using CodeHatch.Engine.Core.Cache;
using CodeHatch.Blocks.Networking.Events;
using CodeHatch.UserInterface.Dialogues;

namespace Oxide.Plugins
{
    [Info("Popups", "Mughisi", 1.0)]
    public class Popups : ReignOfKingsPlugin
    {
       
        [ChatCommand("popup")]
        private void Popup(Player player)
        {
            player.ShowConfirmPopup("Buttons?", "Do you want to press a button?", "Yes", "No", (selection, dialogue, data) => HandlePopup(player, selection, dialogue, data));
        }
       
        [ChatCommand("popup2")]
        private void Popup2(Player player)
        {
            PlayerExtensions.ShowPopup(player,"Buttons?", "Do you want to press a button?", "Yes",  (selection, dialogue, data) => HandlePopup(player, selection, dialogue, data));
        }
       
        private void HandlePopup(Player player, Options selection, Dialogue dialogue, object contextData)
        {
            // Available options: Options.Yes & Options.No
            if (selection == Options.Yes)
            {
                PrintToChat(player, "Thanks for your pressing Yes!");
                return;
            }
            player.ShowInputPopup("Why???", "Why didn't you press 'Yes'?", "", "Submit", "Cancel", (options, dialogue1, data) => HandleInputPopup(player, options, dialogue1, data));
        }

        private void HandleInputPopup(Player player, Options selection, Dialogue dialogue, object contextData)
        {
            // Available options: Options.OK & Options.Cancel
            if (selection == Options.Cancel || dialogue.ValueMessage.Length == 0)
            {
                PrintToChat(player, "You are just rude!");
                return;
            }
            PrintToChat(player, "Thanks for letting us know!");
            PrintToChat(player, "Your input: " + dialogue.ValueMessage);
        }
       
    }
}