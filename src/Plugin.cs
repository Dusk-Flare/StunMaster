using BepInEx;
using BepInEx.Logging;
using System.Collections.Generic;
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
    public static Dictionary<Player, float> stunCooldowns = [];
    bool IsInit;

    public void OnEnable()
    {
        Logger = base.Logger;
        On.Player.Update += Player_Update;
        On.RainWorld.OnModsInit += OnModsInit;
        On.SSOracleBehavior.NewAction += CustomIteratorDialogue;
        On.SSOracleSwarmer.Update += NeuronOverride;
    }


    private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (IsInit) return;
        IsInit = true;
    }
    private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);
        if (!stunCooldowns.ContainsKey(self)) stunCooldowns[self] = 0f;
        if (stunCooldowns.ContainsKey(self) && stunCooldowns[self] > 0f) stunCooldowns[self]--;

        if (self.slugcatStats.name.value == "stunmaster")
        {
            if (self.input[0].jmp && self.input[0].pckp && (!self.input[1].jmp || !self.input[1].pckp))
            {
                bool isArena = self.room?.game != null && self.room.game.IsArenaSession;
                if (isArena)
                {
                    StunPower.TriggerArenaStun(self, 7, 3, 2);
                }
                else
                {
                    StunPower.TriggerStoryStun(self, 7, 2);
                }
            }
        }
    }

    private void NeuronOverride(On.SSOracleSwarmer.orig_Update orig, SSOracleSwarmer self, bool eu)
    {
        orig(self, eu);
        Player player = self.grabbedBy.Count > 0 ? self.grabbedBy[0].grabber as Player : null;
        NeuronPathing.NeuronControll(self, player);
    }




    private void CustomIteratorDialogue(On.SSOracleBehavior.orig_NewAction orig, SSOracleBehavior self, SSOracleBehavior.Action action)
    {
        orig(self, action);
        return; // Disable all iterator dialogue
        var metBefore = self.oracle?.room?.game?.GetStorySession?.saveState?.miscWorldSaveData?.SSaiThrowOuts;
        if (self.oracle.room.game.StoryCharacter.value != "stunmaster" || metBefore > 0) orig(self, action);
        StunDialog.MeetPebbles(self, action);
        orig(self, action);
    }
}
