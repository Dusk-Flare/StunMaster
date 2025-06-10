using System;
using System.Runtime.CompilerServices;

namespace StunMaster
{
    internal class StunDialog
    {
        public static readonly Conversation.ID StunPebbles = new("StunPebbles", true);
        public static readonly Conversation.ID StunMoon = new("StunMoon", true);

        internal static void MeetPebbles(SSOracleBehavior self, SSOracleBehavior.Action action)
        {
            var convBehav = self.currSubBehavior as SSOracleBehavior.ConversationBehavior;
            var convo = new SSOracleBehavior.PebblesConversation(self, convBehav, StunPebbles, self.dialogBox);

            convo.events.Add(new Conversation.TextEvent(convo, 0, "Ah, the 'stunmaster'.", 80));
            convo.events.Add(new Conversation.TextEvent(convo, 0, "Why did the iterator 'Flares of Dusk' send you here?", 80));
            convo.events.Add(new Conversation.TextEvent(convo, 0, "I'm fine, if you must know.", 60));
            convo.events.Add(new Conversation.TextEvent(convo, 0, "But you should leave before i decide to put an end to your current cycle.", 80));
            convo.events.Add(new Conversation.TextEvent(convo, 0, "I don't care what your Flares said, leave me alone at once, i will not ask again.", 80));
            convo.events.Add(new Conversation.WaitEvent(convo, 40));
            // Use a custom event to trigger the throw out after dialog finishes
            convo.events.Add(new Conversation.SpecialEvent(convo, 10, "ThrowOut_ThrowOut"));
            self.conversation = convo;

        }
    }
}
