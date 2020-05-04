using System.Collections.Generic;
using System.Linq;

namespace D2VE
{
    public class ItemInstance
    {
        public ItemInstance(string name, string itemType, string season, SortedDictionary<string, long> stats)
        {
            Name = name;
            ItemType = itemType;
            Season = season;
            Stats = stats;
        }
        public string Name { get; }
        public string ItemType { get; }
        public string Season { get; }
        public SortedDictionary<string, long> Stats { get; }
        public override string ToString()
        {
            return Name + " (" + ItemType + ")" + (string.IsNullOrWhiteSpace(Season) ? "" : "[" + Season + "]")
                + "\r\n  " + string.Join("\r\n  ", Stats.Keys.Zip(Stats.Values, (k, v) => k + " = " + v));
        }
    }
}
