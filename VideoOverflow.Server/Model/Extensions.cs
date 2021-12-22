namespace Server.Model;

public static class Extensions
{
    /// <summary>
    /// Converts a status to an action result
    /// </summary>
    /// <param name="status">The status to convert</param>
    /// <param name="location">If created, the location of the new entity</param>
    /// <param name="value">If created, the new object</param>
    /// <returns>The action result based on the status</returns>
    /// <exception cref="NotSupportedException">If a status isn't supported yet</exception>
    public static IActionResult ToActionResult(this Status status, string location="", object? value=null) => status switch
    {
        Updated => new NoContentResult(),
        Deleted => new NoContentResult(),
        NotFound => new NotFoundResult(),
        Conflict => new ConflictResult(),
        Created => new CreatedResult(location, value),
        _ => throw new NotSupportedException($"{status} not supported")
    };

    public static ActionResult<T> ToActionResult<T>(this Option<T> option) where T : class
        => option.IsSome ? option.Value : new NotFoundResult();
}