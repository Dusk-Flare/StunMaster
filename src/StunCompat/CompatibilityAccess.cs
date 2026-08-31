using System.Linq;

namespace StunMaster.StunCompat
{
    public static class CompatibilityAccess
    {
        private static bool _checkedRainMeadow = false;
        private static bool _isMeadowLoaded = false;
        public static bool IsMeadowLoaded
        {
            get
            {
                if(!_checkedRainMeadow) 
                { 
                    _isMeadowLoaded = ModManager.ActiveMods.Any(x => x.id == "henpemaz_rainmeadow");
                    _checkedRainMeadow = true;
                }
                return _isMeadowLoaded;
            }
        }
        public static void UpdateOnlineStun(Player source, int stunRadius) 
        { 
            if(IsMeadowLoaded) IRainMeadow.UpdateOnlineStun(source, stunRadius);
        }
        public static bool IsArenaBattleEnabled()
        {
            if (IsMeadowLoaded) return IRainMeadow.IsArenaBattleEnabled();
            return false;
        }

        public static bool IsMeadowArena()
        {
            if (IsMeadowLoaded) return IRainMeadow.IsMeadowArena();
            return false;
        }
    }
}
