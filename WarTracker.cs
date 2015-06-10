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


namespace Oxide.Plugins
{
    [Info("WarTracker", "Scorpyon", "1.0.5")]
    public class WarTracker : ReignOfKingsPlugin
    {
        //
        // MODIFY THIS VALUE TO THE NUMBER OF 'SECONDS' THAT WARS WILL LAST FOR
        private const int WarTimeLength = 5400; // Currently 1.5 hrs
        // MODIFY THIS VALUE TO THE NUMBER OF 'SECONDS' THAT YOU WANT BETWEEN WAR UPDATE REPORTS TO PLAYERS 
        private const int WarReportInterval = 300; // Currently 5 minutes
        // MODIFY THIS VALUE FOR PREPARATION TIME BEFORE WAR STARTS
        private const int WarPrepTime = 600; // Total prep time in seconds = Currently 10 minutes
        private const int WarPrepTimeHours = 0;         //
        private const int WarPrepTimeMinutes = 10;      // These are for text purposes to save time on calculations later
        private const int WarPrepTimeSeconds = 0;       //




        // DO NOT EDIT ANYTHING BELOW THIS LINE UNLESS YOU WANT TO EXPERIMENT / KNOW WHAT YOU'RE DOING.
        // ==================================================================================================

        private Collection<Collection<string>> WarList = new Collection<Collection<string>>();
        private const int WarTimerInterval = 5;

        // SAVE DATA ===============================================================================================

		private void LoadWarData()
		{
            WarList = Interface.GetMod().DataFileSystem.ReadObject<Collection<Collection<string>>>("SavedWarList");
		}

        private void SaveWarListData()
        {
            Interface.GetMod().DataFileSystem.WriteObject("SavedWarList", WarList);
        }
        
        void Loaded()
        {            
            // Load the WarTimer Updater
            timer.Repeat(WarReportInterval, 0, WarReport);
            timer.Repeat(1, 0, WarUpdate);

            LoadWarData();
		}


        // ===========================================================================================================


        private void WarReport()
        {
            var hours = "";
            var minutes = "";
            var seconds = "";

            if(WarList.Count >= 1)
            {
                // Check each War in the List
                PrintToChat("[0000FF]WAR REPORT");
                for (var i = 0; i < WarList.Count; i++)
                {
                    var timeLeft = Int32.Parse(WarList[i][3]);
                    hours = (timeLeft / 60 / 60).ToString();
                    minutes = ((timeLeft - (Int32.Parse(hours) * 60 * 60))/60).ToString();
                    var intSeconds = timeLeft - (Int32.Parse(hours) * 60 * 60) - (Int32.Parse(minutes) * 60);
                    seconds = intSeconds.ToString();

                    if(timeLeft > WarTimeLength)
                    {
                        var prepTimeLeft = timeLeft  - WarTimeLength;
                        var prepHours = (prepTimeLeft / 60 / 60).ToString();
                        var prepMinutes = ((prepTimeLeft - (Int32.Parse(prepHours) * 60 * 60))/60).ToString();
                        var intPrepSeconds = prepTimeLeft - (Int32.Parse(prepHours) * 60 * 60) - (Int32.Parse(prepMinutes) * 60);
                        var prepSeconds = intPrepSeconds.ToString();

                        PrintToChat("[FF0000]War Report : [00FF00]" + Capitalise(WarList[i][1]) + "[FFFFFF] is preparing for war with [00FF00]" + Capitalise(WarList[i][2]));
                        PrintToChat("[FFFFFF]There are [00FF00]" + prepHours + "[FFFFFF]hrs, [00FF00]" + prepMinutes + "[FFFFFF]mins, [00FF00]" + prepSeconds + "[FFFFFF]secs until this war begins!");
                    }
                    else
                    {
                        PrintToChat("[FF0000]War Report : [00FF00]" + Capitalise(WarList[i][1]) + "[FFFFFF] is at war with [00FF00]" + Capitalise(WarList[i][2]));
                        PrintToChat("[00FF00]" + hours + "[FFFFFF]hrs, [00FF00]" + minutes + "[FFFFFF]mins, [00FF00]" + seconds + "[FFFFFF]secs remaining.");
                    }
                }
            }

            // Save the data
            SaveWarListData();
        }

