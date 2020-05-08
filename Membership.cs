namespace D2VE
{
    public class Membership
    {
        public Membership(string displayName, string membershipType, string membershipId)
        {
            DisplayName = displayName;
            Type = membershipType;
            Id = membershipId;
        }
        public string DisplayName { get; }
        public string Type { get; }
        public string Id { get; }
        public override string ToString() { return DisplayName; }
    }
}
