using System.Collections.Generic;

namespace Glav.Gardening.Communications
{
    public static class AppServices
    {
        private static Dictionary<string, ServiceAppConfig> _svcConfig = new Dictionary<string, ServiceAppConfig>();
        static AppServices()
        {
            CreateConfig();
        }
        private static void CreateConfig()
        {
            
            _svcConfig.Add(ServiceAppId.DataSanitiser, new ServiceAppConfig(ServiceAppId.DataSanitiser, "localhost:5001"));
            _svcConfig.Add(ServiceAppId.InfoGatheringController, new ServiceAppConfig(ServiceAppId.InfoGatheringController, "localhost:5003"));
            _svcConfig.Add(ServiceAppId.GardenOrgAgent, new ServiceAppConfig(ServiceAppId.GardenOrgAgent, "localhost:5005"));
            _svcConfig.Add(ServiceAppId.DataStorage, new ServiceAppConfig(ServiceAppId.DataStorage, "localhost:5007"));
            _svcConfig.Add(ServiceAppId.State, new ServiceAppConfig(ServiceAppId.State, "localhost:6379"));
            _svcConfig.Add(ServiceAppId.PubSub, new ServiceAppConfig(ServiceAppId.PubSub, "localhost:6379"));
        }

        public static ServiceAppConfig ById(string appId)
        {
            if (_svcConfig.ContainsKey(appId))
            {
                return _svcConfig[appId];
            }

            throw new System.Exception($"No known service by Id [{appId}]");
        }
    }
}