namespace StunMaster.WorldEdit
{
    public static class CloudEffects
    {
        public static void ATAboveCloudsView(On.AboveCloudsView.orig_ctor orig, AboveCloudsView self, Room room, RoomSettings.RoomEffect effect)
        {
            if ((room.world.region != null && room.world.region.name == "AT") || room.abstractRoom.name.StartsWith("AT_"))
            {
                self.startAltitude = 0f;
                self.endAltitude = 36900f;
                orig(self, room, effect);
                self.OEClouds = true;
            }
            else orig(self, room, effect);
        }
        public static void ATshiftView(On.AboveCloudsView.orig_OEshiftView orig, AboveCloudsView self)
        {
            bool room1Loaded = self.room.world.GetAbstractRoom("GATE_SB_OE") != null;
            bool room2Loaded = self.room.world.GetAbstractRoom("OE_RUIN02") != null;
            if (room1Loaded && room2Loaded) orig(self);
        }
    }
}