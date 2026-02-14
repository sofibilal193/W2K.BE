using W2K.Common.Application.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace W2K.WebApp.Controllers;

[OpenApiIgnore]
[AllowAnonymous]
[Route("")]
[Route("Home")]
public class HomeController : BaseApiController
{
    // GET: /<controller>/
    [HttpGet]
    public IActionResult Index()
    {
        return new RedirectResult("~/swagger");
    }
}
