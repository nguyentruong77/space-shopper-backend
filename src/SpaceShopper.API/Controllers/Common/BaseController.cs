using Microsoft.AspNetCore.Mvc;

namespace SpaceShopper.API.Controllers.Common
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BaseController() : ControllerBase
    {
    }
}
