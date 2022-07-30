using Microsoft.AspNetCore.Mvc; 

namespace user_service.Controller;


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
