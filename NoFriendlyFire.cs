using CodeHatch.Networking.Events.Entities;
using CodeHatch.Common;

namespace Oxide.Plugins
{
    [Info("No Friendly Fire", "CrZy", "0.1")]
    public class NoFriendlyFire : ReignOfKingsPlugin
    {
        //private void OnEntityHealthChange(EntityDamageEvent damageEvent)
        //{
        //    if (
        //        damageEvent.Damage.Amount > 0 // taking damage
        //        && damageEvent.Entity.IsPlayer // entity taking damage is player
        //        && damageEvent.Damage.DamageSource.IsPlayer // entity delivering damage is a player
        //        && damageEvent.Entity != damageEvent.Damage.DamageSource // entity taking damage is not taking damage from self
        //        && damageEvent.Entity.Owner.GetGuild().DisplayName == damageEvent.Damage.DamageSource.Owner.GetGuild().DisplayName // both entities are in the same guild
        //    ) 
        //    {
        //        damageEvent.Cancel("No Friendly Fire");
        //        damageEvent.Damage.Amount = 0f;  
        //    }
        //}
    }
}