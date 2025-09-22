using System;
using System.Collections.Generic;
using UnityEngine;
using RWCustom;

namespace StunMaster.StunObjects
{
    internal class StunSpear : Spear
    {
        public Vector2[,] rag;
        public float conRad = 7f;
        public Color blueColor;
        public int igniteCounter;
        public int explodeAt;
        public bool exploded;
        public Color explodeColor = new(0.1f, 0.3f, 1f);
        public List<int> miniExplosions;
        public int destroyCounter;
        public SharedPhysics.TerrainCollisionData scratchTerrainCollisionData;

        public bool Ignited => igniteCounter > 0;

        public void Ignite()
        {
            if (igniteCounter <= 0)
            {
                igniteCounter = 1;
                room.PlaySound(SoundID.Fire_Spear_Ignite, firstChunk);
            }
        }

        public StunSpear(AbstractPhysicalObject abstractSpear, World world) : base(abstractSpear, world)
        {
            UnityEngine.Random.State state = UnityEngine.Random.state;
            UnityEngine.Random.InitState(abstractSpear.ID.RandomSeed);
            explodeAt = UnityEngine.Random.Range(60, 100);
            rag = new Vector2[UnityEngine.Random.Range(4, UnityEngine.Random.Range(4, 10)), 6];
            miniExplosions = [];
            int num = 20;
            for (int i = 0; i < explodeAt / num; i++)
            {
                miniExplosions.Add(UnityEngine.Random.Range(i * num, (i + 1) * num));
            }
            UnityEngine.Random.state = state;
        }

        public void ResetRag()
        {
            Vector2 vector = RagAttachPos(1f);
            for (int i = 0; i < rag.GetLength(0); i++)
            {
                rag[i, 0] = vector;
                rag[i, 1] = vector;
                rag[i, 2] *= 0f;
            }
        }

