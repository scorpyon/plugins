using System;
using System.Collections.Generic;
using CodeHatch.Blocks.Networking.Events;
using CodeHatch.Engine.Networking;
using CodeHatch.Common;
using CodeHatch.Networking.Events;
using CodeHatch.Networking.Events.Entities;
using CodeHatch.UserInterface.Dialogues;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Warp Shrine", "Scorpyon", "1.0.1")]
    public class WarpShrine : ReignOfKingsPlugin
    {
        #region SERVER VARIABLES (MODIFIABLE)
        

        #endregion

        #region Save and Load Data Methods

        // SAVE DATA ===============================================================================================
        private void LoadWarpData()
        {
            _warpList = Interface.GetMod().DataFileSystem.ReadObject<Dictionary<string, float[]>>("SavedWarpLocations");
        }

        private void SaveWarpData()
        {
            Interface.GetMod().DataFileSystem.WriteObject("SavedWarpLocations", _warpList);
        }



        void Loaded()
        {
            LoadWarpData();

            SaveWarpData();
        }
        // ===========================================================================================================

        #endregion

        #region SERVER VARIABLES (DO NOT MODIFY)

        // ID = Location ID; float[] coordinates (x, z)
        private Dictionary<string, float[]> _warpList = new Dictionary<string, float[]>();

        #endregion

        #region PLAYER COMMANDS

        // Add current location to the warp list
        [ChatCommand("addwarplocation")]
        private void AddMyLocation(Player player, string cmd, string[] input)
        {
            AddThisLocationToWarpList(player, cmd, input);
        }

        // Get current location
        [ChatCommand("location")]
        private void GetMyLocation(Player player, string cmd)
        {
            GetCurrentLocation(player, cmd);
        }

        // Get current location
        [ChatCommand("warp")]
        private void CommenceWarpSpeed(Player player, string cmd)
        {
            WarpPlayerToShrine(player, cmd);
        }

        #endregion

#region PRIVATE METHODS

        //void CreateNewPlatform(Player player/*, string cmd, int[] args*/)
        //{
        //    PrintToChat("Starting to create a platform...");
        //    byte material = 7;
        //    PrintToChat("Material to use: Wood Block");
        //    Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        //    byte prefabId = 0;

        //    int setX = 0;
        //    int setY = 180;
        //    int setZ = 0;
        //    Vector3 position = new Vector3(setX, setY, setZ);
        //    PrintToChat("Start Position: " + setX + ", " + setY + ", " + setZ);

        //    int platformLength = 10;//args[0];
        //    int platformWidth = 5;//args[1];

        //    PrintToChat("We will now create a platform " + platformLength + " x " + platformWidth);

        //    for (var i = 0; i < platformWidth; i++)
        //    {
        //        for (var ii = 0; ii < platformLength; ii++)
        //        {
        //            var newPosition = new Vector3Int(1,2,3);
        //            EventManager.CallEvent((BaseEvent)new CubePlaceEvent(0, newPosition, material, rotation, prefabId, 0.0f));
        //            PrintToChat("Placing block at: " + position);
        //            setX += 1;
        //            Vector3 temp = new Vector3(setX, setY, setZ);
        //            position = temp;
        //        }
        //        setZ += 1;
        //    }
        //    PrintToChat("Done!");
        //}

        #region LOCATION

        private void AddThisLocationToWarpList(Player player, string cmd, string[] input)
        {
            if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "Only admins can add warp locations.");
                return;
            }

            if (input.Length < 1)
            {
                PrintToChat(player, "Usage: /addlocation <Name>");
                return;
            }

            var locName = input.JoinToString(" ").ToLower();

            if (_warpList.ContainsKey(locName.ToLower()))
            {
                PrintToChat(player, "A warp location with that name already exists!");
                return;
            }

            _warpList.Add(locName, new float[] { player.Entity.Position.x, player.Entity.Position.y, player.Entity.Position.z });
            PrintToChat(player, "The warp location has been added to the lisp of warp areas.");

            SaveWarpData();
        }

        private void GetCurrentLocation(Player player, string cmd)
        {
            if (!player.HasPermission("admin"))
            {
                PrintToChat(player, "For now, only admins can check locations.");
                return;
            }
            PrintToChat(player, string.Format("Current Location: x:{0} y:{1} z:{2}", player.Entity.Position.x.ToString(), player.Entity.Position.y.ToString(), player.Entity.Position.z.ToString()));
        }


        private void WarpPlayerToShrine(Player player, string cmd)
        {
            if (_warpList.Count < 1)
            {
                PrintToChat(player,"There are no warp destinations currently set.");
                return;
            }

            if (!InWarpShrineLocation(player))
            {
                PrintToChat("You are not currently at a Warp Shrine.");
                return;
            }

            var message = "Current available warp destinations:\n";

            // Open a popup with the resource details
            foreach (var warp in _warpList)
            {
                message = message + Capitalise(warp.Key) + "\n";
            }
            message = message + "\n\n Where would you like to travel to?";

            player.ShowInputPopup("Warp Shrine", message, "", "Submit", "Cancel", (options, dialogue1, data) => WarpPlayerToShrineConfirm(player, options, dialogue1, data));
        }

        private bool InWarpShrineLocation(Player player)
        {
            if (_warpList.Count < 1) return false;

            foreach (var warp in _warpList)
            {
                var posX = warp.Value[0];
                var posY = warp.Value[1];
                var posZ = warp.Value[2];

                var playerX = player.Entity.Position.x;
                var playerY = player.Entity.Position.y;
                var playerZ = player.Entity.Position.z;

                if (playerX > posX - 2 && playerX < posX + 2)
                {
                    if (playerY > posY - 2 && playerY < posY + 2)
                    {
                        if (playerZ > posZ - 2 && playerZ < posZ + 2) return true;
                    }
                }
            }

            return false;
        }


        private void WarpPlayerToShrineConfirm(Player player, Options selection, Dialogue dialogue, object contextData)
        {

            if (selection == Options.Cancel)
            {
                //Leave
                return;
            }

            var locName = dialogue.ValueMessage.ToLower();
            if (!_warpList.ContainsKey(locName))
            {
                return;
            }

            var locCoords = _warpList[locName];
            var posX = locCoords[0];
            var posY = locCoords[1];
            var posZ = locCoords[2];

            var newPos = new Vector3(posX,posY + 1,posZ);
            EventManager.CallEvent((BaseEvent)new TeleportEvent(player.Entity, newPos));
//            EventManager.CallEvent((BaseEvent)new TeleportEvent(player.Entity, Lerp(player.Entity.Position, newPos)));

        }

        #endregion


        #region UTILITY METHODS
        // Capitalise the Starting letters
        private string Capitalise(string word)
        {
            var finalText = "";
            finalText = Char.ToUpper(word[0]).ToString();
            var spaceFound = 0;
            for (var i = 1; i < word.Length; i++)
            {
                if (word[i] == ' ')
                {
                    spaceFound = i + 1;
                }
                if (i == spaceFound)
                {
                    finalText = finalText + Char.ToUpper(word[i]).ToString();
                }
                else finalText = finalText + word[i].ToString();
            }
            return finalText;
        }
        #endregion
#endregion
    }
}
