using UnityEngine;

namespace StunMaster.EngeneerDrone
{
    public class DroneGraphics : GraphicsModule
    {
        public static FAtlas DroneSprite;
        public Drone drone;
        public DroneGraphics(PhysicalObject ow) : base(ow, false)
        {
            drone = ow as Drone;
            Plugin.Logger.LogInfo("Drone graphics ctor called");
        }

        public override void Update()
        {
            base.Update();
            State += .1f;
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new("drone0")
            {
                color = new Color(1f, 1f, 1f)
            };
            AddToContainer(sLeaser, rCam, null);
            base.InitiateSprites(sLeaser, rCam);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            int num = (int) State;
            sLeaser.sprites[0].element = Futile.atlasManager.GetElementWithName($"drone{num}");
            sLeaser.sprites[0].isVisible = true;
            sLeaser.sprites[0].SetPosition(drone.mainBodyChunk.pos - camPos);
        }

        private float _state = 0;

        public float State
        {
            get => _state;
            set => _state = value % 8;
        }
    }
}
