using System.Collections.Generic;
using System.Linq;

namespace D2VE
{
    public class ItemInstance
    {
        public ItemInstance(ItemInfo itemInfo, long power, string energyType, SortedDictionary<string, long> stats,
               SortedDictionary<string, Perk> perks)
        {
            Name = itemInfo.Name;
            TierType = itemInfo.TierType;
            ItemType = itemInfo.ItemType;
            Slot = itemInfo.Slot;
            Season = itemInfo.Season;
            ClassType = itemInfo.ClassType;
            Power = power;
            EnergyType = energyType;
            Stats = stats;
            Perks = perks;
        }
        public string Name { get; }
        public long Power { get; }
        public string TierType { get; }
        public string ItemType { get; }
        public string Slot { get; }
        public string EnergyType { get; }
        public string Season { get; }
        public string ClassType { get; }
        public SortedDictionary<string, long> Stats { get; }
        public SortedDictionary<string, Perk> Perks { get; }
        public override string ToString()
        {
            return Name + " " + Power.ToString() + " (" + ItemType + ") " + Slot + " " + EnergyType + " " + TierType
                + (string.IsNullOrWhiteSpace(ClassType) ? "" : " [" + ClassType + "]")
                + (string.IsNullOrWhiteSpace(Season) ? "" : " [" + Season + "]")
                + "\r\n  " + string.Join("\r\n  ",
                Stats.Keys.Zip(Stats.Values, (k, v) => ConvertValue.StatSortedName(k) + " = " + v))
                + "\r\n    " + string.Join("\r\n    ",
                Perks.Keys.Zip(Perks.Values, (k, v) => k + " = " + v));
        }
    }
}
