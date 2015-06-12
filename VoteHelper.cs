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
using Oxide.Core.Libraries;
using CodeHatch.Networking.Events.Entities;
using CodeHatch.Networking.Events.Entities.Players;
using CodeHatch.Networking.Events.Players;
using CodeHatch.ItemContainer;


namespace Oxide.Plugins
{
    [Info("VoteHelper", "Scorpyon", "1.0.1")]
    public class VoteHelper : ReignOfKingsPlugin
    {
		private readonly WebRequests webRequests = Interface.GetMod().GetLibrary<WebRequests>("WebRequests");
		void Log(string msg) => Puts($"{Title} : {msg}");
		
		
        [ChatCommand("vote")]
        private void ExamplePostRequest(Player player, string command, string[] args)
        {
            webRequests.EnqueuePost("http://reign-of-kings.net/server/319/vote/confirm/", "", (code, response) => WebRequestCallback(code, response, player), this);
			//webRequests.EnqueueGet("http://reign-of-kings.net/server/319/vote/confirm/", (code, response) => WebRequestCallback(code, response, player), this);
        }

        private void WebRequestCallback(int code, string response, Player player)
        {
            if (response == null || code != 200)
            {
                Log("Couldn't get an answer from Google for " + player.Name);
                return;
            }
            Log("Google answered for " + player.Name);
        }
	}
}