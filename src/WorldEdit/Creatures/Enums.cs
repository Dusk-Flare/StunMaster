namespace StunMaster.WorldEdit.Creatures
{
    public static class Enums
    {
        public static void Register()
        {
            CreatureTemplateType.Register();

            SandboxUnlockID.Register();
        }

        public static void UnRegister()
        {
            CreatureTemplateType.UnRegister();

            SandboxUnlockID.UnRegister();
        }
		
        public static class CreatureTemplateType
        {
            public static CreatureTemplate.Type Drone;

            public static void Register()
            {
                Plugin.Logger.LogInfo("Drone register called");
                Drone = new CreatureTemplate.Type("EngeneerDrone", true);
            }

            public static void UnRegister()
            {
                Drone?.Unregister();
                Drone = null;
            }
        }

        public static class SandboxUnlockID
        {
            public static MultiplayerUnlocks.SandboxUnlockID Drone;

            public static void Register()
            {
                Plugin.Logger.LogInfo("Drone sandbox register called");
                Drone = new MultiplayerUnlocks.SandboxUnlockID("EngeneerDrone", true);
            }

            public static void UnRegister()
            {
                Drone?.Unregister();
                Drone = null;
            }

            public static void RegisterUnlockLists()
            {
                Plugin.Logger.LogInfo("Drone unlock register called");
                if (!MultiplayerUnlocks.CreatureUnlockList.Contains(Drone)) MultiplayerUnlocks.CreatureUnlockList.Add(Drone);
            }
        }
    }
}
