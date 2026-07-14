namespace Web.Response;

public record TicketCategoryResponse(
    Guid Id,
    string Name,
    decimal PriceMultiplier,
    string? Description
);
