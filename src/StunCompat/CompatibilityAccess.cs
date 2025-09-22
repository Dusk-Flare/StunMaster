using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StunMaster.StunCompat
{
    class CompatibilityAccess
    {
        public static bool IsRainMeadowLoaded()
        {
            try
            {
                return ModManager.ActiveMods.Any(x => x.id == "henpemaz_rainmeadow");
            }
            catch
            {
                return false;
            }
        }
        public static bool IsArenaBattleEnabled()
        {
            try
            {
                RainMeadow.RainMeadow.isArenaMode(out var arena);
                return !arena.countdownInitiatedHoldFire;
            }
            catch 
            { 
                return false; 
            }
        }

        public static bool IsMeadowArena()
        {
            try
            {
                return RainMeadow.RainMeadow.isArenaMode(out _);
            }
            catch
            {
                return false;
            }
        }

        public static void PassOnlineData()
        {
            try
            {
                RainMeadow.Lobby lobby = RainMeadow.OnlineManager.lobby;
                if (lobby != null)
                {
                    OnlineData onlineData = new();
                    lobby.AddData(onlineData);
                }
            }
            catch (Exception except)
            {
                RainMeadow.RainMeadow.Error(except);
            }
        }

        public static ReturnData GetOnlineData()
        {
            ReturnData returnData = new();
            try
            {
                RainMeadow.Lobby lobby = RainMeadow.OnlineManager.lobby;
                if (lobby != null)
                {
                    lobby.TryGetData(out OnlineData data);
                    if (data != null)
                    {
                        returnData.LegacyPowers = data.Legacy;
                    }
                }
            }
            catch (Exception except)
            {
                RainMeadow.RainMeadow.Error(except);
            }
            return returnData;
        }
    }
}
