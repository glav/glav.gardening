using System.Net.Http;
using System.Threading.Tasks;

namespace Glav.InformationGathering.Domain.GardenOrg
{
    public class GaardenOrgWebsiteAgent : IInfoGatheringAgent
    {
        const string queryUrl = "https://garden.org/plants/search/text/?q={0}";

        public Task<int> Progress()
        {
            throw new System.NotImplementedException();
        }

        public async Task StartAsync(string queryTerm)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync(string.Format(queryUrl,queryTerm));
        }

        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}