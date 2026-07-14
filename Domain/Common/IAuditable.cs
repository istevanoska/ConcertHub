namespace Domain.Common;

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    string? CreatedById { get; set; }
    DateTime LastModifiedAt { get; set; }
    string? LastModifiedById { get; set; }
}
