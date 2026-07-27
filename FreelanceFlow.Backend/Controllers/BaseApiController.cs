using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceFlow.Backend.Controllers;

/// <summary>
/// Base for every controller that requires an authenticated user. Pulls the
/// freelancer's own Id out of the JWT claims once so feature controllers
/// don't repeat that boilerplate.
/// </summary>
[Authorize]
public abstract class BaseApiController : ControllerBase
{
    protected Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Authenticated request is missing a NameIdentifier claim."));
}