using StunMaster.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SlugBase.DataTypes;
using UnityEngine;

namespace StunMaster.StunCat
{
    public static class Lanta
    {
        public static ConditionalWeakTable<PlayerGraphics, FAtlasIndex> Table = new();
        public static ConditionalWeakTable<string, FAtlas> SpriteTable = new();
        public static FAtlas stunForehead;
        public static FAtlas stunArm;
        public static FAtlas stunLegs;
        public static int startSpriteIndex = -1;
        public static void PlayerUpdate(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            bool legacy = Plugin.OIinstance.LPC.Value;
            if (Plugin.OnlineData is not null) legacy = Plugin.OnlineData.LegacyPowers;
            StunInputs.Update(self, legacy);
        }

        public static void InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (self.player.SlugCatClass.value == PluginData.stunMasterID)
            {
                startSpriteIndex = sLeaser.sprites.Length;
                Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 4);

                if (!Table.TryGetValue(self, out FAtlasIndex theData))
                {
                    theData = new();
                    Table.Remove(self);
                    Table.Add(self, theData);
                }
                theData[9] = startSpriteIndex;
                theData[5] = startSpriteIndex + 1;
                theData[6] = startSpriteIndex + 2;
                theData[4] = startSpriteIndex + 3;

                sLeaser.sprites[startSpriteIndex] = new FSprite("stunmaster_FaceA0");
                sLeaser.sprites[startSpriteIndex + 1] = new FSprite("stunmaster_PlayerArm0");
                sLeaser.sprites[startSpriteIndex + 2] = new FSprite("stunmaster_PlayerArm0");
                sLeaser.sprites[startSpriteIndex + 3] = new FSprite("stunmaster_LegsA0");

                foreach (KeyValuePair<int, int> index in theData)
                {
                    FSprite newSprite = sLeaser.sprites[index.Value];
                    FSprite baseSprite = sLeaser.sprites[index.Key];

                    newSprite.anchorX = baseSprite.anchorX;
                    newSprite.anchorY = baseSprite.anchorY;
                }
            }
        }

        public static void DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (!Table.TryGetValue(self, out FAtlasIndex data)) return;
            foreach (KeyValuePair<int, int> index in data)
            {
                FSprite newSprite = sLeaser.sprites[index.Value];
                FSprite baseSprite = sLeaser.sprites[index.Key];

                if (baseSprite.container != newSprite.container) {
                    newSprite.RemoveFromContainer();
                    baseSprite.container.AddChild(newSprite);
                }
                newSprite.MoveInFrontOfOtherNode(baseSprite);
                newSprite._textureRect = baseSprite.textureRect;
                newSprite.scaleX = baseSprite.scaleX;
                newSprite.scaleY = baseSprite.scaleY;
                newSprite.rotation = baseSprite.rotation;
                newSprite.color = PlayerColor.GetCustomColor(self, "Markings");
                newSprite.alpha = baseSprite.alpha;
                newSprite.isVisible = baseSprite.isVisible;
                newSprite.SetPosition(baseSprite.GetPosition());

                if(baseSprite.element != null) newSprite.element = Futile.atlasManager.GetElementWithName(PluginData.stunMasterID + "_" + baseSprite.element.name);
            }
            sLeaser.sprites[7].color = PlayerColor.GetCustomColor(self, "Markings");
            sLeaser.sprites[8].color = PlayerColor.GetCustomColor(self, "Markings");
        }
    }
}
