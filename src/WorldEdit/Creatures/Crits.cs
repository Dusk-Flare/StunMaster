using StunMaster.EngeneerDrone;
using System.Collections.Generic;
using static StaticWorld;
using UnityEngine;

namespace StunMaster.WorldEdit.Creatures
{
    public static class Crits
    {
        public static FAtlas DroneIcon;
        public static void InititateDroneAI(On.AbstractCreature.orig_InitiateAI orig, AbstractCreature self)
        {
            orig(self);

            if (self.creatureTemplate.type == Enums.CreatureTemplateType.Drone)
            {
                Plugin.Logger.LogInfo("Drone init ai called");
                self.abstractAI.RealAI = new DroneAI(self, self.world);
            }
        }

        public static void RealizeDrone(On.AbstractCreature.orig_Realize orig, AbstractCreature self)
        {
            if (self.Room == null || self.realizedCreature != null)
            {
                return;
            }

            if (self.creatureTemplate.type == Enums.CreatureTemplateType.Drone)
            {
                Plugin.Logger.LogInfo("Drone realize called");
                self.realizedCreature = new Drone(self, self.world);
            }
            else
            {
                orig(self);
                return;
            }

            self.InitiateAI();
            for (int i = 0; i < self.stuckObjects.Count; i++)
            {
                if (self.stuckObjects[i].A.realizedObject == null)
                {
                    self.stuckObjects[i].A.Realize();
                }

                if (self.stuckObjects[i].B.realizedObject == null)
                {
                    self.stuckObjects[i].B.Realize();
                }
            }
        }

        public static void InitDroneTemplate(On.StaticWorld.orig_InitCustomTemplates orig)
        {
            orig();

            Plugin.Logger.LogInfo("Drone init template called");

            List<TileTypeResistance> tiles = 
            [
                new(AItile.Accessibility.Air, 1, PathCost.Legality.Allowed)
            ];
            List<TileConnectionResistance> resistances = 
            [
                new(MovementConnection.MovementType.Standard, 1f, PathCost.Legality.Allowed),
                new(MovementConnection.MovementType.OpenDiagonal, 1f, PathCost.Legality.Allowed),
                new(MovementConnection.MovementType.CeilingSlope, 1f, PathCost.Legality.Allowed),
                new(MovementConnection.MovementType.ShortCut, 1f, PathCost.Legality.Allowed),
                new(MovementConnection.MovementType.Slope, 1f, PathCost.Legality.Allowed),
                new(MovementConnection.MovementType.BetweenRooms, 1f, PathCost.Legality.Allowed)
            ];
            CreatureTemplate creature = new(Enums.CreatureTemplateType.Drone, null, tiles, resistances, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, 1f))
            {
                waterRelationship = CreatureTemplate.WaterRelationship.Amphibious,
                waterPathingResistance = 3f,
                waterVision = 1f,
                baseDamageResistance = 3.5f,
                baseStunResistance = 2f,
                AI = true,
                canFly = true,
                canSwim = true,
                requireAImap = true,
                doPreBakedPathing = false,
                preBakedPathingAncestor = GetCreatureTemplate(CreatureTemplate.Type.Fly),
                movementBasedVision = 0f,
                dangerousToPlayer = 0f,
                communityID = CreatureCommunities.CommunityID.None,
                countsAsAKill = 1,
                grasps = 0,
                bodySize = .5f,
                visualRadius = 1200f,
                throughSurfaceVision = 1f,
                usesCreatureHoles = false,
                usesNPCTransportation = false,
                usesRegionTransportation = false,
                wormGrassImmune = true,
                jumpAction = "Fly"
            };
            List<CreatureTemplate> list = [creature];

            foreach (CreatureTemplate t in list)
            {
                if (t.type.Index != -1)
                {
                    creatureTemplates[t.type.Index] = t;
                }
            }
        }

        public static void InitRelationships(On.StaticWorld.orig_InitStaticWorldRelationships orig)
        {
            orig();

            Plugin.Logger.LogInfo("Drone init relationship called");
            EstablishRelationship(Enums.CreatureTemplateType.Drone, CreatureTemplate.Type.Scavenger, new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Antagonizes, 0.7f));
        }

        public static void CreatureTemplatector(On.CreatureTemplate.orig_ctor_Type_CreatureTemplate_List1_List1_Relationship orig, CreatureTemplate self, CreatureTemplate.Type type, CreatureTemplate ancestor, List<TileTypeResistance> tileResistances, List<TileConnectionResistance> connectionResistances, CreatureTemplate.Relationship defaultRelationship)
        {
            orig(self, type, ancestor, tileResistances, connectionResistances, defaultRelationship);

            if (type == Enums.CreatureTemplateType.Drone)
            {
                Plugin.Logger.LogInfo("Drone template name called");
                self.name = "EngeneerDrone";
            }
        }

        public static string SpriteNameOfCreature(On.CreatureSymbol.orig_SpriteNameOfCreature orig, IconSymbol.IconSymbolData iconData)
        {
            if (iconData.critType == Enums.CreatureTemplateType.Drone)
            {
                Plugin.Logger.LogInfo("Drone sprite name called");
                return "drone";
            }

            return orig(iconData);
        }

        public static Color ColorOfCreature(On.CreatureSymbol.orig_ColorOfCreature orig, IconSymbol.IconSymbolData iconData)
        {
            if (iconData.critType == Enums.CreatureTemplateType.Drone)
            {
                Plugin.Logger.LogInfo("Drone color called");
                return new Color(1f, 0f, 0f);
            }

            return orig(iconData);
        }
    }
}
