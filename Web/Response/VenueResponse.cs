namespace Web.Response;

public record VenueResponse(
    Guid Id,
    string Name,
    string City,
    string Address,
    int Capacity
);
