using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeHatch.Engine.Networking;
using CodeHatch.Common;
using CodeHatch.Inventory.Blueprints;
using Oxide.Core;
using CodeHatch.Networking.Events.Entities;
using CodeHatch.ItemContainer;
using CodeHatch.UserInterface.Dialogues;
using CodeHatch.Engine.Events.Prefab;
using CodeHatch.Blocks.Networking.Events;

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
            _warpList = Interface.GetMod().DataFileSystem.ReadObject<Dictionary<int, double[]>>("SavedWarpLocations");
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

        // ID = Location ID; Double[] coordinates (x, z)
        private Dictionary<int, double[]> _warpList = new Dictionary<int, double[]>();

        #endregion

        #region PLAYER COMMANDS

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

        #region LOCATION

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
            PrintToChat(player.Id.ToString()); 
 //           player.Entity.Position = new Vector(player.Entity.Position.x + 15, player.Entity.Position.y, player.Entity.Position.z);
//            player.Entity.Position.x = player.Entity.Position.x + 15;
        }

        #endregion

#endregion
    }
}
