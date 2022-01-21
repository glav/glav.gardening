using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Glav.DataStorage.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class DataStorageController : ControllerBase
{
    private readonly ILogger<DataStorageController> _logger;

    public DataStorageController(ILogger<DataStorageController> logger)
    {
        _logger = logger;
    }

    [HttpPost("/persist")]
    public async Task<object> Persist()
    {
        _logger.LogInformation("DataStorage: Persisting data to storage");
        return new
        {
            Status = "ok",
            Date = DateTime.Now
        };
    }
}

