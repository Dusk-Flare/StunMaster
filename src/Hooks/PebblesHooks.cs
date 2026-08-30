using StunMaster.StunLogic;

namespace StunMaster.Hooks
{
    public static partial class HookManager
    {
        internal static void ApplyPebbles()
        {
            On.SSOracleSwarmer.Update += PebblesExternals.NeuronOverride;
            //On.SSOracleBehavior.NewAction += CustomPebblesDialogue;
        }
        internal static void RemovePebbles()
        {
            On.SSOracleSwarmer.Update -= PebblesExternals.NeuronOverride;
            //On.SSOracleBehavior.NewAction -= CustomPebblesDialogue;
        }
    }
}
