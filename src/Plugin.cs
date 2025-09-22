using BepInEx;
using BepInEx.Logging;
using MonoMod.RuntimeDetour;
using SlugBase;
using StunMaster.StunCompat;
using StunMaster.StunLogic;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;
using UnityEngine;


#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace StunMaster;

[BepInPlugin("com.author.stunmaster", "Stun Master", "0.1.0")]
sealed class Plugin : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    public static string stunMasterID = "stunmaster";
    public static AbstractPhysicalObject.AbstractObjectType stunSpear = new("StunSpear", true);
    public static ReturnData OnlineData;
    public static Dictionary<Player, float> stunCooldowns = [];
    public static OptionsMenu OIinstance = new();
    bool IsInit;

    public void OnEnable()
    {
        Logger = base.Logger;
        On.RainWorld.OnModsInit += OnModsInit;
        On.Player.Update += PlayerUpdate;
        StunPowers.Apply();
    }

    private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (IsInit) return;
        MachineConnector.SetRegisteredOI(stunMasterID, OIinstance);
        if (CompatibilityAccess.IsRainMeadowLoaded()) AddMeadowHooks();
        IsInit = true;
    }
    private void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);
        bool legacy = OIinstance.LPC.Value;
        if (OnlineData is not null)
        {
            legacy = OnlineData.LegacyPowers;
        }
        Player.InputPackage currInput = self.input[0];
        Player.InputPackage prevInput = self.input[1];
        if (!stunCooldowns.ContainsKey(self)) stunCooldowns[self] = 0f;
        if (stunCooldowns.ContainsKey(self) && stunCooldowns[self] > 0f) stunCooldowns[self]--;

        if (self.slugcatStats.name.value == stunMasterID && legacy)
        {
            if (currInput.jmp && currInput.pckp && (!prevInput.jmp || !prevInput.pckp))
            {
                bool isStory = self.room?.game.session is StoryGameSession;

                if (isStory) LegacyStun.TriggerStoryStun(self, 7, 2);
                else LegacyStun.TriggerArenaStun(self, 7, 3, 2);
            }
        }
    }

    private void AddMeadowHooks()
    {
        new Hook(typeof(RainMeadow.Lobby).GetMethod("ActivateImpl", BindingFlags.NonPublic | BindingFlags.Instance), (Action<RainMeadow.Lobby> orig, RainMeadow.Lobby self) =>
        {
            orig(self);
            try
            {
                CompatibilityAccess.PassOnlineData();
                OnlineData = CompatibilityAccess.GetOnlineData();
            }
            catch (Exception except)
            {
                RainMeadow.RainMeadow.Error(except);
            }
        });
    }
}
