using System;
using System.Collections.Generic;
using System.Linq;

namespace D2VE
{
    public class Armor
    {
        public Armor(string name, string classType, string tierType, string itemType, string energyType,
            bool artifice, long energyCapacity, long powerCap,
            long mobility, long resilience, long recovery, long discipline, long intellect, long strength)
        {
            Name = name;
            ClassType = classType;
            TierType = tierType;
            ItemType = itemType;
            EnergyType = energyType;
            Artifice = artifice;
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
            RD = recovery + discipline;
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
        public bool Artifice { get; }
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
        public long RD { get; }
        public string Id { get; }
        public override string ToString() { return Id; }
    }
    public class ArmorCalculator
    {
        public ArmorCalculator(string classType, string exotic,
            long mobilityMod = 0L, long resilienceMod = 0L, long recoveryMod = 0L,
            long disciplineMod = 0L, long intellectMod = 0L, long strengthMod = 0L)
        {
            string mods = mobilityMod.ToString() + "," + resilienceMod.ToString() + "," + recoveryMod.ToString()
                + "," + disciplineMod.ToString() + "," + intellectMod.ToString() + "," + strengthMod.ToString();
            Name = classType + " - " + exotic + (mods == "0,0,0,0,0,0" ? "" : " - " + mods);
            ClassType = classType;
            Exotic = exotic;
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
            category.ColumnIndex("Affinity");
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
            category.ColumnIndex("Artifice");
            category.ColumnIndex("Head Type");
            category.ColumnIndex("Arms Type");
            category.ColumnIndex("Chest Type");
            category.ColumnIndex("Leg Type");
            category.ColumnIndex("Wastage");
            category.ColumnIndex("LowestBaseStats");
            category.ColumnIndex("Mob");
            category.ColumnIndex("Res");
            category.ColumnIndex("Rec");
            category.ColumnIndex("Dis");
            category.ColumnIndex("Int");
            category.ColumnIndex("Str");
            category.ColumnIndex("MRR");
            category.ColumnIndex("DIS");
            category.ColumnIndex("Exotic");
            category.ColumnIndex("ExoticType");
            category.ColumnIndex("Masterworked");
            category.ColumnIndex("Exotic Masterworked");
            category.ColumnIndex("Head Masterworked");
            category.ColumnIndex("Arms Masterworked");
            category.ColumnIndex("Chest Masterworked");
            category.ColumnIndex("Leg Masterworked");
            category.ColumnIndex("Head MMR");
            category.ColumnIndex("Arms MMR");
            category.ColumnIndex("Chest MMR");
            category.ColumnIndex("Leg MMR");
            category.ColumnIndex("Head DIS");
            category.ColumnIndex("Arms DIS");
            category.ColumnIndex("Chest DIS");
            category.ColumnIndex("Leg DIS");
            category.ColumnIndex("Head Rec");
            category.ColumnIndex("Arms Rec");
            category.ColumnIndex("Chest Rec");
            category.ColumnIndex("Leg Rec");
            // First find the exotic.
            List<Armor> exotic = armorItems.Where(a => a.Name == Exotic).ToList();
            if (exotic.Count == 0)  // we don't have one!
                return category;
            string exoticType = exotic[0].ItemType;
            List<Armor> helmets = exoticType == "Helmet" ? exotic :
                armorItems.Where(a => a.ItemType == "Helmet" && a.TierType != "Exotic").ToList();
            List<Armor> gauntlets = exoticType == "Gauntlets" ? exotic :
                armorItems.Where(a => a.ItemType == "Gauntlets" && a.TierType != "Exotic").ToList();
            List<Armor> chestArmor = exoticType == "Chest Armor" ? exotic :
                armorItems.Where(a => a.ItemType == "Chest Armor" && a.TierType != "Exotic").ToList();
            List<Armor> legArmor = exoticType == "Leg Armor" ? exotic :
                armorItems.Where(a => a.ItemType == "Leg Armor" && a.TierType != "Exotic").ToList();
            foreach (Armor head in helmets)
                if (head.ClassType == ClassType)
                    foreach (Armor arm in gauntlets)
                        if (arm.ClassType == ClassType)
                            foreach (Armor chest in chestArmor)
                                if (chest.ClassType == ClassType)
                                    foreach (Armor leg in legArmor)
                                        if (leg.ClassType == ClassType)
                                        {
                                            object[] row = Calculate(category, head, arm, chest, leg, exoticType);
                                            if (row != null)
                                                category.Rows.Add(row);
                                        }
            return category;
        }
        private object[] Calculate(Category category, Armor head, Armor arm, Armor chest, Armor leg, string exoticType)
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
            bool artifice = exoticType == "Helmet" || head.Artifice
                && exoticType == "Gauntlets" || arm.Artifice
                && exoticType == "Chest Armor" || chest.Artifice
                && exoticType == "Leg Armor" || leg.Artifice;
            long wastage = mobility + resilience + recovery + discipline + intellect + strength - usage;
            long lowestBaseStats = Math.Min(Math.Min(head.BaseStats, arm.BaseStats), Math.Min(chest.BaseStats, leg.BaseStats));
            long mrr = mobi + resi + reco;
            long dis = disc + inte + stre;
            bool masterworked = head.EnergyCapacity == 10L && arm.EnergyCapacity == 10L
                && chest.EnergyCapacity == 10L && leg.EnergyCapacity == 10L;
            bool exoticMasterworked = exoticType == "Helmet" && head.EnergyCapacity == 10L
                || exoticType == "Gauntlets" && arm.EnergyCapacity == 10L
                || exoticType == "Chest Armor" && chest.EnergyCapacity == 10L
                || exoticType == "Leg Armor" && leg.EnergyCapacity == 10L;
            string affinity = (head.EnergyCapacity == 10L ? ConvertValue.EnergyTypeShort(head.EnergyType) : "_")
                + (arm.EnergyCapacity == 10L ? ConvertValue.EnergyTypeShort(arm.EnergyType) : "_")
                + (chest.EnergyCapacity == 10L ? ConvertValue.EnergyTypeShort(chest.EnergyType) : "_")
                + (leg.EnergyCapacity == 10L ? ConvertValue.EnergyTypeShort(leg.EnergyType) : "_");
            object[] row = new object[category.ColumnNames.Count];
            row[category.ColumnIndex("Name")] = head.Id + "/" + arm.Id + "/" + chest.Id + "/" + leg.Id;
            row[category.ColumnIndex("Affinity")] = affinity;
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
            row[category.ColumnIndex("Artifice")] = artifice;
            row[category.ColumnIndex("Head Type")] = head.EnergyCapacity == 10L ? head.EnergyType : "";
            row[category.ColumnIndex("Arms Type")] = arm.EnergyCapacity == 10L ? arm.EnergyType : "";
            row[category.ColumnIndex("Chest Type")] = chest.EnergyCapacity == 10L ? chest.EnergyType : "";
            row[category.ColumnIndex("Leg Type")] = leg.EnergyCapacity == 10L ? leg.EnergyType : "";
            row[category.ColumnIndex("Wastage")] = wastage;
            row[category.ColumnIndex("LowestBaseStats")] = lowestBaseStats;
            row[category.ColumnIndex("Mob")] = mobility;
            row[category.ColumnIndex("Res")] = resilience;
            row[category.ColumnIndex("Rec")] = recovery;
            row[category.ColumnIndex("Dis")] = discipline;
            row[category.ColumnIndex("Int")] = intellect;
            row[category.ColumnIndex("Str")] = strength;
            row[category.ColumnIndex("MRR")] = mrr;
            row[category.ColumnIndex("DIS")] = dis;
            row[category.ColumnIndex("Exotic")] = Exotic;
            row[category.ColumnIndex("ExoticType")] = exoticType;
            row[category.ColumnIndex("Masterworked")] = masterworked;
            row[category.ColumnIndex("Exotic Masterworked")] = exoticMasterworked;
            row[category.ColumnIndex("Head Masterworked")] = head.EnergyCapacity == 10L;
            row[category.ColumnIndex("Arms Masterworked")] = arm.EnergyCapacity == 10L;
            row[category.ColumnIndex("Chest Masterworked")] = chest.EnergyCapacity == 10L;
            row[category.ColumnIndex("Leg Masterworked")] = leg.EnergyCapacity == 10L;
            row[category.ColumnIndex("Head MMR")] = head.Mrr;
            row[category.ColumnIndex("Arms MMR")] = arm.Mrr;
            row[category.ColumnIndex("Chest MMR")] = chest.Mrr;
            row[category.ColumnIndex("Leg MMR")] = leg.Mrr;
            row[category.ColumnIndex("Head DIS")] = head.Dis;
            row[category.ColumnIndex("Arms DIS")] = arm.Dis;
            row[category.ColumnIndex("Chest DIS")] = chest.Dis;
            row[category.ColumnIndex("Leg DIS")] = leg.Dis;
            row[category.ColumnIndex("Head Rec")] = head.Recovery;
            row[category.ColumnIndex("Arms Rec")] = arm.Recovery;
            row[category.ColumnIndex("Chest Rec")] = chest.Recovery;
            row[category.ColumnIndex("Leg Rec")] = leg.Recovery;
            return row;
        }
        private const int _minimumUsage = 300;
    }
}
