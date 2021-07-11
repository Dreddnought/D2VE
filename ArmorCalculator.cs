using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2VE
{
    public class Armor
    {
        public Armor(string name, string classType, string tierType, string itemType, string energyType,
            long energyCapacity, long powerCap,
            long mobility, long resilience, long recovery, long discipline, long intellect, long strength)
        {
            Name = name;
            ClassType = classType;
            TierType = tierType;
            ItemType = itemType;
            EnergyType = energyType;
            EnergyCapacity = energyCapacity;
            PowerCap = powerCap;
            Mobility = mobility;
            Resilience = resilience;
            Recovery = recovery;
            Discipline = discipline;
            Intellect = intellect;
            Strength = strength;
            BaseStats = mobility + resilience + recovery + discipline + intellect + strength;
            Mrr = mobility + resilience + recovery;
            Dis = discipline + intellect + strength;
            RI = recovery + intellect;
            Id = Name + " " + BaseStats.ToString() + "(" + RI.ToString() + ")="
                + Mrr.ToString() + "+" + Dis.ToString()
                + " (" + Mobility.ToString() + "-" + Resilience.ToString() + "-" + Recovery.ToString()
                + "-" + Discipline.ToString() + "-" + Intellect.ToString() + "-" + Strength.ToString() + ")"
                + (EnergyCapacity == 10L ? " " + EnergyType : "");
        }
        public string Name { get; }
        public string ClassType { get; }
        public string TierType { get; }
        public string ItemType { get; }
        public string EnergyType { get; }
        public long EnergyCapacity { get; }
        public long PowerCap { get; }
        public long Mobility { get; }
        public long Resilience { get; }
        public long Recovery { get; }
        public long Discipline { get; }
        public long Intellect { get; }
        public long Strength { get; }
        public long BaseStats { get; }
        public long Mrr { get; }
        public long Dis { get; }
        public long RI { get; }
        public string Id { get; }
        public override string ToString() { return Id; }
    }
    public class ArmorCalculator
    {
        public ArmorCalculator(string name, string classType, string exotic, long powerCap = 0L,
            long mobilityMod = 0L, long resilienceMod = 0L, long recoveryMod = 0L,
            long disciplineMod = 0L, long intellectMod = 0L, long strengthMod = 0L)
        {
            Name = name;
            ClassType = classType;
            Exotic = exotic;
            PowerCap = powerCap;
            MobilityMod = mobilityMod;
            ResilienceMod = resilienceMod;
            RecoveryMod = recoveryMod;
            DisciplineMod = disciplineMod;
            IntellectMod = intellectMod;
            StrengthMod = strengthMod;
        }
        /// <summary>Name for the calculation.</summary>
        public string Name { get; }
        /// <summary>Class type (Warlock, Hunter or Titan).</summary>
        public string ClassType { get; }
        /// <summary>Exotic name to base the build on.</summary>
        public string Exotic { get; }
        /// <summary>Power cap to restrict to. (Optional)</summary>
        public long PowerCap { get; }
        /// <summary>Expected mobility mod value, e.g., 25 (20 for Powerful Friends, 5 for Traction. (Optional)</summary>
        public long MobilityMod { get; }
        /// <summary>Expected resilence mod value. (Optional)</summary>
        public long ResilienceMod { get; }
        public long RecoveryMod { get; }
        public long DisciplineMod { get; }
        public long IntellectMod { get; }
        public long StrengthMod { get; }
        public Category Calculate(List<Armor> armorItems)
        {
            Category category = new Category(Name);
            category.ColumnIndex("Name");
            category.ColumnIndex("Helmet");
            category.ColumnIndex("Gauntlets");
            category.ColumnIndex("Chest Armor");
            category.ColumnIndex("Leg Armor");
            category.ColumnIndex("Mobility");
            category.ColumnIndex("Resilience");
            category.ColumnIndex("Recovery");
            category.ColumnIndex("Discipline");
            category.ColumnIndex("Intellect");
            category.ColumnIndex("Strength");
            category.ColumnIndex("Usage");
            category.ColumnIndex("Wastage");
            category.ColumnIndex("TotalExcludingStrength");
            category.ColumnIndex("LowestBaseStats");
            category.ColumnIndex("Mob");
            category.ColumnIndex("Res");
            category.ColumnIndex("Rec");
            category.ColumnIndex("Dis");
            category.ColumnIndex("Int");
            category.ColumnIndex("Str");
            category.ColumnIndex("MRR");
            category.ColumnIndex("DIS");
            // First find the exotic.
            List<Armor> exotic = armorItems.Where(a => a.Name == Exotic).ToList();
            string exoticType = exotic[0].ItemType;
            List<Armor> helmets = exoticType == "Helmet" ? exotic :
                armorItems.Where(a => a.ItemType == "Helmet" && a.TierType != "Exotic" &&
                a.PowerCap >= PowerCap).ToList();
            List<Armor> gauntlets = exoticType == "Gauntlets" ? exotic :
                armorItems.Where(a => a.ItemType == "Gauntlets" && a.TierType != "Exotic" &&
                a.PowerCap >= PowerCap).ToList();
            List<Armor> chestArmor = exoticType == "Chest Armor" ? exotic :
                armorItems.Where(a => a.ItemType == "Chest Armor" && a.TierType != "Exotic" &&
                a.PowerCap >= PowerCap).ToList();
            List<Armor> legArmor = exoticType == "Leg Armor" ? exotic :
                armorItems.Where(a => a.ItemType == "Leg Armor" && a.TierType != "Exotic" &&
                a.PowerCap >= PowerCap).ToList();
            foreach (Armor head in helmets)
                if (head.ClassType == ClassType)
                    foreach (Armor arm in gauntlets)
                        if (arm.ClassType == ClassType)
                            foreach (Armor chest in chestArmor)
                                if (chest.ClassType == ClassType)
                                    foreach (Armor leg in legArmor)
                                        if (leg.ClassType == ClassType)
                                        {
                                            object[] row = Calculate(category, head, arm, chest, leg);
                                            if (row != null)
                                                category.Rows.Add(row);
                                        }
            return category;
        }
        private object[] Calculate(Category category, Armor head, Armor arm, Armor chest, Armor leg)
        {
            // Calculate a result for this combination.
            long mobility = 10 + MobilityMod + head.Mobility + arm.Mobility + chest.Mobility + leg.Mobility;
            long resilience = 10 + ResilienceMod + head.Resilience + arm.Resilience + chest.Resilience + leg.Resilience;
            long recovery = 10 + RecoveryMod + head.Recovery + arm.Recovery + chest.Recovery + leg.Recovery;
            long discipline = 10 + DisciplineMod + head.Discipline + arm.Discipline + chest.Discipline + leg.Discipline;
            long intellect = 10 + IntellectMod + head.Intellect + arm.Intellect + chest.Intellect + leg.Intellect;
            long strength = 10 + StrengthMod + head.Strength + arm.Strength + chest.Strength + leg.Strength;
            // Calculate effective values (divide by 10).
            long mobi = Math.Min(10L, mobility / 10);
            long resi = Math.Min(10L, resilience / 10);
            long reco = Math.Min(10L, recovery / 10);
            long disc = Math.Min(10L, discipline / 10);
            long inte = Math.Min(10L, intellect / 10);
            long stre = Math.Min(10L, strength / 10);
            long usage = (mobi + resi + reco + disc + inte + stre) * 10;
            if (usage < _minimumUsage)
                return null;
            long wastage = mobility + resilience + recovery + discipline + intellect + strength - usage;
            long totalExcludingStrength = mobi + resi + reco + disc + inte;
            long lowestBaseStats = Math.Min(Math.Min(head.BaseStats, arm.BaseStats), Math.Min(chest.BaseStats, leg.BaseStats));
            long mrr = mobi + resi + reco;
            long dis = disc + inte + stre;
            object[] row = new object[category.ColumnNames.Count];
            row[category.ColumnIndex("Name")] = head.Id + "/" + arm.Id + "/" + chest.Id + "/" + leg.Id;
            row[category.ColumnIndex("Helmet")] = head.Id;
            row[category.ColumnIndex("Gauntlets")] = arm.Id;
            row[category.ColumnIndex("Chest Armor")] = chest.Id;
            row[category.ColumnIndex("Leg Armor")] = leg.Id;
            row[category.ColumnIndex("Mobility")] = mobi;
            row[category.ColumnIndex("Resilience")] = resi;
            row[category.ColumnIndex("Recovery")] = reco;
            row[category.ColumnIndex("Discipline")] = disc;
            row[category.ColumnIndex("Intellect")] = inte;
            row[category.ColumnIndex("Strength")] = stre;
            row[category.ColumnIndex("Usage")] = usage;
            row[category.ColumnIndex("Wastage")] = wastage;
            row[category.ColumnIndex("TotalExcludingStrength")] = totalExcludingStrength;
            row[category.ColumnIndex("LowestBaseStats")] = lowestBaseStats;
            row[category.ColumnIndex("Mob")] = mobility;
            row[category.ColumnIndex("Res")] = resilience;
            row[category.ColumnIndex("Rec")] = recovery;
            row[category.ColumnIndex("Dis")] = discipline;
            row[category.ColumnIndex("Int")] = intellect;
            row[category.ColumnIndex("Str")] = strength;
            row[category.ColumnIndex("MRR")] = mrr;
            row[category.ColumnIndex("DIS")] = dis;
            return row;
        }
        private const int _minimumUsage = 290;
    }
}
