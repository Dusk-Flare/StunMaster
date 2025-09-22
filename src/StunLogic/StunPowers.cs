using MoreSlugcats;
using UnityEngine;

namespace StunMaster.StunLogic
{
    internal static class StunPowers
    {
        private const int CraftCost = 1;

        internal static void Apply()
        {
            On.Player.GraspsCanBeCrafted += CraftCheck;
            On.Player.CraftingResults += Player_CraftingResults;
            On.Player.SpitUpCraftedObject += Player_SpitUpCraftedObject;
        }
        private static AbstractPhysicalObject.AbstractObjectType Player_CraftingResults(On.Player.orig_CraftingResults orig, Player self)
        {
            if (self.SlugCatClass.value == Plugin.stunMasterID)
            {
                if (self.FoodInStomach > 0)
                {
                    Creature.Grasp[] array = self.grasps;
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != null && array[i].grabbed is IPlayerEdible && (array[i].grabbed as IPlayerEdible).Edible)
                        {
                            return null;
                        }
                    }

                    if (array[0] != null && array[0].grabbed is Spear && !(array[0].grabbed as Spear).abstractSpear.explosive)
                    {
                        return Plugin.stunSpear;
                    }

                    if (array[0] == null && array[1] != null && array[1].grabbed is Spear && !(array[1].grabbed as Spear).abstractSpear.explosive && self.objectInStomach == null)
                    {
                        return Plugin.stunSpear;
                    }
                }

                return null;
            }
            return orig(self);
        }
        private static void Player_SpitUpCraftedObject(On.Player.orig_SpitUpCraftedObject orig, Player self)
        {
            if (self.objectInStomach != null && self.objectInStomach.type == Plugin.stunSpear)
            {
                if (self.objectInStomach.realizedObject == null) self.objectInStomach.RealizeInRoom();

                var stunSpear = self.objectInStomach.realizedObject;
                self.room.AddObject(stunSpear);
                stunSpear.firstChunk.HardSetPosition(self.firstChunk.pos);

                if (self.FoodInStomach > 0)
                {
                    self.SubtractFood(CraftCost);
                }
                self.objectInStomach = null;
                return;
            }
            orig(self);
        }
        private static bool CraftCheck(On.Player.orig_GraspsCanBeCrafted orig, Player self)
        {
            if (self.SlugCatClass.value == Plugin.stunMasterID)
            {
                return self.CraftingResults() != null;
            }
            return orig(self);
        }
    }
}
