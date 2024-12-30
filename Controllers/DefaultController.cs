using Microsoft.AspNetCore.Mvc;

namespace user_service.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class DefauiltController : ControllerBase
{
    [Route("/")]
    [Route("/swagger")]
    public RedirectResult Index()
    {
        return new RedirectResult("~/swagger");
    }
}