        public Vector2 RagAttachPos(float timeStacker)
        {
            return Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker) + (Vector2)Vector3.Slerp(lastRotation, rotation, timeStacker) * 15f;
        }

        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);
            ResetRag();
        }

        public override void NewRoom(Room newRoom)
        {
            base.NewRoom(newRoom);
            ResetRag();
        }

        public override void Update(bool eu)
        {
            bool flag = mode == Mode.Thrown;
            base.Update(eu);

            if (exploded)
            {
                if (destroyCounter > 4) return;
                destroyCounter++;
                room.AddObject(new PuffBallSkin(firstChunk.pos + rotation * (pivotAtTip ? 0f : 10f), Custom.RNV() * Mathf.Lerp(10f, 30f, UnityEngine.Random.value), blueColor, Color.Lerp(blueColor, new Color(0f, 0f, 0f), 0.3f)));
            }

            for (int i = 0; i < rag.GetLength(0); i++)
            {
                float t = i / (float)(rag.GetLength(0) - 1);
                rag[i, 1] = rag[i, 0];
                rag[i, 0] += rag[i, 2];
                rag[i, 2] -= rotation * Mathf.InverseLerp(1f, 0f, i) * 0.8f;
                rag[i, 4] = rag[i, 3];
                rag[i, 3] = (rag[i, 3] + rag[i, 5] * Custom.LerpMap(Vector2.Distance(rag[i, 0], rag[i, 1]), 1f, 18f, 0.05f, 0.3f)).normalized;
                rag[i, 5] = (rag[i, 5] + Custom.RNV() * UnityEngine.Random.value * Mathf.Pow(Mathf.InverseLerp(1f, 18f, Vector2.Distance(rag[i, 0], rag[i, 1])), 0.3f)).normalized;

                if (room.PointSubmerged(rag[i, 0]))
                {
                    rag[i, 2] *= Custom.LerpMap(rag[i, 2].magnitude, 1f, 10f, 1f, 0.5f, Mathf.Lerp(1.4f, 0.4f, t));
                    rag[i, 2].y += 0.05f;
                    rag[i, 2] += Custom.RNV() * 0.1f;
                    continue;
                }

                rag[i, 2] *= Custom.LerpMap(Vector2.Distance(rag[i, 0], rag[i, 1]), 1f, 6f, 0.999f, 0.7f, Mathf.Lerp(1.5f, 0.5f, t));
                rag[i, 2].y -= room.gravity * Custom.LerpMap(Vector2.Distance(rag[i, 0], rag[i, 1]), 1f, 6f, 0.6f, 0f);

                if (i % 3 == 2 || i == rag.GetLength(0) - 1)
                {
                    SharedPhysics.TerrainCollisionData cd = scratchTerrainCollisionData.Set(rag[i, 0], rag[i, 1], rag[i, 2], 1f, new IntVector2(0, 0), goThroughFloors: false);
                    cd = SharedPhysics.HorizontalCollision(room, cd);
                    cd = SharedPhysics.VerticalCollision(room, cd);
                    cd = SharedPhysics.SlopesVertically(room, cd);
                    rag[i, 0] = cd.pos;
                    rag[i, 2] = cd.vel;
                    if (cd.contactPoint.x != 0) rag[i, 2].y *= 0.6f;
                    if (cd.contactPoint.y != 0) rag[i, 2].x *= 0.6f;
                }
            }

            for (int j = 0; j < rag.GetLength(0); j++)
            {
                if (j > 0)
                {
                    Vector2 normalized = (rag[j, 0] - rag[j - 1, 0]).normalized;
                    float num = Vector2.Distance(rag[j, 0], rag[j - 1, 0]);
                    float num2 = num > conRad ? 0.5f : 0.25f;
                    rag[j, 0] += normalized * (conRad - num) * num2;
                    rag[j, 2] += normalized * (conRad - num) * num2;
                    rag[j - 1, 0] -= normalized * (conRad - num) * num2;
                    rag[j - 1, 2] -= normalized * (conRad - num) * num2;
                    if (j > 1)
                    {
                        normalized = (rag[j, 0] - rag[j - 2, 0]).normalized;
                        rag[j, 2] += normalized * 0.2f;
                        rag[j - 2, 2] -= normalized * 0.2f;
                    }

                    if (j < rag.GetLength(0) - 1)
                    {
                        rag[j, 3] = Vector3.Slerp(rag[j, 3], (rag[j - 1, 3] * 2f + rag[j + 1, 3]) / 3f, 0.1f);
                        rag[j, 5] = Vector3.Slerp(rag[j, 5], (rag[j - 1, 5] * 2f + rag[j + 1, 5]) / 3f,
                            Custom.LerpMap(Vector2.Distance(rag[j, 1], rag[j, 0]), 1f, 8f, 0.05f, 0.5f));
                    }
                }
                else
                {
                    rag[j, 0] = RagAttachPos(1f);
                    rag[j, 2] *= 0f;
                }
            }


            if (flag && mode != Mode.Thrown && igniteCounter < 1)
            {
                Ignite();
            }

            if (Submersion > 0.2f && room.waterObject.WaterIsLethal && igniteCounter < 1)
            {
                Ignite();
            }

            if (igniteCounter > 0)
            {
                int num3 = igniteCounter;
                igniteCounter++;
                if (stuckInObject == null) igniteCounter++; 

                room.AddObject(new Spark(firstChunk.pos + rotation * 15f, -rotation * Mathf.Lerp(6f, 11f, UnityEngine.Random.value) + Custom.RNV() * UnityEngine.Random.value * 1.5f, explodeColor, null, 8, 18));
                room.MakeBackgroundNoise(0.5f);
                if (miniExplosions.Count > 0 && num3 < miniExplosions[0] && igniteCounter >= miniExplosions[0])
                {
                    miniExplosions.RemoveAt(0);
                    SpearStunArea(this, 85, 5);
                }

                if (igniteCounter > explodeAt && !exploded)
                {
                    SpearStunArea(this, 210, 80);
                    exploded = true;
                }
            }
        }
        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            base.ApplyPalette(sLeaser, rCam, palette); 
            color = palette.blackColor;blueColor = Color.Lerp(new Color(.04f, 0.05f, 1f), palette.blackColor, 0.1f + 0.8f * palette.darkness);
            sLeaser.sprites[0].color = Custom.HSL2RGB(Custom.Decimal(abstractSpear.hue + EggBugGraphics.HUE_OFF), 1f, 0.5f);
            sLeaser.sprites[1].color = blueColor;
            sLeaser.sprites[2].color = blueColor;
            
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            base.InitiateSprites(sLeaser, rCam);
            if (sLeaser.sprites.Length < 3) Array.Resize(ref sLeaser.sprites, 3);
            sLeaser.sprites[0] = new FSprite("FireBugSpear");
            sLeaser.sprites[1] = new FSprite("SpearRag");
            sLeaser.sprites[2] = TriangleMesh.MakeLongMesh(rag.GetLength(0), pointyTip: false, customColor: false);
            sLeaser.sprites[2].shader = rCam.game.rainWorld.Shaders["JaggedSquare"];
            sLeaser.sprites[2].alpha = rCam.game.SeededRandom(abstractPhysicalObject.ID.RandomSeed);
            AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            if (exploded)
            {
                sLeaser.sprites[1].isVisible = false;
                sLeaser.sprites[2].isVisible = false;
                return;
            }
            Vector2 spearPos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
            sLeaser.sprites[1].SetPosition(spearPos - camPos);
            sLeaser.sprites[1].rotation = Custom.VecToDeg(Vector3.Slerp(lastRotation, rotation, timeStacker));

            float num = 0f;
            Vector2 vector = RagAttachPos(timeStacker);
            for (int i = 0; i < rag.GetLength(0); i++)
            {
                float f = i / (float)(rag.GetLength(0) - 1);
                Vector2 vector2 = Vector2.Lerp(rag[i, 1], rag[i, 0], timeStacker);
                float num2 = (2f + 2f * Mathf.Sin(Mathf.Pow(f, 2f) * (float)Math.PI)) * Vector3.Slerp(rag[i, 4], rag[i, 3], timeStacker).x;
                Vector2 normalized = (vector - vector2).normalized;
                Vector2 vector3 = Custom.PerpendicularVector(normalized);
                float num3 = Vector2.Distance(vector, vector2) / 5f;
                (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4, vector - normalized * num3 - vector3 * (num2 + num) * 0.5f - camPos);
                (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 1, vector - normalized * num3 + vector3 * (num2 + num) * 0.5f - camPos);
                (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 2, vector2 + normalized * num3 - vector3 * num2 - camPos);
                (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 3, vector2 + normalized * num3 + vector3 * num2 - camPos);
                vector = vector2;
                num = num2;
            }
        }

        private void SpearStunArea(Spear spear, int stunRadiusPixels, int stunDurationFrames, float violenceStrength = 0f)
        {
            if (spear.room == null) return;

            SpearStunEffects(spear, stunRadiusPixels);

            foreach (var creature in spear.room.abstractRoom.creatures)
            {
                Creature victim = creature.realizedCreature;
                if (victim != null && victim != spear.thrownBy)
                {
                    float dist = Vector2.Distance(spear.firstChunk.pos, victim.mainBodyChunk.pos);
                    if (victim is Player player && player.slugcatStats.name.value == "stunmaster") continue;
                    if (dist < stunRadiusPixels) victim.Violence(spear.firstChunk, null, victim.mainBodyChunk, null, Creature.DamageType.Explosion, violenceStrength, stunDurationFrames);
                }
            }
        }
        internal void SpearStunEffects(Spear spear, int stunRadiusPixels)
        {
            var room = spear.room;
            var pos = spear.firstChunk.pos;
            var color = new Color(0.1f, 0.3f, 1f);

            room.AddObject(new Explosion.ExplosionLight(pos, stunRadiusPixels, 0.2f, 2, color));
            room.AddObject(new Explosion.ExplosionLight(pos, stunRadiusPixels, 0.1f, 1, explodeColor));
            room.AddObject(new ExplosionSpikes(room, pos, stunRadiusPixels, 10f, 3f, 2f, 60f, color));
            room.AddObject(new ShockWave(pos, stunRadiusPixels, 0.02f, 2, false));

            room.ScreenMovement(pos, default, 0.5f);
            room.PlaySound(SoundID.Bomb_Explode, pos, 0.5f, 1f);
            room.InGameNoise(new Noise.InGameNoise(pos, 30f, spear, 0.3f));
        }

    }
}
