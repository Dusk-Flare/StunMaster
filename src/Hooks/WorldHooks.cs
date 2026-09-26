using StunMaster.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StunMaster.Hooks
{
    static partial class HookManager
    {
        internal static void ApplyWorld()
        {
            On.Room.Loaded += RegisterEffect.RoomLoaded;
            //On.AboveCloudsView.ctor += CloudEffects.ATAboveCloudsView;
            //On.AboveCloudsView.OEshiftView +=  CloudEffects.ATshiftView;
            On.RoomSettings.RoomEffect.GetSliderCount += RegisterEffect.GetSliderCount;
            On.RoomSettings.RoomEffect.GetSliderName += RegisterEffect.GetSliderName;
            On.RoomSettings.RoomEffect.GetSliderDefault += RegisterEffect.GetSliderDefault;
        }
        internal static void RemoveWorld()
        {
            On.Room.Loaded -= RegisterEffect.RoomLoaded;
            //On.AboveCloudsView.ctor -= CloudEffects.ATAboveCloudsView;
            //On.AboveCloudsView.OEshiftView -= CloudEffects.ATshiftView;
            On.RoomSettings.RoomEffect.GetSliderCount -= RegisterEffect.GetSliderCount;
            On.RoomSettings.RoomEffect.GetSliderName -= RegisterEffect.GetSliderName;
            On.RoomSettings.RoomEffect.GetSliderDefault -= RegisterEffect.GetSliderDefault;
        }
    }
}
