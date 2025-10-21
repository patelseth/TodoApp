using Domain.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Service abstraction for Todo operations.
    /// Enables swapping implementations or mocking for testing.
    /// </summary>
    public interface ITodoService
    {
        Task<Todo> CreateAsync(string title, string description); // Creates a new Todo
    }
}