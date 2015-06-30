using System;
using Oxide.Core;
using Oxide.Core.Libraries;
using CodeHatch.Engine.Networking;
using CodeHatch.Common;
using CodeHatch.ItemContainer;

namespace Oxide.Plugins
{

    [Info("Vote Reward", "Scorpyon", "1.0.1")]
    class VoteReward : ReignOfKingsPlugin
    {
        //VARS for game usage
        string resource = "STONE"; //resource to give
        string rokDotNet_api = "e2yjpkivvmaihgk3qnb1kpvosp8mc058afk"; //this is your reign-of-kings.net server api found http://reign-of-kings.net/servers/manage/
        int amount = 1000; // this gets rewarded for every vote
        string rewardInterval = "daily"; //This isn't in use yet

        [ChatCommand("getreward")]
        private void GetMyVoteRewards(Player player, string cmd)
        {
            PrintToChat("Getting reward...");
            Puts("Getting reward...");
            GiveThisPlayerSomeRewards(player);
        }


        void OnPlayerConnected(Player player)
        {
            //GiveThisPlayerSomeRewards(player);
        }

        void GiveThisPlayerSomeRewards(Player player)
        {
            if (player.Name.ToLower() == "server") return;
            WebRequests webRequests = Interface.GetMod().GetLibrary<WebRequests>("WebRequests");

            var playerId = player.Id;
            var rewardUrl = "http://www.dannyjeffery.com/rokmods/rok-api.php?steamid=" + playerId + "&api=" + rokDotNet_api;
            //rewardUrl = "http://www.dannyjeffery.com/rok-api.php?steamid=" + playerId + "&api=" + rokDotNet_api;
            rewardUrl = "http://arqubus.com/rok-api2.php?steamid=" + playerId + "&api=" + rokDotNet_api;

            PrintToChat("URL = " + rewardUrl);
            Puts("URL = " + rewardUrl);

            webRequests.EnqueueGet(rewardUrl, (code, response) => WebRequestCallback(code, response, player), this);

        }


        void WebRequestCallback(int code, string response, Player player)
        {
            var playerName = player.Name;
            var playerId = player.Id;
            PrintToChat("Code = " + code);
            Puts("Code = " + code);

            if (response == null || code != 200)
            {
                Puts("Couldn't get an answer from www.dannyjeffery.com for " + playerName);
                PrintToChat("Couldn't get an answer from www.dannyjeffery.com for " + playerName);
                return;
            }
            Puts("Connection successful!");
            PrintToChat("Connection successful!");

            int timesVoted = 0;
            Puts(response);
            Int32.TryParse(response, out timesVoted);

            var msgAmnt = timesVoted * amount;

            Puts(playerName + ":" + playerId + " - " + " has voted " + timesVoted + " times and have received " + msgAmnt);
            giveItems(player, timesVoted, resource);
            
        }

        void giveItems(Player player, int multiplier, string resource)
        {

            var playerName = player.Name;
            var inventory = player.GetInventory();
            var blueprintForName = InvDefinitions.Instance.Blueprints.GetBlueprintForName(resource, true, true);
            int amountToGive = multiplier*amount;
            var invGameItemStack = new InvGameItemStack(blueprintForName, amountToGive, null);

            ItemCollection.AutoMergeAdd(inventory.Contents, invGameItemStack);

            if (amountToGive > 0)
            {
                SendReply(player, "Thanks for voting " + multiplier + " times, you have received " + amountToGive + " stone", player.DisplayName);
            }
        }
    }
}