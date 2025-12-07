using Core.Enums;

namespace Core.DTOs;

public record TaskUpdateDto(
    Guid Id,
    Guid TaskItemId,
    Guid UpdatedById,
    TaskStatus Status,
    string? Comment,
    string? AttachmentUrl,
    DateTime CreatedAt,
    double EffortLogged
);
