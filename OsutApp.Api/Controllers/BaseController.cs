using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OsutApp.Api.Controllers;

public class BaseController : ControllerBase
{
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        return Guid.Parse(userIdClaim);
    }
}