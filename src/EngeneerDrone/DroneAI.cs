namespace StunMaster.EngeneerDrone
{
    public class DroneAI : ArtificialIntelligence
    {
        public Creature Owner;
        public Drone RealDrone;
        public float idleCounter;
        public Behaviour behaviour;
        public Creature targetCreature;
        public DroneAI(AbstractCreature creature, World world) : base(creature, world)
        {
            RealDrone = creature.realizedCreature as Drone;
            RealDrone.AI = this;
            AddModule(new MissionTracker(this));
            AddModule(new StandardPather(this, world, creature));
        }

        public override void Update()
        {
            base.Update();
            if (RealDrone.room == null || Owner == null) return;

            if(!RealDrone.OwnerSeen)
            {
                WorldCoordinate target = Owner.abstractCreature.pos;
                SetDestination(target);
            }

            if(Owner.inShortcutVessel is not null)
            {
                ShortcutHandler.ShortCutVessel vessel = Owner.inShortcutVessel;
                WorldCoordinate pos = RealDrone.room.GetWorldCoordinate(vessel.pos);
                SetDestination(pos);
            }
        }

        public class Behaviour(string value, bool register = true) : ExtEnum<Behaviour>(value, register)
        {
            public static readonly Behaviour Follow = new("Follow", true);
            public static readonly Behaviour HoverTarget = new("HoverTarget", true);
            public static readonly Behaviour Wander = new("Wander", true);
            public static readonly Behaviour Scan = new("Scan", true);
        }
    }
}
