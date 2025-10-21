namespace Application.Requests
{
    /// <summary>
    /// Record used for updating an existing Todo.
    /// </summary>
    public record UpdateTodoRequest(
        string Id,
        string Title,
        string? Description
    );
}
