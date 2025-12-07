namespace Core.DTOs;

public record CreateNotificationRequest(
    Guid UserId,
    string Type,
    string PayloadJson
);
