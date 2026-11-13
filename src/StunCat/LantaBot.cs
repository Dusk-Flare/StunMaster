using MoreSlugcats;
using UnityEngine;

namespace StunMaster.StunCat
{
    public class LantaBot
    {
        public static void DroneUpdate(Player self)
        {
            AbstractCreature firstAlivePlayer = self?.room?.game?.FirstAlivePlayer;
            if (self.room != null && self.room.game.session is StoryGameSession && ((self.AI == null && (self.room.game.session as StoryGameSession).saveState.hasRobo) || (self.AI != null && (self.playerState as PlayerNPCState).Drone)) && (self.myRobot == null || self.myRobot.slatedForDeletetion) && (!ModManager.CoopAvailable || (firstAlivePlayer != null && firstAlivePlayer.realizedCreature != null && firstAlivePlayer.realizedCreature == self)))
            {
                self.timeWithoutRobot++;
                if (self.timeWithoutRobot >= 10)
                {
                    self.myRobot = new AncientBot(self.mainBodyChunk.pos, new Color(1f, 0f, 0f), self, true);
                    self.room.AddObject(self.myRobot);
                }
            }
            else
            {
                self.timeWithoutRobot = 0;
            }
        }
    }
}
