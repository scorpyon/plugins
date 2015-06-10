
using System;
using System.Collections.Generic;
using CodeHatch.Build;
using CodeHatch.Engine.Networking;
using CodeHatch.Networking.Events.Entities;
using CodeHatch.Networking.Events.Players;
using CodeHatch.Engine.Core.Cache;
using UnityEngine;

namespace Oxide.Plugins
{
    //Need a Script?
    //Patrick Shearon aka Hawthorne
    //www.theregulators.org
    //pat.shearon@gmail.com
    //05-21-2015
    [Info("No Attack", "Hawthorne", "1.0.")]
    public class KilledBy : ReignOfKingsPlugin
    {
        private void OnEntityDeath(EntityDeathEvent deathEvent)
        {
            if (deathEvent.Entity.IsPlayer)
            {
                string killer = "[00FF00]" + deathEvent.KillingDamage.DamageSource.Owner.DisplayName;
                string message = "[FFFFFF]SERVER: [FF0000]" + deathEvent.Entity.Owner.DisplayName + " [FFFFFF]was killed by " + killer;
                PrintToChat(message);
            }
        }
    }
}
