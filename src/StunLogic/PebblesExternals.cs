namespace StunMaster.StunLogic
{
    public static class PebblesExternals
    {
        public static void NeuronOverride(On.SSOracleSwarmer.orig_Update orig, SSOracleSwarmer self, bool eu)
        {
            orig(self, eu);
            Player player = null;
            if (self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player) player = self.grabbedBy[0].grabber as Player;
            NeuronPathing.NeuronControll(self, player);
        }
        public static void CustomPebblesDialogue(On.SSOracleBehavior.orig_NewAction orig, SSOracleBehavior self, SSOracleBehavior.Action action)
        {
            if (!StunDialog.ReadyToThrowOut.ContainsKey(self)) StunDialog.ReadyToThrowOut[self] = false;
            orig(self, action);

            int? metBefore = self.oracle?.room?.game?.GetStorySession?.saveState?.miscWorldSaveData?.SSaiThrowOuts;
            if (self.oracle.room.game.StoryCharacter.value != PluginData.stunMasterID || metBefore != null && metBefore > 0) return;

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
