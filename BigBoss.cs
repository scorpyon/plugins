 using System;
 using CodeHatch.Engine.Networking;
 using CodeHatch.Common;
 using CodeHatch.Networking.Events.Entities;


namespace Oxide.Plugins
{
    [Info("BigBoss", "Scorpyon", "1.0.2")]
    public class BigBoss : ReignOfKingsPlugin
    {
		private const double damageReduction = 10; // Amount reduce damage against the boss. Use 1 - 100 (100 is normal damage taken)
		private const double damageIncrease = 250; // Percentage to increase the boss's damage. 200% is double the normal damage, etc.
		
		
		private Player theBoss;
		
		[ChatCommand("setboss")]
        private void SetTheCurrentRaidBoss(Player player, string cmd, string[] input)
		{
			if (!player.HasPermission("admin")) 
			{
				PrintToChat(player, "You must be an admin to use this command.");
				return;
			}
			
			// Get target's name
            var playerName = ConvertArrayToString(input);
			
			var targetPlayer = Server.GetPlayerByName(playerName);
			if(targetPlayer == null)
			{
				PrintToChat(player, "That player does not appear to be online.");
				return;	
			}
			
			//Set the player to be the boss
			SetTheBoss(targetPlayer);
			PrintToChat("[FF0000]Raid[FFFFFF] : " + targetPlayer.DisplayName + " has been turned into a devastating evil knight by the Gods! Kill them quick!");
        }
		
		
		[ChatCommand("removeboss")]
        private void RemoveTheCurrentRaidBoss(Player player, string cmd, string[] input)
		{
			if (!player.HasPermission("admin")) 
			{
				PrintToChat(player, "You must be an admin to use this command.");
				return;
			}
			
			//Reset the boss variable to null
			theBoss = null;
			PrintToChat("[FF0000]Raid[FFFFFF] : The evil knight has been reduced to a mere mortal once more.");
		}
		
		private void SetTheBoss(Player player)
		{
			theBoss = player;
		}
		
		
		private void OnEntityHealthChange(EntityDamageEvent damageEvent) 
		{
			//var attacker = damageEvent.Damage.DamageSource.Owner;
			var target = damageEvent.Entity.Owner;		
			if(theBoss == null) return;
			
			//Other creatures on the server
			//if(attacker.DisplayName == "Server") return;					

			//Was the boss hurt by a player?
			if(target == theBoss)
			{
				if (damageEvent.Damage.Amount > 0 // taking damage
						&& damageEvent.Entity.IsPlayer // entity taking damage is player
						&& damageEvent.Damage.DamageSource.IsPlayer // entity delivering damage is a player
						&& damageEvent.Entity != damageEvent.Damage.DamageSource // entity taking damage is not taking damage from self
				){
//						PrintToChat(attacker, "[FF0000]Raid[FFFFFF] : Your attacks are doing less damage to this person!");
					double damageTaken =damageEvent.Damage.Amount * (damageReduction/100);
					damageEvent.Damage.Amount = (int)damageTaken;
				}		
			}
			
			// if(attacker == theBoss)
			// {
				// //Did the boss hurt another player's face?
				// if (damageEvent.Damage.Amount > 0 // taking damage
						// && damageEvent.Entity.IsPlayer // entity taking damage is player
						// && damageEvent.Damage.DamageSource.IsPlayer // entity delivering damage is a player
						// && damageEvent.Entity != damageEvent.Damage.DamageSource // entity taking damage is not taking damage from self
				// ){
					// PrintToChat(attacker, "[FF0000]Raid[FFFFFF] : Your foe deals you a devastating! blow!");
					// damageEvent.Damage.Amount = damageEvent.Damage.Amount * (damageIncrease/100);
				// }		
			// }
		}
		
        private string ConvertArrayToString(string[] textArray)
        {
            var newText = textArray[0];
            if (textArray.Length > 1)
            {
                for (var i = 1; i < textArray.Length; i++)
                {
                    newText = newText + " " + textArray[i];
                }
            }
            return newText;
        }
	}
}
