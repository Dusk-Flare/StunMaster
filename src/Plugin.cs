using BepInEx;
using BepInEx.Logging;
using System.Security.Permissions;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace StunFall;

[BepInPlugin("com.author.stunfall", "Stun Fall", "0.1.0")]
sealed class Plugin : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    bool IsInit;

    public void OnEnable()
    {
        Logger = base.Logger;
        On.RainWorld.OnModsInit += OnModsInit;
        On.Player.Update += PlayerDrop;
        On.Scavenger.Update += ScavDrop;
    }

    private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (IsInit) return;
        IsInit = true;

        // Initialize assets, your mod config, and anything that uses RainWorld here
        Logger.LogDebug("Hello world!");
    }

    private void PlayerDrop(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);
        
        if(self.stun > 0)
        {
            for (int i = self.grasps.Length - 1; i >= 0; i--)
            {
                if (self.grasps[i] != null)
                {
                    self.ReleaseGrasp(i);
                }
            }

        }
    }

    private void ScavDrop(On.Scavenger.orig_Update orig, Scavenger self, bool eu)
    {
        orig(self, eu);

        if (self.stun > 0)
        {
            for (int i = self.grasps.Length - 1; i >= 0; i--)
            {
                if (self.grasps[i] != null)
                {
                    self.ReleaseGrasp(i);
                }
            }

        }
    }
}
