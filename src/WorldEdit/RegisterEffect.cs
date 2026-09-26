using StunMaster.Utils;
using StunMaster.WorldEdit.Cliffside;

namespace StunMaster.WorldEdit
{
    public static class RegisterEffect
    {
        public static EffectTable Effects = [];

        public static void RegisterEnum()
        {
            Effects.Register("CliffSideEffect", true);
        }

        public static void UnregisterEnum()
        {
            foreach (string effect in Effects.Keys)
            {
                Effects[effect]?.Unregister();
                Effects[effect] = null;
            }
        }

        public static void RoomLoaded(On.Room.orig_Loaded orig, Room self)
        {
            orig(self);
            if (self.game == null || self.roomSettings == null) return;
            foreach (RoomSettings.RoomEffect.Type type in Effects.Values)
            {
                if (type == null) continue;
                RoomSettings.RoomEffect effect = self.roomSettings.GetEffect(type);
                if (effect != null && effect.type == type)
                {
                    self.AddObject(new CliffsideView(self, effect));
                }
            }
        }

        public static int GetSliderCount(On.RoomSettings.RoomEffect.orig_GetSliderCount orig, RoomSettings.RoomEffect.Type type)
        {
            if (type == Effects["CliffSideEffect"]) return 1;
            return orig(type);
        }

        public static string GetSliderName(On.RoomSettings.RoomEffect.orig_GetSliderName orig, RoomSettings.RoomEffect.Type type, int index)
        {
            if (type == Effects["CliffSideEffect"] && index == 0) return "Cliff Sky";
            return orig(type, index);
        }

        public static float GetSliderDefault(On.RoomSettings.RoomEffect.orig_GetSliderDefault orig, RoomSettings.RoomEffect.Type type, int index)
        {
            if (type == Effects["CliffSideEffect"] && index == 0) return 0f;
            return orig(type, index);
        }
    }
}
