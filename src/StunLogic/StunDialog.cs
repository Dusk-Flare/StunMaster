using System.Collections.Generic;
using static Conversation;
using static SSOracleBehavior;

namespace StunMaster.StunLogic
{
    internal static class StunDialog
    {
        public static readonly ID StunPebbles = new("StunPebbles", register: true);
        public static readonly ID StunMoon = new("StunMoon", register: true);
        public static readonly Dictionary<SSOracleBehavior, bool> ReadyToThrowOut = [];

        internal static void MeetPebbles(SSOracleBehavior self)
        {
            if (self?.dialogBox == null) return;
            if (self.currSubBehavior is not ConversationBehavior convBehav) return;

            PebblesConversation convo = new(self, convBehav, StunPebbles, self.dialogBox);
            self.action = Action.General_Idle;
            convo.events.Add(new TextEvent(convo, 0, "Ah, the 'stunmaster'.", 40));
            convo.events.Add(new TextEvent(convo, 0, "Why did the Flares of Dusk send you here?", 60));
            convo.events.Add(new TextEvent(convo, 0, "I'm fine, if you must know.", 40));
            convo.events.Add(new TextEvent(convo, 0, "However, you should leave, i have work i must tend to.", 60));
            convo.events.Add(new TextEvent(convo, 0, "I don't care what Flares said, leave me alone at once, I will not ask again.", 40));
            if (self.conversation == null) self.conversation.paused = true;
            convo.events.Add(new WaitEvent(convo, 80));
            convo.events.Add(new EndConversationEvent(convo, self, () =>
            {
                self.currSubBehavior = new ThrowOutBehavior(self);
                self.NewAction(Action.ThrowOut_ThrowOut);
                self.throwOutCounter = 140;
            }));

            self.conversation = convo;
        }
    }
    internal class EndConversationEvent(PebblesConversation conv, SSOracleBehavior oracle, System.Action onConversationEnd) : DialogueEvent(conv, 0)
    {
        private readonly SSOracleBehavior oracle = oracle;
        private bool finished = false;
        private readonly System.Action onConversationEnd = onConversationEnd;

        public override void Update()
        {
            base.Update();

            if (!finished && oracle != null)
            {
                oracle.conversation = null;
                finished = true;
                onConversationEnd?.Invoke();
            }
        }

        public override bool IsOver => finished;
    }
}
