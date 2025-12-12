using Microsoft.AspNetCore.Mvc;

namespace ProjectM.Controllers
{
    /// <summary>
    /// API Version 1.0
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
    }
}
