namespace Application.Requests
{
    /// <summary>
    /// Record used for creating a new Todo.
    /// </summary>
    public record CreateTodoRequest(
        string Title,
        string? Description
    );
}