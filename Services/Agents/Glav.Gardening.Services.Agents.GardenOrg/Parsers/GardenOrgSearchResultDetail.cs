namespace Glav.Gardening.Services.Agents.GardenOrg.Parsers
{

    public record GardenOrgSearchResultDetail
    {

        public string OrdinaryName { get; init; }
        public string ScientificName { get; init; }
        public string Habit { get; init; }
        public string SunRequirements { get; init; }
        public string Leaves { get; init; }
    }
}