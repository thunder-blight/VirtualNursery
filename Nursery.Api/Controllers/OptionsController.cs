using Microsoft.AspNetCore.Mvc;
using Nursery.Core.Common;

namespace Nusery.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OptionsController : ControllerBase
{
    // GET /api/options
    [HttpGet]
    public ActionResult GetOptions()
    {
        return Ok(new
        {
            plantTypes = Enum.GetNames<PlantType>(),
            lifeCycleTypes = Enum.GetNames<LifeCycleType>()
        });
    }
}

