using StunMaster.StunCompat;
using StunMaster.Utils;
using UnityEngine;

namespace StunMaster.StunCat
{
    public static class LegacyStun
    {
        public static void TriggerStoryStun(this StunPlayer self, int stunRadius, int stunDuration)
        {
            int stunDurationFrames = stunDuration * 40;
            int stunRadiusPixels = stunRadius * 20;

            if (self.Instance.FoodInStomach < 1) return;
            self.Instance.SubtractFood(1);
            StunArea(self, stunRadiusPixels, stunDurationFrames);
        }
        public static void TriggerArenaStun(this StunPlayer self, int stunRadius, int stunCooldown, int stunDuration)
        {
            int stunDurationFrames = stunDuration * 40;
            float stunCooldownFrames = stunCooldown * 40f;
            int stunRadiusPixels = stunRadius * 20;

            if (StunInputs.StunEntries.GetOrStun(self, 0) > 0) return;
            if (CompatibilityAccess.IsMeadowArena() && !CompatibilityAccess.IsArenaBattleEnabled()) return;
            StunInputs.StunEntries.Stun(self, stunCooldownFrames);
            StunArea(self, stunRadiusPixels, stunDurationFrames);
        }

        public static void StunArea(Player self, int stunRadiusPixels, int stunDurationFrames, float violenceStrength = 0f)
        {
            StunEffects(self, stunRadiusPixels);
            foreach (AbstractCreature creature in self.room.abstractRoom.creatures)
            {
                Creature victim = creature.realizedCreature;
                if (victim != null && creature.realizedCreature != self)
                {
                    float dist = Vector2.Distance(self.mainBodyChunk.pos, victim.mainBodyChunk.pos);
                    if (dist < stunRadiusPixels)
                    {
                        if (victim is Player player && player.slugcatStats.name.value == PluginData.stunMasterID) continue;
                        victim.Violence(self.mainBodyChunk, null, victim.mainBodyChunk, null, Creature.DamageType.Explosion, violenceStrength, stunDurationFrames);
                    }
                }
            }
        }

        public static void StunEffects(Player self, int stunRadiusPixels)
        {
            Room room = self.room;
            Vector2 pos = self.mainBodyChunk.pos;
            Color color = self.ShortCutColor();
            room.AddObject(new Explosion.ExplosionLight(pos, stunRadiusPixels, 0.2f, 2, color));
            room.AddObject(new Explosion.ExplosionLight(pos, stunRadiusPixels, 0.1f, 1, color));
            room.AddObject(new ExplosionSpikes(room, pos, stunRadiusPixels, 10f, 3f, 2f, 60f, color));
            room.AddObject(new ShockWave(pos, stunRadiusPixels, 0.02f, 2, false));

            room.ScreenMovement(pos, default, 0.5f);
            room.PlaySound(SoundID.Bomb_Explode, pos, 0.5f, 1f);
            room.InGameNoise(new Noise.InGameNoise(pos, 30f, self, 0.3f));
        }
    }
}
