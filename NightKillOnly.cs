using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeHatch.Engine.Networking;
using CodeHatch.Networking.Events.Entities;
using Oxide.Core;

namespace Oxide.Plugins
{
    [Info("Night Killing Only", "CrZy & Scorpyon", "1.1")]
    public class NightKillOnly : ReignOfKingsPlugin
    {
    //    #region Configuration Data
    //    bool configChanged;
    //    bool broadcastedNightMessage = false;
    //    string defaultNightMessage = "It is now Killing Time!";
    //    string nightMessage = "";
    //    string defaultNightOverMessage = "Killing time is over!";
    //    string nightOverMessage = "";
    //    private bool _pvPAllowed = true;

    //    #endregion

    //    void Loaded()
    //    {
    //        LoadConfigData();
    //        timer.Repeat(1, 0, BroadcastNightMessage);
    //        LoadPvPCheck();
    //    }

    //    protected override void LoadDefaultConfig() => Warning("New configuration file created.");
    //    void Warning(string msg) => PrintWarning($"{Title} : {msg}");

    //    [ChatCommand("allowpvp")]
    //    private void LetPlayersPvPAtNight(Player player, string cmd)
    //    {
    //        if (_pvPAllowed)
    //        {
    //            _pvPAllowed = false;
    //            PrintToChat(player,"You have enabled PvP at Night.");
    //        }
    //        else
    //        {
    //            _pvPAllowed = true;
    //            PrintToChat(player,"You have disabled PvP at Night.");
    //        }
    //        SavePvPCheck();
    //    }

    //    private void LoadPvPCheck()
    //    {
    //        _pvPAllowed = Interface.GetMod().DataFileSystem.ReadObject<bool>("SavedPvPAllowance");
    //    }

    //    private void SavePvPCheck()
    //    {
    //        Interface.GetMod().DataFileSystem.WriteObject("SavedPvPAllowance", _pvPAllowed);
    //    }

    //    [ChatCommand("checknight")]
    //    private void CheckTimeBlock(Player player, string cmd, string[] args)
    //    {
    //        string message = "It is currently not night";

    //        if (IsNight())
    //            message = "It is currently night";
            
    //        PrintToChat(player, message);

    //    }

    //    private void OnEntityHealthChange(EntityDamageEvent damageEvent)
    //    {
    //        GameClock.TimeBlock TimeBlock = GameClock.Instance.CurrentTimeBlock;

    //        if (
    //            damageEvent.Damage.Amount > 0 // taking damage
    //            && damageEvent.Entity.IsPlayer // entity taking damage is player
    //            && damageEvent.Damage.DamageSource.IsPlayer // entity delivering damage is a player
    //            && damageEvent.Entity != damageEvent.Damage.DamageSource // entity taking damage is not taking damage from self
    //            && !IsNight() // Not night time
    //            && !PvPAllowed() // Not night time
    //        ) 
    //        {
    //            damageEvent.Cancel("Can Only Kill At Night");
    //            damageEvent.Damage.Amount = 0f; 
    //            PrintToChat(damageEvent.Damage.DamageSource.Owner, "[FF0000]You can only attack other players at night.[FFFFFF]");
    //        }
    //    }

    //    private bool PvPAllowed()
    //    {
    //        return _pvPAllowed;
    //    }

    //    bool IsNight()
    //    {
    //        GameClock.TimeBlock TimeBlock = GameClock.Instance.CurrentTimeBlock;

    //        if (TimeBlock != GameClock.TimeBlock.Dusk && TimeBlock != GameClock.TimeBlock.Night)
    //            return false;

    //        if (TimeBlock == GameClock.TimeBlock.Dusk && GetCurrentHour() < 21)
    //            return false;

    //        return true;
    //    }

    //    int GetCurrentHour()
    //    {
    //        string time = GameClock.Instance.TimeOfDayAsClockString(true);
    //        string[] timePeices = time.Split(new Char [] {':'});

    //        return Convert.ToInt32(timePeices[0]);
    //    }

    //    void BroadcastNightMessage()
    //    {
    //        if (!IsNight() && broadcastedNightMessage)
    //        {
    //            PrintToChat(nightOverMessage);
    //            broadcastedNightMessage = false;
    //            return;
    //        }

    //        if (IsNight() && !broadcastedNightMessage)
    //        {
    //            PrintToChat(nightMessage);
    //            broadcastedNightMessage = true;
    //        }
    //    }

    //    void LoadConfigData()
    //    {
    //        nightMessage = GetConfigValue("Messages", "NightStart", defaultNightMessage);
    //        nightOverMessage = GetConfigValue("Messages", "NightEnd", defaultNightOverMessage);

    //        if (!configChanged) return;
    //        Warning("The configuration file was updated!");
    //        SaveConfig();
    //    }

    //    private T GetConfigValue<T>(string category, string setting, T defaultValue)
    //    {
    //        var data = Config[category] as Dictionary<string, object>;
    //        object value;

    //        if (data == null)
    //        {
    //            data = new Dictionary<string, object>();
    //            Config[category] = data;
    //            configChanged = true;
    //        }

    //        if (data.TryGetValue(setting, out value)) 
    //            return (T)Convert.ChangeType(value, typeof(T));

    //        value = defaultValue;
    //        data[setting] = value;
    //        configChanged = true;

    //        return (T)Convert.ChangeType(value, typeof(T));
    //    }

    }
}