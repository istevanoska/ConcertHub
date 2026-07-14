namespace Web.Request;

public record VenueRequest(
    string Name,
    string City,
    string Address,
    int Capacity
);
