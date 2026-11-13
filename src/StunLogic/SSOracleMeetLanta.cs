namespace StunMaster.StunLogic
{
    public class SSOracleMeetLanta : SSOracleBehavior.ConversationBehavior
    {
        public SSOracleMeetLanta(SSOracleBehavior owner) : base(owner, StunDialog.MeetLanta, StunDialog.StunPebbles)
        {
            owner.getToWorking = 0f;
        }

        public override void Update()
        {
            base.Update();
            if (inActionCounter > 15 && !_yapped && owner.conversation == null)
            {
                owner.InitateConversation(convoID, this);
                _yapped = true;
            }
            if ((owner.conversation == null || owner.conversation.slatedForDeletion) && _yapped)
            {
                owner.NewAction(SSOracleBehavior.Action.ThrowOut_Polite_ThrowOut);
            }
        }

        private bool _yapped;
    }
}
