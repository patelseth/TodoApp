using System.Threading.Tasks;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interfaces
{
    /// <summary>
    /// Repository abstraction for Todo persistence.
    /// Allows service to depend on an interface instead of a concrete class.
    /// </summary>
    public interface ITodoRepository
    {
        Task<Todo> GetByTitleAsync(string title); // Retrieves a Todo by title
        Task<Todo> CreateAsync(Todo todo); // Creates a new Todo
    }
}