using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;
using TodoApp.Application.Interfaces;
using MongoDB.Driver;
using TodoApp.Infrastructure.Settings;

namespace TodoApp.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Todo persistence using MongoDB.
    /// Implements ITodoRepository.
    /// </summary>
    public class TodoRepository : ITodoRepository
    {
        private readonly IMongoCollection<Todo> _todos;

        /// <summary>
        /// Constructor creates MongoDB client and gets the Todos collection.
        /// </summary>
        /// <param name="settings">MongoDB settings from configuration</param>
        public TodoRepository(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _todos = database.GetCollection<Todo>("Todos");
        }

        /// <summary>
        /// Retrieves a Todo by its unique Id.
        /// </summary>
        /// <param name="id">Todo Id</param>
        /// <returns>Todo entity or null if not found</returns>
        public async Task<Todo?> GetByIdAsync(string id) =>
            await _todos.Find(t => t.Id == id).FirstOrDefaultAsync();

        /// <summary>
        /// Retrieves a Todo by its title.
        /// </summary>
        /// <param name="title">Todo title</param>
        /// <returns>Todo entity or null if not found</returns>
        public async Task<Todo?> GetByTitleAsync(string title) =>
            await _todos.Find(t => t.Title == title).FirstOrDefaultAsync();

        /// <summary>
        /// Retrieves all Todos from the collection.
        /// </summary>
        /// <returns>Enumerable of all Todos</returns>
        public async Task<IEnumerable<Todo>> GetAllAsync() =>
            await _todos.Find(_ => true).ToListAsync();

        /// <summary>
        /// Adds a new Todo to the collection.
        /// Returns the created entity.
        /// </summary>
        /// <param name="todo">Todo to add</param>
        /// <returns>The created Todo</returns>
        public async Task<Todo> CreateAsync(Todo todo)
        {
            await _todos.InsertOneAsync(todo); // Insert into MongoDB
            return todo; // Return entity
        }

        /// <summary>
        /// Updates an existing Todo by replacing it in the collection.
        /// Returns the updated entity.
        /// </summary>
        /// <param name="todo">Todo to update</param>
        /// <returns>The updated Todo</returns>
        public async Task<Todo> UpdateAsync(Todo todo)
        {
            await _todos.ReplaceOneAsync(t => t.Id == todo.Id, todo); // Replace document
            return todo; // Return entity
        }

        /// <summary>
        /// Deletes a Todo by its Id.
        /// Returns the deleted entity or null if not found.
        /// </summary>
        /// <param name="id">Id of Todo to delete</param>
        /// <returns>The deleted Todo or null</returns>
        public async Task<Todo?> DeleteAsync(string id)
        {
            var todo = await GetByIdAsync(id); // Fetch the entity first
            if (todo != null)
                await _todos.DeleteOneAsync(t => t.Id == id); // Delete from collection
            return todo; // Return deleted entity
        }
    }
}
