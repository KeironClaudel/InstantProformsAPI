using MediatR;

namespace InstantProforms.Application.Features.Proforms.SendProformByEmail;

/// <summary>
/// Represents a request to send a proform by email.
/// </summary>
public sealed record SendProformByEmailCommand(
    Guid ProformId,
    string ToEmail,
    string? Subject,
    string? Message) : IRequest<SendProformByEmailResponse>;