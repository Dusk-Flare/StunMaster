using StunMaster.WorldEdit.Creatures;

namespace StunMaster.Hooks
{
    static partial class HookManager
    {
        internal static void ApplyCreatures()
        {
            On.CreatureSymbol.ColorOfCreature += Crits.ColorOfCreature;
            On.CreatureSymbol.SpriteNameOfCreature += Crits.SpriteNameOfCreature;

            On.CreatureTemplate.ctor_Type_CreatureTemplate_List1_List1_Relationship += Crits.CreatureTemplatector;

            On.StaticWorld.InitCustomTemplates += Crits.InitDroneTemplate;
            On.StaticWorld.InitStaticWorldRelationships += Crits.InitRelationships;

            On.AbstractCreature.Realize += Crits.RealizeDrone;
            On.AbstractCreature.InitiateAI += Crits.InititateDroneAI;
        }
        internal static void RemoveCreatures()
        {
            On.CreatureSymbol.ColorOfCreature -= Crits.ColorOfCreature;
            On.CreatureSymbol.SpriteNameOfCreature -= Crits.SpriteNameOfCreature;

            On.CreatureTemplate.ctor_Type_CreatureTemplate_List1_List1_Relationship -= Crits.CreatureTemplatector;

            On.StaticWorld.InitCustomTemplates -= Crits.InitDroneTemplate;
            On.StaticWorld.InitStaticWorldRelationships -= Crits.InitRelationships;

            On.AbstractCreature.Realize -= Crits.RealizeDrone;
            On.AbstractCreature.InitiateAI -= Crits.InititateDroneAI;
        }
    }
}
