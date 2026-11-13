using RWCustom;
using UnityEngine;

namespace StunMaster.EngeneerDrone
{
    public class Drone : Creature, IOwnProjectedCircles
    {
        public DroneAI AI;
        public Creature Owner => AI.Owner;
        public WorldCoordinate OwnerLastSeenPos;
        public Vector2 CurrentPos;
        public Vector2 LastPos;
        public float StuckTimer;
        public bool OwnerSeen;
        public Drone(AbstractCreature abstractCreature, World world) : base(abstractCreature, world)
        {
            Plugin.Logger.LogInfo("Drone ctor called");
            mainBodyChunkIndex = 0;
            bodyChunks = new BodyChunk[1];
            bodyChunks[0] = new BodyChunk(this, 0, new(0, 0), 6f, .05f);
            bodyChunkConnections = [];
            GoThroughFloors = true;
            airFriction = 0.9f;
            gravity = 0;
            bounce = 0.1f;
            surfaceFriction = 0.5f;
            collisionLayer = 0;
            waterFriction = 0.9f;
            buoyancy = 0f;
            windAffectiveness = 0.5f;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (Owner == null) Destroy();
            if (AI == null || Owner == null || room == null) return;
            if (Owner.slatedForDeletetion) Destroy();
            CurrentPos = mainBodyChunk.pos;
            OwnerSeen = false;
            if(Owner?.inShortcutVessel != null)
            {
                ShortcutHandler.ShortCutVessel vessel = Owner.inShortcutVessel;
                if(!inShortcut) SuckedIntoShortCut(vessel.pos, false);
            }
            if(Owner.room == room)
            {
                WorldCoordinate ownerPos = Owner.abstractCreature.pos;
                WorldCoordinate selfPos = abstractCreature.pos;
                if (selfPos.Tile.FloatDist(ownerPos.Tile) > 15f) Teleport(Owner.mainBodyChunk.pos);
            }
            if (SeesOwner())
            {
                OwnerSeen = true;
                OwnerLastSeenPos = Owner.abstractCreature.pos;
                Vector2 direction = Custom.DirVec(CurrentPos, Owner.mainBodyChunk.pos);
                FlyTowards(direction, 6f);
            }
            else
            {
                MovementConnection connection = (AI.pathFinder as StandardPather).FollowPath(abstractCreature.pos, true);
                if (connection != default)
                {
                    mainBodyChunk.vel = Vector2.Lerp(mainBodyChunk.vel, FlightDirection(connection) * 6f, .15f);
                }
                else FlyTowards(OwnerLastSeenPos, 6f);
            }
            if(StuckTimer > 3f)
            {
                Vector2 direction = new Vector2(Random.value, Random.value).normalized;
                FlyTowards(direction, 2f);
            }
            Act();
            if (Vector2.Distance(CurrentPos, LastPos) < 1f) StuckTimer++;
            else StuckTimer = 0;
            LastPos = CurrentPos;
        }

        public Vector2 FlightOffset(Vector2 facing, Vector2 pos, float followRad)
        {
            facing.Normalize();
            Vector2 offset = new Vector2(Mathf.Sqrt(2)/2, Mathf.Sqrt(2) / 2) * followRad;
            if (facing.y > -Mathf.Sin(40f * Mathf.Deg2Rad)) offset.y *= -1;
            if (facing.x < 0) offset.x *= -1;

            float iter = 0f;
            Vector2 temp = offset + pos;
            while (!FitsThrough(pos, temp))
            {
                temp = Vector2.Lerp(offset + pos, pos, iter);
                iter += .1f;
            }
            return temp;
        }

        public bool FitsThrough(Vector2 startPos, Vector2 endPos)
        {
            if (startPos == endPos) return true;
            if(!room.VisualContact(startPos, endPos)) return false;
            Vector2 direction = (endPos - startPos).normalized;
            Vector2 offset = new Vector2(-direction.y, direction.x) * mainBodyChunk.rad;
            return room.VisualContact(startPos + offset, endPos + offset) && room.VisualContact(startPos - offset, endPos - offset);
        }

        public bool SeesOwner()
        {
            if (Owner.room != room) return false;
            return FitsThrough(CurrentPos, Owner.mainBodyChunk.pos);
        }

        public void FlyTowards(Vector2 direction, float speed)
        {
            mainBodyChunk.vel = Vector2.Lerp(mainBodyChunk.vel, direction * speed, .15f);
        }

        public void FlyTowards(WorldCoordinate position, float speed)
        {
            if (abstractCreature.pos.room != position.room) return;
            Vector2 direction = Custom.DirVec(CurrentPos, room.MiddleOfTile(position));
            FlyTowards(direction, speed);
        }

        public void Teleport(Vector2 position)
        {
            mainBodyChunk.pos = position;
            mainBodyChunk.vel = Vector2.zero;
        }

        public Vector2 FlightDirection(MovementConnection connection)
        {
            Vector2 target = room.MiddleOfTile(connection.destinationCoord);
            MovementConnection step = connection;

            for (int i = 0; i < 10; i++)
            {
                step = (AI.pathFinder as StandardPather).FollowPath(step.destinationCoord, false);

                if (!step.destinationCoord.TileDefined || !FitsThrough(CurrentPos, room.MiddleOfTile(step.destinationCoord))) break;

                target = room.MiddleOfTile(step.destinationCoord);

                if (step.destinationCoord == AI.pathFinder.GetDestination) break;
            }
            if (!step.destinationCoord.TileDefined) target = room.MiddleOfTile(OwnerLastSeenPos);
            Vector2 dir = Custom.DirVec(CurrentPos, target);
            return dir;
        }

        public virtual void Act()
        {
            AI.Update();
            MovementConnection movementConnection = (AI.pathFinder as StandardPather).FollowPath(abstractCreature.pos, true);
            if (movementConnection == default) return;
            if (shortcutDelay < 1 && (movementConnection.type == MovementConnection.MovementType.ShortCut || movementConnection.type == MovementConnection.MovementType.NPCTransportation))
            {
                enteringShortCut = new IntVector2?(movementConnection.StartTile);
                if (movementConnection.type == MovementConnection.MovementType.NPCTransportation)
                {
                    NPCTransportationDestination = movementConnection.destinationCoord;
                }
                return;
            }
        }

        public override void InitiateGraphicsModule()
        {
            graphicsModule ??= new DroneGraphics(this);
        }

        public override bool CanBeGrabbed(Creature grabber)
        {
            return true;
        }

        public Vector2 CircleCenter(int index, float timeStacker)
        {
            return mainBodyChunk.pos;
        }

        public Room HostingCircleFromRoom()
        {
            return room;
        }

        public bool CanHostCircle()
        {
            return !dead && !slatedForDeletetion;
        }
    }
}
