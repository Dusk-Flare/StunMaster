using BepInEx;
using BepInEx.Logging;
using IL.JollyCoop.JollyMenu;
using MoreSlugcats;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Permissions;
using UnityEngine;
using static Creature;

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
    public OptionsMenu OIinstance = new();

    public void OnEnable()
    {
        
        Logger = base.Logger;
        On.RainWorld.OnModsInit += OnModsInit;
        On.Player.Update += PlayerDrop;
        On.Scavenger.Update += ScavDrop;
        On.Vulture.Update += KingVultureDrop;
    }

    private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        if (IsInit) return;
        IsInit = true;

        MachineConnector.SetRegisteredOI("stunfall", OIinstance);
    }

    private void PlayerDrop(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if(self.grabbedBy.Any(grasp => grasp.grabber is Lizard) && !OIinstance.LGP.Value)
        {
            return;
        }

        if (self.stun > 0)
        {
            self.tongue?.Release();
            for (int i = self.grasps.Length - 1; i >= 0; i--)
            {
                if (self.grasps[i] != null)
                {
                    if (OIinstance.DSI.Value)
                    {
                        self.ReleaseGrasp(i);
                    }
                    else
                    {
                        PhysicalObject item = self.grasps[i].grabbed;
                        if (item is not EnergyCell && item is not NSHSwarmer && item is not SpearMasterPearl)
                        {
                            self.ReleaseGrasp(i);
                        }
                    }
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

    private void KingVultureDrop(On.Vulture.orig_Update orig, Vulture self, bool eu)
    {
        orig(self, eu);

        if (self.Template.type == CreatureTemplate.Type.KingVulture && self.stun > 20)
        {
            for (int i = self.kingTusks.tusks.Length - 1; i >= 0; i--)
            {
                var tusk = self.kingTusks.tusks[i];
                if (tusk != null)
                {
                    tusk.SwitchMode(KingTusks.Tusk.Mode.Dangling);
                    tusk.currWireLength = 500f;
                }
            }
        }
    }

}
