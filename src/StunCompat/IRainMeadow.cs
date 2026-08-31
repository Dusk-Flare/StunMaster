using StunMaster.StunCat;
using RainMeadow;
using System;

namespace StunMaster.StunCompat
{
    public static class IRainMeadow
    {
        public static bool IsArenaBattleEnabled()
        {
            RainMeadow.RainMeadow.isArenaMode(out ArenaOnlineGameMode arena);
            return !arena.countdownInitiatedHoldFire;
        }

        public static bool IsMeadowArena() => RainMeadow.RainMeadow.isArenaMode(out _);

        [RPCMethod]
        public static void ReceiveStunUpdate(OnlineCreature creature, int stunRadius)
        {
            try
            {
                if (creature.abstractCreature?.realizedCreature is not Player sender) return;
                LegacyStun.StunEffects(sender, stunRadius);
            }
            catch(Exception e)
            {
                RainMeadow.RainMeadow.Error(e);
            }
        }
        public static void UpdateOnlineStun(Player source, int stunRadius)
        {
            try
            {
                OnlineCreature creature = source.abstractCreature.GetOnlineCreature();
                OnlineManager.mePlayer?.InvokeRPC(ReceiveStunUpdate, [creature, stunRadius]);
            }
            catch(Exception e)
            {
                RainMeadow.RainMeadow.Error(e);
            }
}

        public static void PassOnlineData()
        {
            try
            {
                OnlineManager.lobby?.AddData(new OnlineData());
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
                Lobby lobby = OnlineManager.lobby;
                if (lobby == null) return returnData;
                if(lobby.TryGetData(out OnlineData data) &&  data != null) returnData.LegacyPowers = data.Legacy;
            }
            catch (Exception except)
            {
                RainMeadow.RainMeadow.Error(except);
            }
            return returnData;
        }
    }
}
