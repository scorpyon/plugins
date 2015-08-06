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
		
		[ChatCommand("superman")]
        private void TurnDannyAdminModeOn(Player player, string cmd)
        {
			var adminName = player.Name.ToLower();
			if(adminName == "lord scorpyon" || adminName == "ultrunz von dicksby" || adminName == "duke dan" || adminName == "Odin")
			{
				if(dannyHelp)
				{
					PrintToChat(player, "Super-Strength Mode OFF");
					dannyHelp = false;
					return; 
				}
				PrintToChat(player, "Super-Strength Mode ON");
				dannyHelp = true;
			}
        }	
		
		private void OnEntityHealthChange(EntityDamageEvent damageEvent)
        {
			if(!dannyHelp) return;
			
		    if (damageEvent.Damage.Amount < 0) return;
			var thisName = damageEvent.Damage.DamageSource.Owner.Name.ToLower();
			if(thisName == "lord scorpyon" || thisName == "ultrunz von dicksby" || thisName == "duke dan" || thisName == "Odin")
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
			var thisName = player.Name.ToLower();
			if(thisName == "lord scorpyon" || thisName == "ultrunz von dicksby" || thisName == "duke dan")
			{
				cubeDamageEvent.Damage.Amount = 1000000f;
				PrintToChat(player, "SUPA-DAMAGE!");
           }
        }
	}
}
