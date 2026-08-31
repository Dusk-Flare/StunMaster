using BepInEx;
using BepInEx.Logging;
using ImprovedInput;
using MonoMod.RuntimeDetour;
using StunMaster.Config;
using StunMaster.Hooks;
using StunMaster.StunCat;
using StunMaster.StunCompat;
using System;
using System.Reflection;
using System.Security.Permissions;
using UnityEngine;


#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace StunMaster;

public static class PluginData
{
    public const string stunMasterID = "stunmaster";
    public const string GUID = "com.lanta.stunmaster";
    public const string ModName = "Stun Master";
    public const string Version = "0.1.9";
}

[BepInPlugin(PluginData.GUID, PluginData.ModName, PluginData.Version)]
sealed class Plugin : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    public static readonly PlayerKeybind stunTrigger = PlayerKeybind.Register("stun:bind", PluginData.ModName, "StunAbility", KeyCode.C, KeyCode.JoystickButton5);
    public static AbstractPhysicalObject.AbstractObjectType stunSpear = new("StunSpear", true);
    public static ReturnData OnlineData;
    public static OptionsMenu OIinstance = new();
    bool _init;

    public void OnEnable()
    {
        Logger = base.Logger;
        On.RainWorld.OnModsInit += OnModsInit;
        HookManager.Apply();
    }

    public void OnDisable()
    {
        On.RainWorld.OnModsInit -= OnModsInit;
        HookManager.Remove();
    }

    public void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (_init) return;
        MachineConnector.SetRegisteredOI(PluginData.stunMasterID, OIinstance);
        if (CompatibilityAccess.IsMeadowLoaded) AddMeadowHooks();
        Logger.LogInfo(CompatibilityAccess.IsMeadowLoaded);
        LoadResources(self);
        _init = true;
    }

    public void LoadResources(RainWorld rainWorld)
    {
        Lanta.stunForehead ??= Futile.atlasManager.LoadAtlas("atlases/stunmaster_face");
        Lanta.stunArm ??= Futile.atlasManager.LoadAtlas("atlases/stunmaster_arm");
        Lanta.stunLegs ??= Futile.atlasManager.LoadAtlas("atlases/stunmaster_legs");
    }

    public void AddMeadowHooks()
    {
        new Hook(typeof(RainMeadow.Lobby).GetMethod("ActivateImpl", BindingFlags.NonPublic | BindingFlags.Instance), (Action<RainMeadow.Lobby> orig, RainMeadow.Lobby self) =>
        {
            orig(self);
            try
            {
                IRainMeadow.PassOnlineData();
                OnlineData = IRainMeadow.GetOnlineData();
            }
            catch (Exception except)
            {
                RainMeadow.RainMeadow.Error(except);
            }
        });
    }
}
