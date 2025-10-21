using Domain.Entities;
using Domain.Exceptions;
using MongoDB.Driver;
using Infrastructure.Repositories;
using Application.Services;
using Infrastructure.Settings;
using Application.Interfaces;

namespace IntegrationTests
{
    /// <summary>
    /// Full integration tests for TodoService with MongoDB persistence.
    /// Ensures that all domain rules and service workflows work end-to-end with the database.
    /// </summary>
    public class TodoServiceIntegrationTests : IAsyncLifetime
    {
        private readonly ITodoRepository _repository;
        private readonly TodoService _service;
        private readonly List<string> _cleanupIds = new();

        public TodoServiceIntegrationTests()
        {
            var settings = new MongoDbSettings
            {
                ConnectionString = "mongodb+srv://TodoAdmin:TodoAdmin123@todocluster.uhecxgp.mongodb.net/?retryWrites=true&w=majority",
                DatabaseName = "TodoDbTest"
            };

            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _repository = new TodoRepository(database, settings);
            _service = new TodoService(_repository);
        }

        public async Task InitializeAsync()
        {
            var client = new MongoClient("mongodb+srv://TodoAdmin:TodoAdmin123@todocluster.uhecxgp.mongodb.net/?retryWrites=true&w=majority");
            var db = client.GetDatabase("TodoDbTest");
            await db.DropCollectionAsync("Todos");
        }

        /// <summary>
        /// Runs after all tests finish. Clean up test todos.
        /// </summary>
        public async Task DisposeAsync()
        {
            foreach (var id in _cleanupIds)
                await _repository.DeleteAsync(id);
        }

        #region Create Tests

        /// <summary>
        /// Verifies that creating a Todo with a duplicate title is rejected.
        /// </summary>
        [Fact]
        public async Task CreateAsync_DuplicateTitle_ShouldThrowException()
        {
            var todo = await _service.CreateAsync("Integration Create Todo", "desc");
            _cleanupIds.Add(todo.Id);

            await Assert.ThrowsAsync<DuplicateTitleException>(() =>
                _service.CreateAsync("Integration Create Todo", "desc"));
        }

        #endregion

        #region Update Tests

        /// <summary>
        /// Verifies that updating a Todo's title and description persists correctly.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ShouldPersistChanges()
        {
            var todo = await _service.CreateAsync("Update Test Todo", "original");
            _cleanupIds.Add(todo.Id);

            var updated = await _service.UpdateAsync(todo.Id, "Updated Todo", "Updated desc");

            Assert.Equal("Updated Todo", updated.Title);
            Assert.Equal("Updated desc", updated.Description);
        }

        /// <summary>
        /// Verifies that updating a Todo to a duplicate title throws DuplicateTitleException.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_DuplicateTitle_ShouldThrowException()
        {
            var first = await _service.CreateAsync("Original Todo", "desc");
            var second = await _service.CreateAsync("Duplicate Todo", "desc");
            _cleanupIds.Add(first.Id);
            _cleanupIds.Add(second.Id);

            await Assert.ThrowsAsync<DuplicateTitleException>(() =>
                _service.UpdateAsync(first.Id, "Duplicate Todo", "Updated"));
        }

        #endregion

        #region Status Tests

        /// <summary>
        /// Verifies that valid status transitions (Pending -> InProgress -> Completed) succeed.
        /// </summary>
        [Fact]
        public async Task UpdateStatusAsync_ValidTransitions_ShouldPersist()
        {
            var todo = await _service.CreateAsync("Status Valid Todo", "desc");
            _cleanupIds.Add(todo.Id);

            var inProgress = await _service.UpdateStatusAsync(todo, TodoStatus.InProgress);
            Assert.Equal(TodoStatus.InProgress, inProgress.Status);

            var completed = await _service.UpdateStatusAsync(todo, TodoStatus.Completed);
            Assert.Equal(TodoStatus.Completed, completed.Status);
        }

        /// <summary>
        /// Verifies that invalid status transitions are rejected.
        /// </summary>
        [Fact]
        public async Task UpdateStatusAsync_InvalidTransition_ShouldThrowException()
        {
            var todo = await _service.CreateAsync("Status Invalid Todo", "desc");
            _cleanupIds.Add(todo.Id);

            await Assert.ThrowsAsync<InvalidStatusTransitionException>(() =>
                _service.UpdateStatusAsync(todo, TodoStatus.Completed));
        }

        #endregion

        #region Filtering & GetAll Tests

        /// <summary>
        /// Verifies that GetTodosAsync returns all todos when no filter is applied.
        /// </summary>
        [Fact]
        public async Task GetTodosAsync_NoFilter_ShouldReturnAll()
        {
            var todo1 = await _service.CreateAsync("Board Todo 1", "desc");
            var todo2 = await _service.CreateAsync("Board Todo 2", "desc");
            _cleanupIds.Add(todo1.Id);
            _cleanupIds.Add(todo2.Id);

            var allTodos = await _service.GetTodosAsync(null);

            Assert.Contains(allTodos, t => t.Id == todo1.Id);
            Assert.Contains(allTodos, t => t.Id == todo2.Id);
        }

        /// <summary>
        /// Verifies that GetTodosAsync filters correctly by status.
        /// </summary>
        [Fact]
        public async Task GetTodosAsync_FilterByStatus_ShouldReturnCorrect()
        {
            var pending = await _service.CreateAsync("Pending Board Todo", "desc");
            var inProgress = await _service.CreateAsync("InProgress Board Todo", "desc");
            _cleanupIds.Add(pending.Id);
            _cleanupIds.Add(inProgress.Id);

            await _service.UpdateStatusAsync(inProgress, TodoStatus.InProgress);

            var filtered = await _service.GetTodosAsync(TodoStatus.InProgress);

            Assert.Single(filtered);
            Assert.Equal("InProgress Board Todo", filtered.First().Title);
        }

        #endregion
    }
}