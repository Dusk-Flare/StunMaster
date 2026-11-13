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

        public static void SeePlayer(On.SSOracleBehavior.orig_SeePlayer orig, SSOracleBehavior self)
        {
            orig(self);
            if (self.oracle.room.game.StoryCharacter.value == PluginData.stunMasterID)
            {
                self.NewAction(StunDialog.GreetLanta);
            }
        }

        public static void AddEvents(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation self)
        {
            if (self.id == StunDialog.StunPebbles) 
            {
                self.LoadEventsFromFile(900);
            } 
            else orig(self);
        }

        public static void NewAction(On.SSOracleBehavior.orig_NewAction orig, SSOracleBehavior self, SSOracleBehavior.Action nextAction)
        {
            if (nextAction == StunDialog.GreetLanta)
            {
                SSOracleBehavior.SubBehavior.SubBehavID subBehavID = StunDialog.MeetLanta;
                self.currSubBehavior.NewAction(self.action, nextAction);
                if (subBehavID != self.currSubBehavior.ID)
                {
                    SSOracleBehavior.SubBehavior subBehavior = null;
                    for (int i = 0; i < self.allSubBehaviors.Count; i++)
                    {
                        if (self.allSubBehaviors[i].ID == subBehavID)
                        {
                            subBehavior = self.allSubBehaviors[i];
                            break;
                        }
                    }
                    if (subBehavior == null)
                    {
                        self.LockShortcuts();
                        subBehavior = new SSOracleMeetLanta(self);
                        self.allSubBehaviors.Add(subBehavior);
                    }
                    subBehavior.Activate(self.action, nextAction);
                    self.currSubBehavior.Deactivate();
                    Plugin.Logger.LogInfo(string.Format("Switching subbehavior to: {0} from: {1}", subBehavior.ID, self.currSubBehavior.ID));
                    self.currSubBehavior = subBehavior;
                }
                self.inActionCounter = 0;
                self.action = nextAction;
            }
            else orig(self, nextAction);
        }
    }
}
