using ImprovedInput;
using StunMaster.StunCat;
using UnityEngine;

namespace StunMaster.Utils
{
    public static class StunInputs
    {
        public static StunCooldowns StunEntries = new();
        public static readonly PlayerKeybind stunTrigger = PlayerKeybind.Register("stun:bind", PluginData.ModName, "StunAbility", KeyCode.C, KeyCode.JoystickButton5);
        public static bool StunInput(StunPlayer player) => player.Instance.JustPressed(stunTrigger);
        public static void Update(StunPlayer player, bool legacy)
        {
            StunEntries.Update(player);
            
            if (player.Stats.name.value == PluginData.stunMasterID && legacy)
            {
                if (StunInput(player))
                {
                    bool isStory = player.Room?.game.session is StoryGameSession;

                    if (isStory) player.TriggerStoryStun(7, 2);
                    else player.TriggerArenaStun(7, 3, 2);
                }
            }
        }
    }
}
