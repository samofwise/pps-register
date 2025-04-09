using Microsoft.AspNetCore.Mvc;

namespace PPSRegister.Api.Controllers;
public abstract class BaseController : ControllerBase
{
    protected int ClientId => int.TryParse(Request.Headers["X-Client-Id"].ToString(), out var clientId) ? clientId : 1;
}
