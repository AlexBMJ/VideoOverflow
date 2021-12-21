namespace VideoOverflow.Core
{
    /// <summary>
    /// An enum of all statuses in our repositories
    /// </summary>
    public enum Status
    {
        Created,
        Updated,
        Deleted,
        NotFound,
        BadRequest,
        Conflict
    }
}