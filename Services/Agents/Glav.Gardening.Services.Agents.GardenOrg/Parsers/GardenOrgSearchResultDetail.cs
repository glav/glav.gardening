namespace Glav.Gardening.Services.Agents.GardenOrg.Parsers
{

    public record GardenOrgSearchResultDetail
    {

        public string OrdinaryName { get; init; }
        public string ScientificName { get; init; }
        public string[] PlantHabit { get; init; }
        public string[] SunRequirements { get; init; }
        public string[] Leaves { get; init; }
        public string[] Flowers { get; init; }
        public string[] Uses { get; init; }
        public string[] WildlifeAttractant { get; init; }
        public PropagationMethods Propagation { get; init; }
        public string[] Containers { get; init; }
    }

    public record PropagationMethods
    {
        public string Seeds { get; init; }
        public string OtherMethods { get; init; }
    }
}