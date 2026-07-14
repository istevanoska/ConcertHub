using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Service.Interface;

namespace Web.Interceptor;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;

    public AuditInterceptor(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context == null)
            return base.SavingChanges(eventData, result);

        var entries = context.ChangeTracker
            .Entries<IAuditable>();

        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;
            var user = _currentUser.GetUserId() ?? "system";

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedById = user;
                entry.Entity.CreatedAt = now;
                entry.Entity.LastModifiedById = user;
                entry.Entity.LastModifiedAt = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedById = user;
                entry.Entity.LastModifiedAt = now;
            }
        }

        return base.SavingChanges(eventData, result);
    }
}