        private void WarUpdate()
        {
            // Check each War in the List
            foreach(var war in WarList)
            {
                // Countdown the time for this war
                var timeLeft = Int32.Parse(war[3]);

                if(timeLeft == WarTimeLength)
                {
                    PrintToChat("[FF0000]WAR BETWEEN [00FF00]" + Capitalise(war[1]) + "[FF0000] AND " + Capitalise(war[2]) + "[FF0000] HAS BEGUN!");
                }

                timeLeft = timeLeft - 1;

                //Store this value in the War Record
                war[3] = timeLeft.ToString();

                // If war has ended, let everyone know and end the war
                if(timeLeft <= 0)
                {
                    PrintToChat("[FF0000] War Report [FFFFFF]([00FF00]WAR OVER![FFFFFF]) : The war between [00FF00]" + Capitalise(war[1]) + " [FFFFFF] and [00FF00]" + Capitalise(war[2]) + "[FFFFFF] has now ended!");
                    EndWar(war[0]);
                }
            }
        }
        
        // Get current War Report
        [ChatCommand("warreport")]
        private void GetWarReport(Player player, string cmd)
        {
            if(WarList.Count <= 0) PrintToChat("[FF0000] War Report [FFFFFF] : There are currently no active wars. The land is finally at peace once more.");
            WarReport();
        }

        // CHEAT COMMAND for Admins to end all wars
        [ChatCommand("endallwars")]
        private void EndAllWarsOnServer(Player player, string cmd)
        {
            if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only an admin may end all guild wars!!");
                return;
            }
            PrintToChat(player, "Ending all guild wars...");

            // End all Wars in the List
            WarList = new Collection<Collection<string>>();
        }

        // CHEAT COMMAND for end specific wars
        [ChatCommand("endwar")]
        private void EndSpecificWarOnServer(Player player, string cmd, string[] playerToEndWar)
        {
            if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only an admin may end a guild war!!");
                return;
            }

            var playerName = playerToEndWar[0];
            if(playerToEndWar.Length > 1){
                for(var i=1; i<playerToEndWar.Length; i++)
                {
                    playerName = string.Format(playerName + " {0}",playerToEndWar[i]);
                }
            }

            Player targetPlayer = Server.GetPlayerByName(playerName);
            if(targetPlayer == null){
                PrintToChat(player,"That player is currently not on the server.");
            }
            var guildName = PlayerExtensions.GetGuild(targetPlayer).DisplayName;

            //PrintToChat(player, "Ending all wars for guild : [00FF00]" + guildName);

