namespace StunMaster.Hooks
{
    public static partial class HookManager
    {
        public static void Apply()
        {
            ApplyGraphics();
            ApplyPebbles();
        }
        public static void Remove()
        {
            RemoveGraphics();
            RemovePebbles();
        }
    }
}
