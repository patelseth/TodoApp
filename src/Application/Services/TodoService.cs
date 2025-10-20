using System.Threading.Tasks;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Exceptions;

namespace TodoApp.Application.Services
{
    /// <summary>
    /// Service responsible for application-level operations on Todo items.
    /// Follows SOLID principles:
    /// - SRP: Handles coordination and business workflow, not internal entity rules.
    /// - OCP: Can be extended for additional rules without modifying entity logic.
    /// - DIP: Depends on ITodoRepository abstraction, not a concrete database.
    /// </summary>
    public class TodoService
    {
        private readonly ITodoRepository _repository;

        /// <summary>
        /// Constructs the service with a repository dependency.
        /// Follows Dependency Inversion Principle by depending on interface.
        /// </summary>
        /// <param name="repository">Repository used to persist and retrieve Todos</param>
        public TodoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Creates a new Todo after ensuring the title is unique.
        /// Throws DuplicateTitleException if a Todo with the same title exists.
        /// </summary>
        /// <param name="title">Title of the new Todo</param>
        /// <param name="description">Optional description</param>
        /// <returns>The created Todo entity</returns>
        public async Task<Todo> CreateAsync(string title, string? description = null)
        {
            var existing = await _repository.GetByTitleAsync(title);
            if (existing != null)
                throw new DuplicateTitleException();

            var todo = new Todo(title, description);
            return await _repository.CreateAsync(todo);
        }

        /// <summary>
        /// Updates the status of a Todo using the entity's ChangeStatus method.
        /// This ensures that all domain rules for status transitions are met.
        /// Throws InvalidStatusTransitionException if the transition is invalid.
        /// </summary>
        /// <param name="todo">The Todo to update</param>
        /// <param name="newStatus">The new status</param>
        /// <returns>The updated Todo</returns>
        public async Task<Todo> UpdateStatusAsync(Todo todo, TodoStatus newStatus)
        {
            // Domain handles validation; service handles workflow
            todo.ChangeStatus(newStatus);

            // Persist updated entity
            return await _repository.UpdateAsync(todo); // or UpdateAsync depending on repo
        }
    }
}