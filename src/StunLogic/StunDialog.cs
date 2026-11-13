using System.Collections.Generic;

namespace StunMaster.StunLogic
{
    internal static class StunDialog
    {
        public static Conversation.ID StunPebbles;
        public static SSOracleBehavior.Action GreetLanta;
        public static SSOracleBehavior.SubBehavior.SubBehavID MeetLanta;

        public static void Register()
        {
            StunPebbles = new("StunPebbles", true);
            GreetLanta = new("GreetLanta", true);
            MeetLanta = new("MeetLanta", true);
        }
    }
}
