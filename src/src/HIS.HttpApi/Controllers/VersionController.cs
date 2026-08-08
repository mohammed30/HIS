using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace HIS.Controllers;

[Route("api/app/version")]
public class VersionController : HISController
{
    [HttpGet]
    public string GetVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";
        return version;
    }
}
