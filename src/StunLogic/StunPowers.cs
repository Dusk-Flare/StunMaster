using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using StunMaster.StunObjects;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            //IL.Player.CraftingResults += StunCraftingResults;
            //IL.Player.SpitUpCraftedObject += StunSpitUpCraftedObject;
            Plugin.Logger.LogInfo("Applied Stun Master crafting hooks.");
        }
        private static bool TryNext(ILCursor cursor)
        {
            return cursor.MoveToNextSlugcat(typeof(MoreSlugcatsEnums.SlugcatStatsName).GetField(nameof(MoreSlugcatsEnums.SlugcatStatsName.Spear)));
        }
        internal static void ImplementILCodeAssumingLdarg0(this ILCursor cursor, Delegate implementation)
        {
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate(implementation);
        }
        internal static bool MoveToNextSlugcat(this ILCursor cursor, FieldInfo info, [CallerMemberName] string method = "")
        {
            try
            {
                Func<Instruction, bool> isSlugcat = info.IsStatic ? (x => x.MatchLdsfld(info)) : x => x.MatchLdfld(info);
                if (cursor.TryGotoNext(MoveType.After,
                    isSlugcat,
                    x => x.MatchCallOrCallvirt(out _)))
                {
                    cursor.MoveAfterLabels();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogInfo($"Failed MoveToNextSlugcat patch: {ex}");
            }
            return false;
        }
        private static bool ExplosiveCraft(bool result, Player player)
        {
            return result || player.SlugCatClass.value == Plugin.stunMasterID;
        }
        private static void StunCraftingResults(ILContext il)
        {
            try
            {
                ILCursor cursor = new(il);

                if (TryNext(cursor))
                {
                    cursor.ImplementILCodeAssumingLdarg0(ExplosiveCraft);

                    if (cursor.TryGotoNext(x => x.MatchLdarg(0),
                        x => x.MatchCallOrCallvirt(typeof(Player).GetProperty(nameof(Player.FoodInStomach)).GetGetMethod())))
                    {
                        ILCursor jumpCursor = cursor.Clone();

                        jumpCursor.GotoNext(MoveType.After, x => x.MatchLdcI4(0));
                        ILLabel jumpLabel = jumpCursor.Next.Operand as ILLabel;

                        cursor.MoveAfterLabels();
                        cursor.Emit(OpCodes.Ldarg_0);
                        static bool CanAffordCost(Player player)
                        {
                            return player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Artificer || (player.SlugCatClass.value == Plugin.stunMasterID) && player.FoodInStomach >= CraftCost;
                        }
                        cursor.EmitDelegate(CanAffordCost);
                        cursor.Emit(OpCodes.Brfalse, jumpLabel);
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.Logger.LogInfo($"Failed ILStunCraftingResults patch: {e}");
            }
        }

        private static void StunSpitUpCraftedObject(ILContext il)
        {
            try
            {
                ILCursor cursor = new(il);

                if (cursor.TryGotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchLdcI4(1),
                    x => x.MatchCallOrCallvirt<Player>(nameof(Player.SubtractFood))
                ))
                {
                    ILCursor jumpCursor = cursor.Clone();
                    jumpCursor.GotoNext(MoveType.After,
                        x => x.MatchCallOrCallvirt<Player>(nameof(Player.SubtractFood)));

                    jumpCursor.Emit(OpCodes.Ldarg_0);
                    ILLabel jumpIfNotArti = jumpCursor.MarkLabel();

                    cursor.Emit(OpCodes.Ldarg_0);
                    static bool isNotArtiOrStun(Player player)
                    {
                        return player.SlugCatClass != MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Artificer
                            && player.SlugCatClass.value != Plugin.stunMasterID;
                    }
                    cursor.EmitDelegate(isNotArtiOrStun);
                    cursor.Emit(OpCodes.Brtrue, jumpIfNotArti);
                }

                if (cursor.TryGotoNext(
                    x => x.MatchLdsfld<AbstractPhysicalObject.AbstractObjectType>(
                        nameof(AbstractPhysicalObject.AbstractObjectType.Spear))
                ))
                {
                    cursor.MoveAfterLabels();

                    cursor.Emit(OpCodes.Ldarg_0);
                    cursor.EmitDelegate<Func<AbstractPhysicalObject.AbstractObjectType, Player, AbstractPhysicalObject.AbstractObjectType>>(
                        (origType, player) =>
                        {
                            if (player.SlugCatClass.value == Plugin.stunMasterID && origType == AbstractPhysicalObject.AbstractObjectType.Spear)
                            {
                                return Plugin.stunSpear;
                            }
                            return origType;
                        });
                }

                Plugin.Logger.LogInfo("IL patch applied: StunMaster can craft stun spears! Identifier");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }


        private static AbstractPhysicalObject.AbstractObjectType Player_CraftingResults(On.Player.orig_CraftingResults orig, Player self)
        {
            Plugin.Logger.LogInfo($"Checking crafting results for {self.SlugCatClass.value}.");

            // Only apply for your custom slugcat
            if (self.SlugCatClass.value == Plugin.stunMasterID)
            {
                Plugin.Logger.LogInfo("Player is Stun Master.");

                // Require enough food
                if (self.FoodInStomach < CraftCost)
                {
                    Plugin.Logger.LogInfo("Player does not have enough food to craft.");
                    return null;
                }

                // Prevent crafting while holding edible items
                Creature.Grasp[] grasps = self.grasps;
                for (int i = 0; i < grasps.Length; i++)
                {
                    if (grasps[i]?.grabbed is IPlayerEdible edible && edible.Edible)
                    {
                        Plugin.Logger.LogInfo("Found edible item in grasp, cannot craft.");
                        return null;
                    }
                }

                Plugin.Logger.LogInfo("No edible items found in grasps.");

                // Look for a normal spear to convert
                for (int i = 0; i < grasps.Length; i++)
                {
                    PhysicalObject grabObj = grasps[i]?.grabbed;
                    if (grabObj is Spear && grabObj is not StunSpear)
                    {
                        Plugin.Logger.LogInfo($"Crafting stun spear from grasp {i}.");

                        // Destroy the consumed spear
                        grabObj.Destroy();
                        grasps[i].Release();

                        // Create the abstract stun spear
                        var stunSpearAbs = new AbstractStunSpear(self.room.world, self.room.GetWorldCoordinate(self.firstChunk.pos), self.room.game.GetNewID());

                        // Put it in the player’s stomach for spitting up
                        self.objectInStomach = stunSpearAbs;

                        Plugin.Logger.LogInfo("Assigned stun spear to player’s stomach.");
                        return Plugin.stunSpear;
                    }
                }

                Plugin.Logger.LogInfo("No valid base spear found to craft.");
                return null;
            }

            // Not the Stun Master → fall back to normal logic
            Plugin.Logger.LogInfo("Player is not Stun Master, using original crafting results.");
            return orig(self);
        }

        private static void Player_SpitUpCraftedObject(On.Player.orig_SpitUpCraftedObject orig, Player self)
        {
            Plugin.Logger.LogInfo($"Spitting up crafted object for {self.SlugCatClass.value}.");

            // Handle our custom spear type
            if (self.objectInStomach != null && self.objectInStomach.type == Plugin.stunSpear)
            {
                Plugin.Logger.LogInfo("Spitting up stun spear.");

                // Make sure the object is realized
                if (self.objectInStomach.realizedObject == null)
                    self.objectInStomach.RealizeInRoom();

                var stunSpear = self.objectInStomach.realizedObject;
                self.room.AddObject(stunSpear);
                stunSpear.firstChunk.HardSetPosition(self.firstChunk.pos);

                // Consume food
                if (self.FoodInStomach >= CraftCost)
                {
                    Plugin.Logger.LogInfo($"Consuming {CraftCost} food to spit up stun spear.");
                    self.SubtractFood(CraftCost);
                }

                self.objectInStomach = null;
                return;
            }

            // Default behavior for other cases
            orig(self);
        }
        private static bool CraftCheck(On.Player.orig_GraspsCanBeCrafted orig, Player self)
        {
            Plugin.Logger.LogInfo($"Checking if grasps can be crafted for {self.SlugCatClass.value}.");
            if (self.SlugCatClass.value == Plugin.stunMasterID)
            {
                Plugin.Logger.LogInfo("Player is Stun Master.");
                return self.CraftingResults() != null;
            }
            return orig(self);
        }
    }
}
