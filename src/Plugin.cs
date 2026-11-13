using BepInEx;
using BepInEx.Logging;
using ImprovedInput;
using MonoMod.RuntimeDetour;
using StunMaster.Config;
using StunMaster.EngeneerDrone;
using StunMaster.Hooks;
using StunMaster.StunCat;
using StunMaster.StunCompat;
using StunMaster.StunLogic;
using StunMaster.WorldEdit;
using StunMaster.WorldEdit.Creatures;
using System;
using System.Reflection;
using System.Security.Permissions;
using UnityEngine;


#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace StunMaster;

[BepInPlugin(PluginData.GUID, PluginData.ModName, PluginData.Version)]
sealed class Plugin : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    public static readonly PlayerKeybind stunTrigger = PlayerKeybind.Register("stun:bind", PluginData.ModName, "StunAbility", KeyCode.C, KeyCode.JoystickButton5);
    public static Conversation.ID stunConvo = new("Stun_Greet", true);
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
        RegisterEffect.UnregisterEnum();
        Enums.UnRegister();
    }

    public void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (_init) return;
        StunDialog.Register();
        RegisterEffect.RegisterEnum();
        Enums.Register();
        MachineConnector.SetRegisteredOI(PluginData.stunMasterID, OIinstance);
        if (CompatibilityAccess.IsMeadowLoaded) AddMeadowHooks();
        Logger.LogInfo(CompatibilityAccess.IsMeadowLoaded);
        LoadResources(self);
        _init = true;
    }

    public void LoadResources(RainWorld _)
    {
        Lanta.stunForehead ??= Futile.atlasManager.LoadAtlas("atlases/stunmaster_face");
        Lanta.stunArm ??= Futile.atlasManager.LoadAtlas("atlases/stunmaster_arm");
        Lanta.stunLegs ??= Futile.atlasManager.LoadAtlas("atlases/stunmaster_legs");
        DroneGraphics.DroneSprite ??= Futile.atlasManager.LoadAtlas("atlases/drone");
        Crits.DroneIcon ??= Futile.atlasManager.LoadAtlas("atlases/droneIco");
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
