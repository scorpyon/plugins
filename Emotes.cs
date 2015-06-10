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



namespace Oxide.Plugins
{
    [Info("Emotes", "Scorpyon", "1.0.1")]
    public class Emotes : ReignOfKingsPlugin
    {
        private const string defaultName = "everyone";
        private Random random = new Random();

        [ChatCommand("emote")]
        private void EmotePlayer(Player player, string cmd, string[] targetPlayer)
        {
            DisplayEmoteTextFor("emote", player, targetPlayer);
        }

        [ChatCommand("taunt")]
        private void TauntPlayer(Player player, string cmd, string[] targetPlayer)
        {
            DisplayEmoteTextFor("taunt", player, targetPlayer);
        }

        [ChatCommand("cheer")]
        private void CheerPlayer(Player player, string cmd, string[] targetPlayer)
        {
            DisplayEmoteTextFor("cheer", player, targetPlayer);
        }

        [ChatCommand("kiss")]
        private void KissPlayer(Player player, string cmd, string[] targetPlayer)
        {
            DisplayEmoteTextFor("kiss", player, targetPlayer);
        }

        [ChatCommand("wave")]
        private void WavePlayer(Player player, string cmd, string[] targetPlayer)
        {
            DisplayEmoteTextFor("wave", player, targetPlayer);
        }

        [ChatCommand("smile")]
        private void SmilePlayer(Player player, string cmd, string[] targetPlayer)
        {
            DisplayEmoteTextFor("smile", player, targetPlayer);
        }

        [ChatCommand("dance")]
        private void DancePlayer(Player player, string cmd, string[] targetPlayer)
        {
            DisplayEmoteTextFor("dance", player, targetPlayer);
        }

        [ChatCommand("slap")]
        private void SlapPlayer(Player player, string cmd, string[] targetPlayer)
        {
            DisplayEmoteTextFor("slap", player, targetPlayer);
        }


        private void DisplayEmoteTextFor(string emoteType, Player player, string[] targetPlayer)
        {
            var playerName = player.DisplayName;
            var targetName = "";
            if (targetPlayer.Length == 0) targetName = defaultName;
            else
            {
                targetName = targetPlayer[0];
                if (targetPlayer.Length > 1)
                {
                    for (var i = 1; i < targetPlayer.Length; i++)
                    {
                        targetName = string.Format(targetName + " {0}", targetPlayer[i]);
                    }
                }

                // Find the chosen target player
                Player targetPlayerSpecific = Server.GetPlayerByName(targetName);

                //Check that this player can be found
                if (targetPlayerSpecific == null)
                {
                    PrintToChat(player, "[FF0000]War Squire[FFFFFF] : That person is not available. Perhaps they exist only in your imagination my Lord?");
                    return;
                }
            }

            if (string.Compare(playerName.ToLower(), targetName.ToLower()) == 0)
            {
                targetName = defaultName;
            }

            if (emoteType == "emote") { Emote(player); }
            else if (emoteType == "taunt") { Taunt(playerName, targetName); }
            else if (emoteType == "kiss") { Kiss(playerName, targetName); }
            else if (emoteType == "wave") { Wave(playerName, targetName); }
            else if (emoteType == "smile") { Smile(playerName, targetName); }
            else if (emoteType == "dance") { Dance(playerName, targetName); }
            else if (emoteType == "slap") { Slap(playerName, targetName); }
            else if (emoteType == "cheer") { Cheer(playerName, targetName); }
            else PrintToChat(player, "That emote was not recognised.");
        }

        private void Emote(Player player)
        {
            PrintToChat(player, "List of Emotes");
            PrintToChat(player, "[00FF00]/taunt");
            PrintToChat(player, "[00FF00]/wave");
            PrintToChat(player, "[00FF00]/kiss");
            PrintToChat(player, "[00FF00]/smile");
            PrintToChat(player, "[00FF00]/dance");
            PrintToChat(player, "[00FF00]/slap");
            PrintToChat(player, "[00FF00]/cheer");
        }

