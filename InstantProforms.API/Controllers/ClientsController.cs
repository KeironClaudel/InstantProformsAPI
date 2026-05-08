using InstantProforms.Api.Contracts.Clients;
using InstantProforms.Application.Features.Clients;
using InstantProforms.Application.Features.Clients.CreateClient;
using InstantProforms.Application.Features.Clients.DeleteClient;
using InstantProforms.Application.Features.Clients.GetClientById;
using InstantProforms.Application.Features.Clients.GetClients;
using InstantProforms.Application.Features.Clients.UpdateClient;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstantProforms.Api.Controllers;

/// <summary>
/// Provides endpoints for client management.
/// </summary>
[ApiController]
[Authorize]
[Route("api/clients")]
public sealed class ClientsController : ControllerBase
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientsController"/> class.
    /// </summary>
    /// <param name="sender">The MediatR sender.</param>
    public ClientsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets active clients for the current company.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The active clients.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ClientResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ClientResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetClientsQuery(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Gets a client by identifier.
    /// </summary>
    /// <param name="id">The client identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The client details.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ClientResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetClientByIdQuery(id), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Creates a new client.
    /// </summary>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created client.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<ClientResponse>> Create(
        [FromBody] CreateClientRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Updates an existing client.
    /// </summary>
    /// <param name="id">The client identifier.</param>
    /// <param name="request">The request payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated client.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ClientResponse>> Update(
        Guid id,
        [FromBody] UpdateClientRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request.ToCommand(id), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Archives a client.
    /// </summary>
    /// <param name="id">The client identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content when the archival succeeds.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteClientCommand(id), cancellationToken);
        return NoContent();
    }
}
