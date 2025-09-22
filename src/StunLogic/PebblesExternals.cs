using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StunMaster.StunLogic
{
    internal static class PebblesExternals
    {
        internal static void Apply()
        {
            On.SSOracleSwarmer.Update += NeuronOverride;
            //On.SSOracleBehavior.NewAction += CustomPebblesDialogue;
        }

        private static void NeuronOverride(On.SSOracleSwarmer.orig_Update orig, SSOracleSwarmer self, bool eu)
        {
            orig(self, eu);
            Player player = self.grabbedBy.Count > 0 ? self.grabbedBy[0].grabber as Player : null;
            NeuronPathing.NeuronControll(self, player);
        }
        private static void CustomPebblesDialogue(On.SSOracleBehavior.orig_NewAction orig, SSOracleBehavior self, SSOracleBehavior.Action action)
        {
            if (!StunDialog.ReadyToThrowOut.ContainsKey(self)) StunDialog.ReadyToThrowOut[self] = false;
            orig(self, action);

            var metBefore = self.oracle?.room?.game?.GetStorySession?.saveState?.miscWorldSaveData?.SSaiThrowOuts;
            if (self.oracle.room.game.StoryCharacter.value != Plugin.stunMasterID || metBefore != null && metBefore > 0) return;

            StunDialog.MeetPebbles(self);
            if (StunDialog.ReadyToThrowOut.TryGetValue(self, out bool shouldThrow) && shouldThrow)
            {
                // Wait until the convo ends
                if (self.conversation == null)
                {
                    StunDialog.ReadyToThrowOut[self] = false;
                    self.currSubBehavior = new SSOracleBehavior.ThrowOutBehavior(self);
                    self.NewAction(SSOracleBehavior.Action.ThrowOut_ThrowOut);
                    self.throwOutCounter = 140;
                    Plugin.Logger.LogInfo("Throwing out StunMaster from Oracle's room.");
                }
            }
        }
    }
}
