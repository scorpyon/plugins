using System;
using System.Collections.Generic;
using CodeHatch.Build;
using CodeHatch.Engine.Networking;
using CodeHatch.Networking.Events.Entities;
using CodeHatch.Networking.Events.Players;
using CodeHatch.Engine.Core.Cache;
using UnityEngine;
using CodeHatch.Blocks.Networking.Events;
using LitJson;
using CodeHatch.Networking.Events;

namespace Oxide.Plugins
{
    //Need a Script?
    //Patrick Shearon aka Hawthorne
    //www.theregulators.org
    //pat.shearon@gmail.com
    //05-21-2015
    [Info("SafeZones", "Hawthorne", "1.2")]
    public class SafeZones : ReignOfKingsPlugin
    {
        // public class MarkClass
        // {
            // public int x { get; set; }
            // public int z { get; set; }
            // public float radius { get; set; }
            // public bool noAttack { get; set; }
            // public MarkClass(int x, int z, float radius, bool noAttack)
            // {
                // this.x = x;
                // this.z = z;
                // this.radius = radius;
                // this.noAttack = noAttack;
            // }

        // }        
        // static List<MarkClass> marks = new List<MarkClass>();
        // #region Helpers
        
        // void Log(string msg) => Puts($"{Title} : {msg}");
        // void Warning(string msg) => PrintWarning($"{Title} : {msg}");

        // void Loaded()
        // {     
// return;		
            // EventManager.Subscribe<PlayerCaptureEvent>(new EventSubscriber<PlayerCaptureEvent>(OnPlayerCapture));            
            // EventManager.Subscribe<PlayerCaptureConfirmEvent>(new EventSubscriber<PlayerCaptureConfirmEvent>(OnPlayerCaptureConfirm));            
            // List<object> data =  GetConfigValue("MarkSettings", "Marks", marks) as List<object>;     
            // marks = new List<MarkClass>();
            // foreach(object o in data)
            // {
                // Dictionary<string, object> d = (Dictionary<string, object>)o;               

                // foreach(var pair in d)
                // {
                    // var key = pair.Key;
                    // var value = pair.Value;
                    // Warning(key + " " + value);
                // }
                // marks.Add(new MarkClass(Convert.ToInt32(d["x"]),Convert.ToInt32(d["z"]),Convert.ToSingle(d["radius"]),Convert.ToBoolean(d["noAttack"])));
            // }
            // Warning( marks.Count + " safe zone markers loaded.");          
        // }


        // protected override void LoadDefaultConfig()
        // {            
            // Warning("New mark configuration file created.");
        // }
    
        // private void SaveMarkData()
        // {
            // SaveConfigValue("MarkSettings", "Marks", marks);
            // Warning("Saving marks.");
            // SaveConfig();
        // }
        // void SaveConfigValue(string category, string setting, object value)
        // {
            // var data = Config[category] as Dictionary<string, object>;                
            // data[setting] = value;
        // }

        // object GetConfigValue(string category, string setting, object defaultValue)
        // {
            // var data = Config[category] as Dictionary<string, object>;
            // object value;
            // if (data == null)
            // {
                // data = new Dictionary<string, object>();
                // Config[category] = data;
            // }
            // if (!data.TryGetValue(setting, out value))
            // {
                // value = defaultValue;
                // data[setting] = value;
            // }
            
            // return value;
        // }

        // private void ShowUsage(Player player)
        // {
            // PrintToChat(player, "[FF9900]Safe Zones by Hawthorne (www.theregulators.org).");
            // PrintToChat(player, "This script will allow you to mark an area where players and objects can't be attacked.");
            // PrintToChat(player, "USAGE:");
            // PrintToChat(player, "mark add radius - [CCCCCC]adds a mark at your loaction for the radius you define.");
            // PrintToChat(player, "mark list - [CCCCCC]shows all marks.");
            // PrintToChat(player, "mark remove markid - [CCCCCC]removes a mark by id as shown in the mark list.");
        // }
        // bool HasPermission(Player player, string perm = null)
        // {
            // var user =  Server.Permissions.GetUser(player.Name);
            // return user != null && user.HasGroup(perm);
        // }
        // #endregion
 
