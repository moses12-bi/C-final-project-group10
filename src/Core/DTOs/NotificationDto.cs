namespace Core.DTOs;

public record NotificationDto(
    Guid Id,
    Guid UserId,
    string Type,
    string PayloadJson,
    bool IsRead,
    DateTime CreatedAt
);
