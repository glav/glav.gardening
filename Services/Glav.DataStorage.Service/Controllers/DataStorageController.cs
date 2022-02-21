using Microsoft.AspNetCore.Mvc;
using Glav.Gardening.Communications;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Glav.DataStorage.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class DataStorageController : ControllerBase
{
    private readonly ILogger<DataStorageController> _logger;
    private readonly ICommunicationProxy _commsProxy;

    public DataStorageController(ILogger<DataStorageController> logger, ICommunicationProxy commsProxy)
    {
        _logger = logger;
        _commsProxy = commsProxy;
    }

    [HttpPost("/persist")]
    public async Task Persist(string key, object data)
    {
        _logger.LogInformation("DataStorage: Persisting data to storage");
        var storeResult = await _commsProxy.StoreState(key, data);
        //var testData = await _commsProxy.GetState(key);
    }
}

