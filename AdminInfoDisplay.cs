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
    [Info("AdminInfoDisplay", "Scorpyon", "1.0.1")]
    public class AdminInfoDisplay : ReignOfKingsPlugin
    {
        private Dictionary<string, string> adminList = new Dictionary<string, string>();

        // =================================================================================
        // ADD ADMINS TO THIS LIST AS NEEDED
        private void Loaded()
        {
            adminList.Add("Duke Dan", "[dd2423](Emperor)");

            adminList.Add("Gallahad the Executioner", "[dd2423](High King)");
            adminList.Add("Dragonclaw the Just", "[dd2423](High King)");

            adminList.Add("Lord Xander", "[dd9323](Archduke)");

            adminList.Add("Vicar Jhared", "[239edd](Knight)");
            adminList.Add("Torolf Longclaw", "[239edd](Knight)");
            adminList.Add("Sir Nut Cracker", "[239edd](Knight)");
            adminList.Add("Earl Rolo", "[239edd](Knight)");
            adminList.Add("Jaren van Strien", "[239edd](Knight)");
            adminList.Add("Sir Deadguy", "[239edd](Knight)");
        }

        // =================================================================================

        [ChatCommand("admins")]
        private void ShowAllAdmins(Player player, string cmd)
        {
            var name = "";
            PrintToChat(player, "[FF0000]Duke's Castle: [FFFFFF] The current admin list is as follows: ");

            foreach (var record in adminList)
            {
                PrintToChat(player, GetTitle(record.Key) + " [00FF00]" + record.Key + " " + GetStatus(record.Key));
            }
        }

        private string GetTitle(string name)
        {
            if(adminList.ContainsKey(name))
            {
                return adminList[name];
            }
            return "";
        }

        private string GetStatus(string name)
        {
            Player targetPlayer = Server.GetPlayerByName(name);
            if (targetPlayer != null) return "[FF0000](Online)";
            return "[000000](Offline)";
        }
	}
}
