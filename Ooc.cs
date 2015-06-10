using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using CodeHatch.Build;
using CodeHatch.Engine.Networking;
using CodeHatch.Common;

namespace Oxide.Plugins
{
    [Info("Ooc", "DumbleDora", "0.1")]
    public class Ooc : ReignOfKingsPlugin {	
	// Ooc Reign of Kings mod by DumbleDora
	// this plugin allows players to use '/o message' to add an OOC tag (for RP chat/servers)
	

		void Loaded()
        {            
			cmd.AddChatCommand("o", this, "ooc");
		}
		
								[ChatCommand("o")]
        private void ooc(Player player, string cmd, string[] args)
        {			
					//if the player only types /o, we want to tell them how to use it
					if( args.Length < 1 ){
						PrintToChat(player, "Usage: /o your OOC message");				
						return ;
					}
					// first half of the message is the player's name and OOC tag
					string toPrint = player.DisplayName + "[3f3f3f] (OOC)[FFFFFF]:";	
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
					// merge the first half and second half, then print!
					string fin = toPrint + chat;
					PrintToChat(fin);							
        }
    }
}
