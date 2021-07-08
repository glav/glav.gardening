using System.Net.Http;
using System.Threading.Tasks;

namespace Glav.InformationGathering.Domain.GardenOrg
{
    public class GaardenOrgWebsiteAgent : IInfoGatheringAgent
    {
        private int _progress = 0;
        const string queryUrl = "https://garden.org/plants/search/text/?q={0}";

        public int Progress => _progress;

        public async Task StartAsync(string queryTerm)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync(string.Format(queryUrl,queryTerm));
            _progress = 33;
        }

        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}