        private void Taunt(string player, string target)
        {
            var num = random.Next(0, 2);

            switch (num)
            {
                case 0: PrintToChat("[FF0000]" + player + "[FFFFFF] sticks two fingers up at [FF0000]" + target + "[FFFFFF].");
                    break;
                case 1: PrintToChat("[FF0000]" + player + "[FFFFFF] blows raspberries at [FF0000]" + target + "[FFFFFF].");
                    break;
                default: PrintToChat("[FF0000]" + player + "[FFFFFF] sticks two fingers up at [FF0000]" + target + "[FFFFFF].");
                    break;
            }
        }

        private void Kiss(string player, string target)
        {
            var num = random.Next(0, 2);

            switch (num)
            {
                case 0: PrintToChat("[FF0000]" + player + "[FFFFFF] plants a big smacker on [FF0000]" + target + "[FFFFFF]'s lips.");
                    break;
                case 1: PrintToChat("[FF0000]" + player + "[FFFFFF] gives [FF0000]" + target + "[FFFFFF] a big kiss.");
                    break;
                default: PrintToChat("[FF0000]" + player + "[FFFFFF] plants a big smacker on [FF0000]" + target + "[FFFFFF]'s lips.");
                    break;
            }
        }

        private void Wave(string player, string target)
        {
            var num = random.Next(0, 2);

            switch (num)
            {
                case 0: PrintToChat("[FF0000]" + player + "[FFFFFF] waves at [FF0000]" + target + "[FFFFFF]. Coo-eee!");
                    break;
                case 1: PrintToChat("[FF0000]" + player + "[FFFFFF] flings their arms around wildly, trying to get [FF0000]" + target + "[FFFFFF]'s attention.");
                    break;
                default: PrintToChat("[FF0000]" + player + "[FFFFFF] waves at [FF0000]" + target + "[FFFFFF]. Coo-eee!");
                    break;
            }
        }

        private void Smile(string player, string target)
        {
            var num = random.Next(0, 2);

            switch (num)
            {
                case 0: PrintToChat("[FF0000]" + player + "[FFFFFF] smiles at [FF0000]" + target + "[FFFFFF].");
                    break;
                case 1: PrintToChat("[FF0000]" + player + "[FFFFFF] grins at [FF0000]" + target + "[FFFFFF], like a looney!");
                    break;
                default: PrintToChat("[FF0000]" + player + "[FFFFFF] smiles at [FF0000]" + target + "[FFFFFF].");
                    break;
            }
        }

        private void Dance(string player, string target)
        {
            var num = random.Next(0, 2);

            switch (num)
            {
                case 0: PrintToChat("[FF0000]" + player + "[FFFFFF] does a little jig for [FF0000]" + target + "[FFFFFF].");
                    break;
                case 1: PrintToChat("[FF0000]" + player + "[FFFFFF] prances around, doing a funny little dance for [FF0000]" + target + "[FFFFFF].");
                    break;
                default: PrintToChat("[FF0000]" + player + "[FFFFFF] does a little jig for [FF0000]" + target + "[FFFFFF].");
                    break;
            }
        }

        private void Slap(string player, string target)
        {
            var num = random.Next(0, 3);

            switch (num)
            {
                case 0: PrintToChat("[FF0000]" + player + "[FFFFFF] slaps [FF0000]" + target + "[FFFFFF] across the face.");
                    break;
                case 1: PrintToChat("[FF0000]" + player + "[FFFFFF] grabs a big, wet trout and slaps [FF0000]" + target + "[FFFFFF] across the face with it.");
                    break;
                case 2: PrintToChat("[FF0000]" + player + "[FFFFFF] gives [FF0000]" + target + "[FFFFFF] a big slap in the face. Stop it!");
                    break;
                default: PrintToChat("[FF0000]" + player + "[FFFFFF] slaps [FF0000]" + target + "[FFFFFF] across the face.");
                    break;
            }
        }

        private void Cheer(string player, string target)
        {
            var num = random.Next(0, 2);

            switch (num)
            {
                case 0: PrintToChat("[FF0000]" + player + "[FFFFFF] raises a horn of mead with [FF0000]" + target + "[FFFFFF]. Cheers!");
                    break;
                case 1: PrintToChat("[FF0000]" + player + "[FFFFFF] gives a big shout for joy and [FF0000]" + target + "[FFFFFF] joins in, in celebration.");
                    break;
                default: PrintToChat("[FF0000]" + player + "[FFFFFF] raises a hron of mead with [FF0000]" + target + "[FFFFFF]. Cheers!");
                    break;
            }
        }
    }
}
