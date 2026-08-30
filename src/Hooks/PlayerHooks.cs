namespace StunMaster.Hooks
{
    partial class HookManager
    {
        public static void ApplyGraphics()
        {
            On.PlayerGraphics.InitiateSprites += StunCat.Lanta.InitiateSprites;
            On.PlayerGraphics.DrawSprites += StunCat.Lanta.DrawSprites;
            On.Player.Update += StunCat.Lanta.PlayerUpdate;
        }

        public static void RemoveGraphics()
        {
            On.PlayerGraphics.InitiateSprites -= StunCat.Lanta.InitiateSprites;
            On.PlayerGraphics.DrawSprites -= StunCat.Lanta.DrawSprites;
            On.Player.Update -= StunCat.Lanta.PlayerUpdate;
        }
    }
}
