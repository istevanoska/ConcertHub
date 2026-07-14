using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class TicketCategoryExtensions
{
    public static TicketCategoryResponse ToResponse(this TicketCategory category)
    {
        return new TicketCategoryResponse(
            category.Id,
            category.Name,
            category.PriceMultiplier,
            category.Description);
    }
}
