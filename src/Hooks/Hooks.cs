namespace StunMaster.Hooks
{
    public static partial class HookManager
    {
        public static void Apply()
        {
            ApplyGraphics();
            ApplyPebbles();
            ApplyWorld();
            ApplyCreatures();
        }
        public static void Remove()
        {
            RemoveGraphics();
            RemovePebbles();
            RemoveWorld();
            RemoveCreatures();
        }
    }
}
