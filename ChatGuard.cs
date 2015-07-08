using System.Collections.Generic;
using System.Reflection;
using System;
using System.Data;
using UnityEngine;
using Oxide.Core;
using CodeHatch.Common;
using CodeHatch.Engine.Networking;
using CodeHatch.Networking.Events.Players;

namespace Oxide.Plugins
{
    [Info("Chat Guard", "LaserHydra", "1.0.0", ResourceId = 0)]
    [Description("Censor words from the chat")]
    class ChatGuard : ReignOfKingsPlugin
    {
        void Loaded()
        {
            LoadDefaultConfig();
        }

        protected override void LoadDefaultConfig()
        {
            PrintWarning("Creating a new configuration file.");
            Config.Clear();
            List<string> forbidden = new List<string>() { "fuck", "bitch", "dick", "shit", "cunt", "prick", "nigger", "bollocks", "bollox", "motherfucker", "fucker", "tosser", "gay" };
            Config["ForbiddenWords"] = forbidden;
            SaveConfig();
        }
		
		[ChatCommand("o")]
        private void ooc(Player player, string cmd, string[] args)
        {			
			List<string> ForbiddenWordsList = Config["ForbiddenWords"] as List<string>;
			string censored = "";
			
			//if the player only types /o, we want to tell them how to use it
			if( args.Length < 1 ){
				PrintToChat(player, "Usage: /o your OOC message");				
				return ;
			}
			// first half of the message is the player's name and OOC tag
			string toPrint = player.DisplayName + "[FF0066] (OOC)[FFFFFF]:";	
			// second half of the message is the rest of their entered text
			string chat = "";
			foreach (string word in args)
			{
				if( true )
				{
					chat += " " + word;	
					continue;
				}
			}

			string message = chat;
			foreach (string swear in ForbiddenWordsList)
			{
				for (int i = 0; i < swear.Length; i++)
				{
					censored = censored + "*";
				}

				if (message.ToLower().Contains(swear.ToLower()))
				{
					message = message.ToLower().Replace(swear.ToLower(), censored);
				}				
			}
			// merge the first half and second half, then print!
			//string fin = toPrint + chat;
			string fin = toPrint + message;
			PrintToChat(fin);							
        }
		
        void OnPlayerChat(PlayerEvent e)
        {
            List<string> ForbiddenWordsList = Config["ForbiddenWords"] as List<string>;
            Player player = e.Player;
            string message = e.ToString();
            string censored = "";
            bool isCensored = false;
            if (message.StartsWith("/"))
            {
                return;
            }
            foreach (string word in ForbiddenWordsList)
            {
                censored = "";
                for (int i = 0; i < word.Length; i++)
                {
                    censored = censored + "*";
                }

                if (message.ToLower().Contains(word.ToLower()))
                {
                    message = message.ToLower().Replace(word.ToLower(), censored);
                    isCensored = true;
                }
            }
            if (isCensored)
            {
                BroadcastChat(player.DisplayName, message);
                e.Cancel();
            }
        }

        #region UsefulMethods
        //--------------------------->   Player finding   <---------------------------//
        /*
        BasePlayer GetPlayer(string searchedPlayer, BasePlayer executer, string prefix)
        {
            BasePlayer targetPlayer = null;
            List<string> foundPlayers = new List<string>();
            string searchedLower = searchedPlayer.ToLower();
            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                string display = player.displayName;
                string displayLower = display.ToLower();

                if (!displayLower.Contains(searchedLower))
                {
                    continue;
                }
                if (displayLower.Contains(searchedLower))
                {
                    foundPlayers.Add(display);
                }
            }
            var matchingPlayers = foundPlayers.ToArray();

            if (matchingPlayers.Length == 0)
            {
                SendChatMessage(executer, prefix, "No matching players found!");
            }

            if (matchingPlayers.Length > 1)
            {
                SendChatMessage(executer, prefix, "Multiple players found:");
                string multipleUsers = "";
                foreach (string matchingplayer in matchingPlayers)
                {
                    if (multipleUsers == "")
                    {
                        multipleUsers = "<color=yellow>" + matchingplayer + "</color>";
                        continue;
                    }

                    if (multipleUsers != "")
                    {
                        multipleUsers = multipleUsers + ", " + "<color=yellow>" + matchingplayer + "</color>";
                    }

                }
                SendChatMessage(executer, prefix, multipleUsers);
            }

            if (matchingPlayers.Length == 1)
            {
                targetPlayer = BasePlayer.Find(matchingPlayers[0]);
            }
            return targetPlayer;
        }
        */
        //---------------------------->   Chat Sending   <----------------------------//

        void BroadcastChat(string prefix, string msg)
        {
            PrintToChat("[FF9A00]" + prefix + "[FFFFFF]: " + msg);
        }

        void SendChatMessage(Player player, string prefix, string msg)
        {
            SendReply(player, "[FF9A00]" + prefix + "[FFFFFF]: " + msg);
        }

        //---------------------------------------------------------------------------//
        #endregion
    }
}
