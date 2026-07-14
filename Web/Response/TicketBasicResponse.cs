namespace Web.Response;

public record TicketBasicResponse(
    Guid Id,
    string Status,
    decimal Price
);