        // [ChatCommand("mark")]
        // private void MarkCommand(Player player, string cmd, string[] args)
        // {
            // if(!Server.Permissions.GetUser(player.Name).HasGroup("admin"))            
                // return;
            // if (args.Length < 1)
            // {
                // ShowUsage(player);
                // return;
            // }
            // switch (args[0].ToLower())
            // {
                // case "remove":
                    // {
                        // if (args.Length < 2)
                        // {
                            // ShowUsage(player);
                            // return;
                        // }
                        // marks.RemoveAt(Convert.ToInt32(args[1]));
                        // SaveMarkData();
                    // }
                    // break;
                // case "add":
                    // {
                        // marks.Add(new MarkClass((int)player.Entity.Position.x,(int)player.Entity.Position.z,Convert.ToSingle(args[1]),true));
                        // SaveMarkData();
                        // PrintToChat(player, "[00FF00]Mark Added.");
                    // }
                    // break;
                // case "list":
                    // {
                        // int x=0;
                        // foreach (MarkClass mc in marks)
                        // {
                            // PrintToChat(player, string.Format("{3}) X: {0} Z: {1} RADIUS: {2}", mc.x,mc.z,mc.radius,x));
                            // x++;
                        // }
                    // }
                    // break;
            // }

           
        // }
        

        // [ChatCommand("loc")]
        // private void LocationCommand(Player player, string cmd, string[] args)
        // {
            // PrintToChat(player, string.Format("Current Location: x:{0} y:{1} z:{2}", player.Entity.Position.x.ToString(), player.Entity.Position.y.ToString(), player.Entity.Position.z.ToString()));
        // }
        // private void OnCubeTakeDamage(CubeDamageEvent cubeDamageEvent)
        // {
            // if (cubeDamageEvent.Entity.IsPlayer & (cubeDamageEvent.Entity != cubeDamageEvent.Damage.DamageSource) & cubeDamageEvent.Damage.DamageSource.IsPlayer)
            // {
                // foreach (MarkClass mc in marks)
                // {
                    // float distance = Math.Abs(Vector2.Distance(new Vector2(mc.x,mc.z), new Vector2(cubeDamageEvent.Entity.Position.x, cubeDamageEvent.Entity.Position.z)));
                    // if (distance <= mc.radius & mc.noAttack)
                    // {
                        // cubeDamageEvent.Cancel("No attack area");
                        // cubeDamageEvent.Damage.Amount = 0f;
                        // PrintToChat(cubeDamageEvent.Damage.DamageSource.Owner, "[FF0000]You can't attack an object in this area.");
                    // }
                // }
            // }
        // }
        // private void OnEntityHealthChange(EntityDamageEvent damageEvent)
        // {
            // if (damageEvent.Entity.IsPlayer & (damageEvent.Entity != damageEvent.Damage.DamageSource) & damageEvent.Damage.DamageSource.IsPlayer)
            // {
                // foreach (MarkClass mc in marks)
                // {                    
                    // float distance = Math.Abs(Vector2.Distance(new Vector2(mc.x,mc.z), new Vector2(damageEvent.Entity.Position.x, damageEvent.Entity.Position.z)));                    
                    // if (distance <= mc.radius & mc.noAttack)
                    // {
                        // damageEvent.Cancel("No attack area");
                        // damageEvent.Damage.Amount = 0f;                        
                        // PrintToChat(damageEvent.Damage.DamageSource.Owner, "[FF0000]You can't attack a person in this area.");
                    // }
                // }
            // }
        // }
        // private void OnPlayerCaptureConfirm(PlayerCaptureConfirmEvent theEvent)
        // {
            // foreach (MarkClass mc in marks)
            // {                    
                // float distance = Math.Abs(Vector2.Distance(new Vector2(mc.x,mc.z), new Vector2(theEvent.TargetEntity.Position.x, theEvent.TargetEntity.Position.z)));                    
                // if (distance <= mc.radius & mc.noAttack)
                // {
                    // theEvent.Cancel("No attack area");        
                    // theEvent.TargetEntity = null;
                    // theEvent.Target = null;

                // }
            // }
        // }

        // private void OnPlayerCapture(PlayerCaptureEvent theEvent)
        // {
            // if (theEvent.Sender.IsClient)
            // {
                // foreach (MarkClass mc in marks)
                // {                    
                    // float distance = Math.Abs(Vector2.Distance(new Vector2(mc.x,mc.z), new Vector2(theEvent.Sender.Entity.Position.x, theEvent.Sender.Entity.Position.z)));                    
                    // if (distance <= mc.radius & mc.noAttack)
                    // {
                        // theEvent.Cancel("No attack area");        
                        // theEvent.TargetEntity = null;
                        // theEvent.Target = null;
                        // PrintToChat(theEvent.Sender, "[FF0000]You can't capture a person in this area.");
                    // }
                // }
            // }
       
        // }   

        

     }
}