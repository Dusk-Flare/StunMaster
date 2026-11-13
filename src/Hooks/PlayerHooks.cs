using StunMaster.StunCat;

namespace StunMaster.Hooks
{
    partial class HookManager
    {
        public static void ApplyGraphics()
        {
            On.PlayerProgression.GetOrInitiateSaveState += Lanta.GetOrInitiateSaveState;
            On.PlayerGraphics.InitiateSprites += Lanta.InitiateSprites;
            On.PlayerGraphics.DrawSprites += Lanta.DrawSprites;
            On.Player.Update += Lanta.PlayerUpdate;
        }

        public static void RemoveGraphics()
        {
            On.PlayerProgression.GetOrInitiateSaveState -= Lanta.GetOrInitiateSaveState;
            On.PlayerGraphics.InitiateSprites -= Lanta.InitiateSprites;
            On.PlayerGraphics.DrawSprites -= Lanta.DrawSprites;
            On.Player.Update -= Lanta.PlayerUpdate;
        }
    }
}
