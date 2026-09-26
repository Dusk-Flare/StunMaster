using UnityEngine;

namespace StunMaster.WorldEdit.Cliffside
{
    public class CliffsideView : BackgroundScene
    {
        public Simple2DBackgroundIllustration Cliffside { get; set; }
        public RoomSettings.RoomEffect CliffsideEffect { get; set; }
        public Color SkyColor;
        public CliffsideView(Room room, RoomSettings.RoomEffect effect) : base(room)
        {
            CliffsideEffect = effect;
            sceneOrigo = new Vector2(2514f, 20000f);
            bool isMoon = RXRandom.Int(100) == 99;
            Cliffside = new Simple2DBackgroundIllustration(this, isMoon ? "AT_SkyscapeMoon" : "AT_Skyscape", new Vector2(683f, 368f));
            AddElement(Cliffside);

            Shader.SetGlobalVector(RainWorld.ShadPropAboveCloudsAtmosphereColor, new Color(0.5f, 0.5f, 0.5f, 1f));
        }
    }
}
