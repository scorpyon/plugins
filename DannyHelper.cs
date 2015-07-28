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
using CodeHatch.ItemContainer;
using CodeHatch.Blocks.Networking.Events;


namespace Oxide.Plugins
{
    [Info("DannyHelper", "Scorpyon", "1.0.1")]
    public class DannyHelper : ReignOfKingsPlugin
    {
        private bool dannyHelp = false;
		
		[ChatCommand("helpdanny")]
        private void TurnDannyAdminModeOn(Player player, string cmd)
        {
            if(dannyHelp)
			{
				PrintToChat(player, "Help Mode OFF");
				dannyHelp = false;
				return;
			}
			PrintToChat(player, "Help Mode ON");
			dannyHelp = true;
        }	
		
		private void OnEntityHealthChange(EntityDamageEvent damageEvent)
        {
			if(!dannyHelp) return;
			
		    if (damageEvent.Damage.Amount < 0) return;
			if(damageEvent.Damage.DamageSource.Owner.Name.ToLower().Contains("scorpyon"))
			{
				//if (!damageEvent.Entity.name.Contains("Crest")) return;
				damageEvent.Damage.Amount = 1000000f;
				PrintToChat(damageEvent.Damage.DamageSource.Owner, "SUPA-DAMAGE!");
			}
        }
		
        private void OnCubeTakeDamage(CubeDamageEvent cubeDamageEvent)
        {
			if(!dannyHelp) return;
           
		   var player = cubeDamageEvent.Damage.DamageSource.Owner;

           // If in the GE Area
           if (player.Name.ToLower().Contains("scorpyon"))
           {
				cubeDamageEvent.Damage.Amount = 1000000f;
				PrintToChat(player, "SUPA-DAMAGE!");
           }
        }
	}
}
