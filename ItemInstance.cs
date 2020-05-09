using System.Collections.Generic;
using System.Linq;

namespace D2VE
{
    public class ItemInstance
    {
        public ItemInstance(ItemInfo itemInfo, long power, string energyType, SortedDictionary<string, long> stats,
               SortedDictionary<string, Perk> perks)
        {
            ItemInfo = itemInfo;
            Power = power;
            EnergyType = energyType;
            Stats = stats;
            Perks = perks;
        }
        public ItemInfo ItemInfo { get; }
        public string Name { get { return ItemInfo.Name; } }
        public long Power { get; }
        public string ItemCategory { get { return ItemInfo.ItemCategory; } }
        public string TierType { get { return ItemInfo.TierType; } }
        public string ItemType { get { return ItemInfo.ItemType; } }
        public string Slot { get { return ItemInfo.Slot; } }
        public string Season { get { return ItemInfo.Season; } }
        public string ClassType { get { return ItemInfo.ClassType; } }
        public string EnergyType { get; }
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
