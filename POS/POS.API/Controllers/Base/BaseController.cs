using Microsoft.AspNetCore.Mvc;

namespace POS.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    protected BaseController()
    {
    }
}