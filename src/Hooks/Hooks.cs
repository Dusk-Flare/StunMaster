namespace StunMaster.Hooks
{
    public static partial class HookManager
    {
        public static void Apply()
        {
            ApplyGraphics();
            ApplyPebbles();
            ApplyWorld();
        }
        public static void Remove()
        {
            RemoveGraphics();
            RemovePebbles();
            RemoveWorld();
        }
    }
}
