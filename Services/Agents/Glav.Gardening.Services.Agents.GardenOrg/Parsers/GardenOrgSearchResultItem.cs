namespace Glav.Gardening.Services.Agents.GardenOrg.Parsers
{

    public record GardenOrgSearchResultItem
    {
        public string Href { get; init; }
        public string ResultText { get; init; }
    }
}