using CodeHatch;
using CodeHatch.Common;
using CodeHatch.Engine.Modules.SocialSystem;
using CodeHatch.Engine.Modules.SocialSystem.Objects;
using CodeHatch.Engine.Networking;
using CodeHatch.Inventory.Blueprints;
using CodeHatch.Engine.Events.Prefab;
using CodeHatch.Thrones.SocialSystem;

namespace Oxide.Plugins
{
    [Info("MultiCrest", "Scorpyon", "1.0.0")]
    public class MultiCrest : ReignOfKingsPlugin
    {

        private void OnObjectDeploy(NetworkInstantiateEvent e)
        {
            Player player = Server.GetPlayerById(e.SenderId);
            if (!player.HasPermission("admin"))
            {
                return;
            }
            if (player == null) return;
            InvItemBlueprint bp = InvDefinitions.Instance.Blueprints.GetBlueprintForID(e.BlueprintId);
            PrintToChat(player, "You have placed a " + bp.Name + ".");
            if (bp.Name.Contains("Crest"))
            {
                MessWithTheCrest(e, player);
            }
        }
        
        private void MessWithTheCrest(NetworkInstantiateEvent e, Player player)
        {
            PrintToChat(player, "Resetting the Crest Count");
            PrintToChat("Instantiation args : " + e.ObjectGUID);
            
            CrestScheme crestScheme = SocialAPI.Get<CrestScheme>();
            if(crestScheme == null) PrintToChat("crestScheme was null");
            if (crestScheme.GetCrest(player.Id) == null) PrintToChat("GetCrest was null");

            //PrintToChat(crestScheme.GetCrest(player.Id).ToString());
            //crestScheme.UnregisterObject(e.ObjectGUID,e.Position);
            //crestScheme.GetCrest(player.Id).ObjectGUID = 0;
            //crestScheme.RemoveGroup(player.GetGuild().BaseID);

            //PrintToChat("GuidManager = " + crestScheme.HasCrest(player.Id));
            //var crests = GUIDManager.TryGetObject<Crests>(4);
            //if (crests != null)
            //{
            //    PrintToChat("Found Crest: " + crests.ToString());
            //    crests.RemoveCrest(player.Id);
            //}
            //else PrintToChat("Couldn't find this crest");
        }
    }
}
