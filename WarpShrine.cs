using System.Collections.Generic;
using CodeHatch.Engine.Networking;
using CodeHatch.Common;
using CodeHatch.Networking.Events;
using CodeHatch.Networking.Events.Entities;
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

            // Save the data
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
        [ChatCommand("addlocation")]
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
        private void CommenceWarpSpeed(Player player, string cmd, string[] input)
        {
            WarpPlayerToShrine(player, cmd, input);
        }

        #endregion

#region PRIVATE METHODS

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

            var locName = input.JoinToString(" ");

            if (_warpList.ContainsKey(locName.ToLower()))
            {
                PrintToChat(player, "A warp location with that name already exists!");
                return;
            }

            _warpList.Add(locName, new float[] { player.Entity.Position.x, player.Entity.Position.y, player.Entity.Position.z });
            PrintToChat(player, "The warp location has been added to the lisp of warp areas.");
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

        private void WarpPlayerToShrine(Player player, string cmd, string[] input)
        {
            var locName = input.JoinToString(" ");
            if (!_warpList.ContainsKey(locName)) return;

            var locCoords = _warpList[locName];
            var posX = locCoords[0];
            var posY = locCoords[1];
            var posZ = locCoords[2];

            var newPos = new Vector3(posX,posY,posZ);
            EventManager.CallEvent((BaseEvent)new TeleportEvent(player.Entity, newPos));

        }

        #endregion

#endregion
    }
}
