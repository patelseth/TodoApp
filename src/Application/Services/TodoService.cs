using System.Threading.Tasks;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Exceptions;

namespace TodoApp.Application.Services
{
    /// <summary>
    /// Implements business logic for Todo operations.
    /// Ensures rules such as unique title are enforced.
    /// </summary>
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository; // Interface dependency allows mocking

        public TodoService(ITodoRepository repository)
        {
            _repository = repository; // Injected repository follows dependency inversion
        }

        /// <summary>
        /// Creates a new Todo if the title is unique.
        /// Throws DuplicateTitleException if a duplicate exists.
        /// </summary>
        public async Task<Todo> CreateAsync(string title, string description)
        {
            var existing = await _repository.GetByTitleAsync(title);

            if (existing != null)
                throw new DuplicateTitleException();

            var todo = new Todo(title, description);

            return await _repository.CreateAsync(todo);
        }
    }
}