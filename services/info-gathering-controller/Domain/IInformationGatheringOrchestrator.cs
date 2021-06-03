using System;

namespace Glav.InformationGathering.Domain
{
    public interface IInformationGatheringOrchestrator
    {
        System.Threading.Tasks.Task InitiateAsync();
    }
}