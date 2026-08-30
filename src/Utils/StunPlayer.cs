namespace StunMaster.Utils
{
    public class StunPlayer
    {
        private Player _instance;
        public Player Instance
        {
            get => _instance;
            set => _instance = value;
        }
        public Player.InputPackage CurrentInput => _instance.input[0];
        public Player.InputPackage PreviousInput => _instance.input[1];
        public Room Room => _instance.room;
        public SlugcatStats Stats => _instance.slugcatStats;

        public StunPlayer(Player player) => Instance = player;

        public static implicit operator Player(StunPlayer player) => player._instance;
        public static implicit operator StunPlayer(Player player) => new(player);
    }
}
