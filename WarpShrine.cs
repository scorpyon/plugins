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
            
            //timer.Repeat(1, 0, timerMethod);
        }

        private void timerMethod()
        {
            var target = Server.GetPlayerByName("scorpyon");
            if (target != null) GetCurrentLocation(target, "");
        }
        // ===========================================================================================================

        #endregion

        #region SERVER VARIABLES (DO NOT MODIFY)

        // ID = Location ID; float[] coordinates (x, z)
        private Dictionary<string, float[]> _warpList = new Dictionary<string, float[]>();

        #endregion

        #region PLAYER COMMANDS

        // Add current location to the warp list
        [ChatCommand("platform")]
        private void AddPlatform(Player player, string cmd, string[] input)
        {
            CreateNewPlatform(player, 5,5);
        }

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
        [ChatCommand("telereset")]
        private void GoTobase(Player player, string cmd)
        {
            var newPos = new Vector3(200, 50, 0);
            EventManager.CallEvent((BaseEvent)new TeleportEvent(player.Entity, newPos));
        }

        // Get current location
        [ChatCommand("warp")]
        private void CommenceWarpSpeed(Player player, string cmd)
        {
            WarpPlayerToShrine(player, cmd);
        }

        #endregion

#region PRIVATE METHODS

        void CreateNewPlatform(Player player, int width, int length)
        {
            byte material = 2;
            Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            byte prefabId = 0;

            Vector3Int playerPos = (Vector3Int)player.Entity.Position;

            int setX = playerPos.x - (width / 2);
            int setY = playerPos.y;
            int setZ = playerPos.z - (length / 2);

            for (var i = 0; i < width; i++)
            {
                for (var ii = 0; ii < length; ii++)
                {
                    var newPosition = new Vector3Int((int)(setX - (double)(setX / 6)), (int)(setY - (double)(setY / 6) - 1), (int)(setZ - (double)(setZ / 6)));
                    var cubeEvent = new CubePlaceEvent(0, newPosition, material, rotation, prefabId, 0.0f);
                    EventManager.CallEvent((BaseEvent)cubeEvent);
                    setX++;
                }
                setX = playerPos.x - (width / 2);
                setZ++;
            }

            // Tele the player over the platform (to prevent getting stuck
            EventManager.CallEvent((BaseEvent)new TeleportEvent(player.Entity, new Vector3(player.Entity.Position.x, player.Entity.Position.y + 1, player.Entity.Position.z)));
        }

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
            PrintToChat(player, "The warp location has been added to the lisp of warp areas and a shrine area has been created.");
            CreateNewPlatform(player, 5, 5);

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
