namespace DatingApp.Helpers;

public sealed class LikesParams : PaginationParams
{
    public Guid UserId { get; set; }
    public string Predicate { get; set; }
}