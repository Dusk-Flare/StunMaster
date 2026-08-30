using System.Collections;
using System.Collections.Generic;

namespace StunMaster.Utils
{
    public record struct StunCooldowns(Dictionary<StunPlayer, float> Cooldowns) : IEnumerable<StunCooldowns.StunEntry>
    {
        public record struct StunEntry(KeyValuePair<StunPlayer, float> Entry)
        {
            public StunPlayer Player => Entry.Key;
            public float CoolDown => Entry.Value;
            public static implicit operator StunEntry(KeyValuePair<StunPlayer, float> entry) => new(entry);
            public static implicit operator KeyValuePair<StunPlayer, float>(StunEntry entry) => entry.Entry;
        }
        public StunCooldowns() : this([]) { }
        public bool Contains(StunPlayer player) => Cooldowns.ContainsKey(player);

        public readonly float this[Player player]
        {
            get => Cooldowns[player];
            set
            {
                Cooldowns[player] = value;
            }
        }
        public bool TryGetStun(StunPlayer player, out float stunTrime) => Cooldowns.TryGetValue(player, out stunTrime);
        public void Stun(StunPlayer player, float stunTime) => Cooldowns[player] = stunTime;
        public float GetOrStun(StunPlayer player, float stunTime)
        {
            if (!TryGetStun(player, out float stunTicks) || stunTicks <= 0) Stun(player, stunTime);
            return Cooldowns[player];
        }

        public void Update(StunPlayer player)
        {
            if (GetOrStun(player, 0) > 0) Cooldowns[player]--;
        }

        public void Update()
        {
            foreach (StunPlayer player in Cooldowns.Keys) Update(player);
        }

        public IEnumerator<StunEntry> GetEnumerator()
        {
            foreach (StunEntry entry in Cooldowns)
            {
                yield return entry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public class FAtlasIndex() : IEnumerable<KeyValuePair<int, int>>
    {
        public int this[int ogIndex] 
        { 
            get => Original[ogIndex]; 
            set => Original[ogIndex] = value;
        }
        public Dictionary<int, int> Original = [];

        public IEnumerator<KeyValuePair<int, int>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<int, int>>)Original).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Original).GetEnumerator();
        }
    }
}
