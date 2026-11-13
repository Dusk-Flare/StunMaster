using UnityEngine;

namespace StunMaster.WorldEdit.Cliffside
{
    public class CliffsideClouds(CliffsideView cliffside, Vector2 pos, float depth, int index) : BackgroundScene.BackgroundSceneElement(cliffside, pos, depth)
    {
        public CliffsideView Cliffside
        {
            get => scene as CliffsideView;
        }
        public float randomOffset = Random.value;
        public int index = index;
        public Color skyColor;

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            skyColor = palette.skyColor;
        }
    }
}
