namespace StunMaster.EngeneerDrone
{
    public class AbstractDroneAI : AbstractCreatureAI
    {
        public AbstractDroneAI(World world, AbstractCreature parent) : base(world, parent)
        {
        }

        public override void AbstractBehavior(int time)
        {
            if (followCreature != null) MoveWithCreature(followCreature, true);
        }
    }
}
