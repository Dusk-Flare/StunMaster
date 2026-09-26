using UnityEngine;

namespace StunMaster.WorldEdit.Cliffside
{
    public class CliffsideClouds : BackgroundScene.BackgroundSceneElement
    {
        public CliffsideView Cliffside
        {
            get => scene as CliffsideView;
        }
        public float randomOffset;
        public int index;
        public Color skyColor;
        public CliffsideClouds(CliffsideView cliffside, Vector2 pos, float depth, int index) : base(cliffside, pos, depth)
        {
            randomOffset = Random.value;
            this.index = index;
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            skyColor = palette.skyColor;
        }
    }
}
