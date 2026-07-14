namespace Web.Request;

public record TicketCategoryRequest(
    string Name,
    decimal PriceMultiplier,
    string? Description
);
