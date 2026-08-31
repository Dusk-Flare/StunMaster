using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StunMaster.Utils
{
    public record struct StunCooldowns(Dictionary<Player, float> Cooldowns) : IEnumerable<StunCooldowns.StunEntry>
    {
        public record struct StunEntry(KeyValuePair<Player, float> Entry)
        {
            public Player Player => Entry.Key;
            public float CoolDown => Entry.Value;
            public static implicit operator StunEntry(KeyValuePair<Player, float> entry) => new(entry);
            public static implicit operator KeyValuePair<Player, float>(StunEntry entry) => entry.Entry;
        }
        public StunCooldowns() : this([]) { }
        public bool Contains(Player player) => Cooldowns.ContainsKey(player);

        public readonly float this[Player player]
        {
            get => Cooldowns[player];
            set => Cooldowns[player] = value;
        }
        public bool TryGetStun(Player player, out float stunTrime) => Cooldowns.TryGetValue(player, out stunTrime);
        public void Stun(Player player, float stunTime)
        {
            if(Contains(player)) Cooldowns.Remove(player);
            Cooldowns.Add(player, stunTime);
            Plugin.Logger.LogInfo($"Stun {player} set to {stunTime}");
        }

        public void Update(Player player)
        {
            if (!Contains(player)) return;
            if (Cooldowns[player] > 0) Plugin.Logger.LogInfo(Cooldowns[player]);
            if (Cooldowns[player] != 0)  Cooldowns[player] = Mathf.Max(0, Cooldowns[player] - 1);
            else Cooldowns.Remove(player);
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
