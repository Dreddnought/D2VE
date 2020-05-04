using System.Collections.Generic;
using System.Linq;

namespace D2VE
{
    public class ItemInstance
    {
        public ItemInstance(ItemInfo itemInfo, long power, string energyType, SortedDictionary<string, long> stats)
        {
            Name = itemInfo.Name;
            ItemType = itemInfo.ItemType;
            Slot = itemInfo.Slot;
            Season = itemInfo.Season;
            ClassType = itemInfo.ClassType;
            Power = power;
            EnergyType = energyType;
            Stats = stats;
        }
        public string Name { get; }
        public long Power { get; }
        public string ItemType { get; }
        public string Slot { get; }
        public string EnergyType { get; }
        public string Season { get; }
        public string ClassType { get; }
        public SortedDictionary<string, long> Stats { get; }
        public override string ToString()
        {
            return Name + " " + Power.ToString() + " (" + ItemType + ") " + Slot + " " + EnergyType
                + (string.IsNullOrWhiteSpace(ClassType) ? "" : " [" + ClassType + "]")
                + (string.IsNullOrWhiteSpace(Season) ? "" : " [" + Season + "]")
                + "\r\n  " + string.Join("\r\n  ",
                Stats.Keys.Zip(Stats.Values, (k, v) => ConvertValue.StatSortedName(k) + " = " + v));
        }
    }
}
