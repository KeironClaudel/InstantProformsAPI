using MediatR;
using Microsoft.AspNetCore.Mvc;
using InstantProforms.Api.Contracts.Auth;
using InstantProforms.Application.Features.Auth.RegisterCompany;

namespace InstantProforms.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register-company")]
    [ProducesResponseType(typeof(RegisterCompanyResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RegisterCompanyResponse>> RegisterCompany(
        [FromBody] RegisterCompanyRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(), cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}