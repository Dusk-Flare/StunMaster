using ImprovedInput;
using StunMaster.StunCat;

namespace StunMaster.Utils
{
    public static class StunInputs
    {
        public static StunCooldowns StunEntries = new();
        public static bool StunInput(Player player) => player.JustPressed(Plugin.stunTrigger);
        public static bool IsStunMaster(Player player) => player.slugcatStats.name.value == PluginData.stunMasterID;
        public static void Update(Player player, bool legacy)
        {
            StunEntries.Update(player);
            if (!IsStunMaster(player) || !StunInput(player) || !legacy) return;
            Lanta.Drones.Remove(player);
            if (player.room?.game.session is StoryGameSession) player.TriggerStoryStun(7, 2);
            else player.TriggerArenaStun(7, 3, 2);
        }
    }
}
