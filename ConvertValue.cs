namespace D2VE
{
    public static class ConvertValue
    {
        public static string ItemSubType(long value)
        {
            switch (value)
            {
                case 0L:
                    return "None";
                case 1L:
                    return "Crucible";
                case 2L:
                    return "Vanguard";
                case 5L:
                    return "Exotic";
                case 6L:
                    return "Auto Rifle";
                case 7L:
                    return "AutoRifle";
                case 8L:
                    return "Machinegun";
                case 9L:
                    return "Hand Cannon";
                case 10L:
                    return "Rocket Launcher";
                case 11L:
                    return "Fusion Rifle";
                case 12L:
                    return "Sniper Rifle";
                case 13L:
                    return "Pulse Rifle";
                case 14L:
                    return "Scout Rifle";
                case 16L:
                    return "Crm";
                case 17L:
                    return "Sidearm";
                case 18L:
                    return "Sword";
                case 19L:
                    return "Mask";
                case 20L:
                    return "Shader";
                case 21L:
                    return "Ornament";
                case 22L:
                    return "Linear Fusion Rifle";
                case 23L:
                    return "Grenade Launcher";
                case 24L:
                    return "Submachine Gun";
                case 25L:
                    return "Trace Rifle";
                case 26L:
                    return "Helmet Armor";
                case 27L:
                    return "Gauntlets Armor";
                case 28L:
                    return "Chest Armor";
                case 29L:
                    return "Leg Armor";
                case 30L:
                    return "Class Armor";
                case 31L:
                    return "Bow";
                default:
                    return value.ToString();
            }
        }
        public static string Season(string value)
        {
            switch (value)
            {
                case "penumbra":  // Year 1
                    return "";
                case "outlaw":
                    return "Outlaw";
                case "forge":
                    return "Forge";
                case "drifter":
                    return "Drifter";
                case "opulence":
                    return "Opulence";
                case "maverick":
                    return "Undying";
                case "v470":
                    return "Dawn";
                case "v480":
                    return "Worthy";
                case "v490":
                    return "Next";
                default:
                    return value.ToString();
            }
        }
        public static string ClassType(long value)
        {
            switch (value)
            {
                case 0L:
                    return "Titan";
                case 1L:
                    return "Hunter";
                case 2L:
                    return "Warlock";
                case 3L:
                    return "";
                default:
                    return value.ToString();
            }
        }
        public static string DamageType(long value)
        {
            switch (value)
            {
                case 0L:
                    return "";
                case 1L:
                    return "Kinetic";
                case 2L:
                    return "Arc";
                case 3L:
                    return "Solar";
                case 4L:
                    return "Void";
                case 5L:
                    return "Raid";
                default:
                    return value.ToString();
            }
        }
        public static string EnergyType(long value)
        {
            switch (value)
            {
                case 0L:
                    return "";
                case 1L:
                    return "Arc";
                case 2L:
                    return "Solar";
                case 3L:
                    return "Void";
                default:
                    return value.ToString();
            }
        }
        public static string StatName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value.ToString();
            switch (value)
            {
                case "Mobility":
                    return "1";
                case "Resilience":
                    return "2";
                case "Recovery":
                    return "3";
                case "Discipline":
                    return "4";
                case "Intellect":
                    return "5";
                case "Strength":
                    return "6";
                default:
                    return value;
            }
        }
        public static string StatSortedName(string value)
        {
            switch (value)
            {
                case "1":
                    return "Mobility";
                case "2":
                    return "Resilience";
                case "3":
                    return "Recovery";
                case "4":
                    return "Discipline";
                case "5":
                    return "Intellect";
                case "6":
                    return "Strength";
                default:
                    return value;
            }
        }
        // Different weapon types use different plug types but we can rename them to avoid lots of space.
        public static string PlugTypeName(string value)
        {
            if (value == "Arrow" || value == "Barrel" || value == "Launcher Barrel" || value == "Scope" || value == "Sight")
                return "Barrel/Sight";
            if (value == "Battery" || value == "Magazine")  // Fusion rifles have a stat called Magazine
                return "Magazine/Battery";
            if (value == "Grip" || value == "Stock")
                return "Perk2";
            return value;
        }
        public static string SeasonAbbreviation(string value)
        {
            switch (value)
            {
                case "Dawn/Worthy/Next":
                    return " W";
                case "Undying/Dawn/Worthy":
                    return " D";
                case "Opulence/Undying/Dawn":
                    return " U";
                case "Outlaw/Forge":
                    return " O";
                case "Opulence/Undying":
                    return " 1";  // This is an armor 1.0 piece
                default:
                    return value;
            }
        }
    }
}
