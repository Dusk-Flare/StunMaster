using UnityEngine;

namespace StunMaster
{
    internal class StunPower
    {
        internal static void TriggerStoryStun(Player self, int stunRadius, int stunDuration)
        {
            int stunDurationFrames = stunDuration * 40;
            int stunRadiusPixels = stunRadius * 20;

            if (self.FoodInStomach < 1) return;
            self.SubtractFood(1);
            StunEffects(self, stunRadiusPixels);

            foreach (var creature in self.room.abstractRoom.creatures)
            {
                if (creature.realizedCreature != null && creature.realizedCreature != self)
                {
                    float dist = Vector2.Distance(self.mainBodyChunk.pos, creature.realizedCreature.mainBodyChunk.pos);

                    if (dist < stunRadiusPixels)
                    {
                        if (creature.realizedCreature is Player player && (player.slugcatStats.name.value == "stunmaster")) return;
                        creature.realizedCreature.Violence(self.mainBodyChunk, null, null, null, Creature.DamageType.Explosion, 0f, stunDurationFrames);
                    }
                }
            }
        }
        internal static void TriggerArenaStun(Player self, int stunRadius, int stunCooldown, int stunDuration)
        {
            int stunDurationFrames = stunDuration * 40;
            float stunCooldownFrames = stunCooldown * 40f;
            int stunRadiusPixels = stunRadius * 20;

            if (Plugin.stunCooldowns[self] > 0) return;
            Plugin.stunCooldowns[self] = stunCooldownFrames;
            StunEffects(self, stunRadiusPixels);

            foreach (var creature in self.room.abstractRoom.creatures)
            {
                if (creature.realizedCreature != null && creature.realizedCreature != self)
                {
                    float dist = Vector2.Distance(self.mainBodyChunk.pos, creature.realizedCreature.mainBodyChunk.pos);

                    if (dist < stunRadiusPixels)
                    {
                        if (creature.realizedCreature is Player player && (player.slugcatStats.name.value == "stunmaster")) return;
                        creature.realizedCreature.Violence(self.mainBodyChunk, null, null, null, Creature.DamageType.Explosion, 0f, stunDurationFrames);
                    }
                }
            }
        }
        internal static void StunEffects(Player self, int stunRadiusPixels)
        {
            var room = self.room;
            var pos = self.mainBodyChunk.pos;
            var color = self.ShortCutColor();
            room.AddObject(new Explosion.ExplosionLight(pos, stunRadiusPixels, 0.2f, 2, color));
            room.AddObject(new Explosion.ExplosionLight(pos, stunRadiusPixels, 0.1f, 1, new Color(1f, 1f, 1f)));
            room.AddObject(new ExplosionSpikes(room, pos, stunRadiusPixels, 10f, 3f, 2f, 60f, color));
            room.AddObject(new ShockWave(pos, stunRadiusPixels, 0.02f, 2, false));

            room.ScreenMovement(pos, default, 0.5f);
            room.PlaySound(SoundID.Bomb_Explode, pos, 0.5f, 1f);
            room.InGameNoise(new Noise.InGameNoise(pos, 30f, self, 0.3f));
        }
    }
}
