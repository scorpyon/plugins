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


        void OnPlayerConnected(Player player)
        {
            if (player.Name.ToLower() == "server") return;
            WebRequests webRequests = Interface.GetMod().GetLibrary<WebRequests>("WebRequests");
            
            var playerId = player.Id;

            timer.Once(30, () => webRequests.EnqueueGet("http://cyberscene.co.za/rok-api.php?steamid=" + playerId + "&api=" + rokDotNet_api, (code, response) => WebRequestCallback(code, response, player), this));

        }
        void WebRequestCallback(int code, string response, Player player)
        {
            var playerName = player.Name;
            var playerId = player.Id;

            if (response == null || code != 200)
            {
                Puts("Couldn't get an answer from Cyberscene for " + playerName);
                return;
            }

            int timesVoted = 0;

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