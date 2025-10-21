namespace Application.Requests
{
    /// <summary>
    /// Record used for updating an existing Todo.
    /// </summary>
    public record UpdateTodoRequest(
        string Title,
        string? Description
    );
}
