namespace D2VE
{
    public static class ConvertValue
    {
        public static string Season(long value)
        {
            switch (value)
            {
                case 0L:
                    return "";
                case 7L:
                    return "Undying";
                case 8L:
                    return "Dawn";
                case 9L:
                    return "Worthy";
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
    }
}
