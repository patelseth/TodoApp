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
        /// <summary>
        /// Retrieves a Todo by its title. Returns null if not found.
        /// </summary>
        Task<Todo?> GetByTitleAsync(string title);

        /// <summary>
        /// Retrieves a Todo by its unique identifier. Returns null if not found.
        /// </summary>
        Task<Todo?> GetByIdAsync(string id);

        /// <summary>
        /// Creates a new Todo in the database.
        /// </summary>
        Task<Todo> CreateAsync(Todo todo);

        /// <summary>
        /// Updates an existing Todo in the database.
        /// </summary>
        Task<Todo> UpdateAsync(Todo todo);
    }
}