            // End all Wars for this guild
            for(var i=0; i<WarList.Count; i++)
            {
                if(string.Compare(guildName.ToLower(),WarList[i][1].ToLower()) == 0 || string.Compare(guildName.ToLower(),WarList[i][2].ToLower()) == 0)
                {
                    PrintToChat("[FF0000]Ending all Wars for :[00FF00]" + guildName);
                    WarList.RemoveAt(i);
                    i--;
                }
            }
        }

        [ChatCommand("declarewar")]
        private void DeclareWarOnGuild(Player player, string cmd, string[] targetPlayerNameArray)
        {
            //if the player only types /delcarewar, show the instructions on what to do
            if (targetPlayerNameArray.Length < 1)
            {
                PrintToChat(player, "[FF0000]Declare War Instructions[FFFFFF] : Type /declarewar followed by the Player's name to declare war on that player's guild. THIS CANNOT BE UNDONE!");
                return;
            }

            // Convert the player name string array to a string
            var targetPlayerName = targetPlayerNameArray[0];
            if(targetPlayerNameArray.Length > 1)
            {
                for(var i=1; i<targetPlayerNameArray.Length; i++)
                {
                    targetPlayerName = string.Format(targetPlayerName + " {0}", targetPlayerNameArray[i]);
                }
            }

            // Find the chosen target player
            Player targetPlayer = Server.GetPlayerByName(targetPlayerName);

            //Check that this player can be found
            if (targetPlayer == null)
            {
                PrintToChat(player, "[FF0000]War Squire[FFFFFF] : That person is not available. Perhaps they exist only in your imagination my Lord?");
                return;
            }

            // Check they are not trying to declare war on themselves
            if (string.Compare(targetPlayerName.ToLower(), player.DisplayName.ToLower()) == 0) 
            {
                PrintToChat(player, "[FF0000]War Squire[FFFFFF] : You can't declare war upon thyself, my Lord! This is crazy talk!");
                return;
            }

            // Get the player's guild
            if (PlayerExtensions.GetGuild(targetPlayer).DisplayName == null)
            {
                PrintToChat(player, "[FF0000]War Squire[FFFFFF] : My Lord, you have not yet formed a guild. You must do so before you can declare a war!");
                return;
            }
            string playerGuild = PlayerExtensions.GetGuild(targetPlayer).DisplayName;

            // Check they are not in the same guild
            string myGuild = PlayerExtensions.GetGuild(player).DisplayName;
            
            // Remove unneccessary [0] at start of string
            playerGuild = playerGuild.Replace("[0]","");
            myGuild = myGuild .Replace("[0]","");

            if (string.Compare(playerGuild, myGuild) == 0)
            {
                PrintToChat(player, "[FF0000]War Squire[FFFFFF] : My Lord! That is your friend! A trusted Ally! You can't declare war on them! It would get... awkward...!");
                return;
            }

            // Check that they aren't already in a war with this guild
            foreach(var war in WarList)
            {
                if((string.Compare(myGuild,war[1]) == 0 && string.Compare(playerGuild,war[2]) == 0) || (string.Compare(myGuild,war[2]) == 0 && string.Compare(playerGuild,war[1]) == 0))
                {
                    PrintToChat(player, "[FF0000]War Squire[FFFFFF] : We are already at war with that guild, my Lord.");
                     return;
                }                
            }

            // Tell the World that war has been declared!
            string warText = player.DisplayName + "[FFFFFF] ([FF0000]Declaring War[FFFFFF]) : [00FF00]" + Capitalise(myGuild) + " [FFFFFF]has declared war on [00FF00]" + Capitalise(playerGuild) + "[FFFFFF]! They have [00FF00]" + WarPrepTimeHours + "[FFFFFF]hrs, [00FF00]" + WarPrepTimeMinutes + "[FFFFFF]mins and [00FF00]" + WarPrepTimeSeconds + "[FFFFFF]secs to prepare for war!";
            PrintToChat(warText);
            
            // Begin the War!
            CommenceWar(myGuild + playerGuild, myGuild, playerGuild);

            // Save the data
            SaveWarListData();
        }

        private void CommenceWar(string warID, string myGuild, string targetGuild)
        {
            // Add the War Details 
            var newWarInfo = new Collection<string>();
            newWarInfo.Add(warID);
            newWarInfo.Add(myGuild);
            newWarInfo.Add(targetGuild);
            var timeLengthAsString = (WarTimeLength + WarPrepTime).ToString();
            newWarInfo.Add(timeLengthAsString);

            // Add the War to the War List
            WarList.Add(newWarInfo);
        }

        private void EndWar(string warID)
        {
            // Find the War by it's ID string
            for (var i = 0; i < WarList.Count; i++)
            {
                if (string.Compare(warID, WarList[i][0]) == 0)
                {
                    WarList.RemoveAt(i);
                }
            }
        }
		
		
		// Capitalise the Starting letters
		private string Capitalise(string word)
		{
			var finalText = "";
			finalText = Char.ToUpper(word[0]).ToString();
			var spaceFound = 0;
			for(var i=1; i<word.Length;i++)
			{
				if(word[i] == ' ')
				{
					spaceFound = i + 1;
				}
				if(i == spaceFound)
				{
					finalText = finalText + Char.ToUpper(word[i]).ToString();
				}
				else finalText = finalText + word[i].ToString();
			}
			return finalText;
		}
	}
}
