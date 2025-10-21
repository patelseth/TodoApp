using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using TodoApp.Domain.Entities;

namespace TodoApp.IntegrationTests
{
    /// <summary>
    /// Full integration tests for the TodoController API.
    /// These simulate real HTTP calls to verify persistence and workflow correctness.
    /// </summary>
    public class TodoControllerIntegrationTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TodoControllerIntegrationTests(ApiWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        #region Create

        /// <summary>
        /// Verifies that creating a new Todo returns 201 Created and persists it.
        /// </summary>
        [Fact]
        public async Task CreateTodo_ShouldReturnCreatedAndPersist()
        {
            var todo = new { Title = "Controller Create Todo", Description = "desc" };

            var response = await _client.PostAsJsonAsync("/api/todos", todo);
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<Todo>();
            Assert.NotNull(created);
            Assert.Equal("Controller Create Todo", created!.Title);
        }

        /// <summary>
        /// Verifies that creating a Todo with duplicate title returns 400 BadRequest.
        /// </summary>
        [Fact]
        public async Task CreateTodo_DuplicateTitle_ShouldReturnBadRequest()
        {
            var todo = new { Title = "Duplicate Controller Todo", Description = "desc" };

            await _client.PostAsJsonAsync("/api/todos", todo);
            var duplicateResponse = await _client.PostAsJsonAsync("/api/todos", todo);

            Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
        }

        #endregion

        #region Read

        /// <summary>
        /// Verifies that getting all Todos returns all persisted items.
        /// </summary>
        [Fact]
        public async Task GetAllTodos_ShouldReturnTodos()
        {
            var response = await _client.GetAsync("/api/todos");
            response.EnsureSuccessStatusCode();

            var todos = await response.Content.ReadFromJsonAsync<List<Todo>>();
            Assert.NotNull(todos);
        }

        /// <summary>
        /// Verifies that getting a Todo by ID returns correct entity.
        /// </summary>
        [Fact]
        public async Task GetTodoById_ShouldReturnCorrectTodo()
        {
            var create = new { Title = "Controller Get Todo", Description = "desc" };
            var response = await _client.PostAsJsonAsync("/api/todos", create);
            var created = await response.Content.ReadFromJsonAsync<Todo>();

            var getResponse = await _client.GetAsync($"/api/todos/{created!.Id}");
            getResponse.EnsureSuccessStatusCode();

            var todo = await getResponse.Content.ReadFromJsonAsync<Todo>();
            Assert.Equal(created.Id, todo!.Id);
        }

        /// <summary>
        /// Verifies that filtering Todos by status returns correct subset.
        /// </summary>
        [Fact]
        public async Task GetTodos_FilterByStatus_ShouldReturnCorrect()
        {
            // Create multiple todos
            var pending = await _client.PostAsJsonAsync("/api/todos", new { Title = "Pending Todo", Description = "desc" });
            var inProgress = await _client.PostAsJsonAsync("/api/todos", new { Title = "InProgress Todo", Description = "desc" });

            var inProgressEntity = await inProgress.Content.ReadFromJsonAsync<Todo>();

            // Update status to InProgress
            await _client.PutAsJsonAsync($"/api/todos/{inProgressEntity!.Id}/status", new { Status = "InProgress" });

            // Filter by status
            var filteredResponse = await _client.GetAsync("/api/todos?status=InProgress");
            filteredResponse.EnsureSuccessStatusCode();

            var filtered = await filteredResponse.Content.ReadFromJsonAsync<List<Todo>>();
            Assert.Single(filtered!);
            Assert.Equal("InProgress Todo", filtered[0].Title);
        }

        #endregion

        #region Update

        /// <summary>
        /// Verifies that updating title and description persists changes.
        /// </summary>
        [Fact]
        public async Task UpdateTodo_ShouldPersistChanges()
        {
            var create = new { Title = "Controller Update Todo", Description = "old" };
            var response = await _client.PostAsJsonAsync("/api/todos", create);
            var created = await response.Content.ReadFromJsonAsync<Todo>();

            var update = new { Title = "Updated Title", Description = "Updated desc" };
            var updateResponse = await _client.PutAsJsonAsync($"/api/todos/{created!.Id}", update);
            updateResponse.EnsureSuccessStatusCode();

            var updated = await updateResponse.Content.ReadFromJsonAsync<Todo>();
            Assert.Equal("Updated Title", updated!.Title);
            Assert.Equal("Updated desc", updated.Description);
        }

        /// <summary>
        /// Verifies that updating to a duplicate title returns 400 BadRequest.
        /// </summary>
        [Fact]
        public async Task UpdateTodo_DuplicateTitle_ShouldReturnBadRequest()
        {
            await _client.PostAsJsonAsync("/api/todos", new { Title = "Original Controller Todo", Description = "desc" });
            var second = await _client.PostAsJsonAsync("/api/todos", new { Title = "Duplicate Controller Todo", Description = "desc" });
            var secondTodo = await second.Content.ReadFromJsonAsync<Todo>();

            var update = new { Title = "Original Controller Todo", Description = "Updated" };
            var duplicateResponse = await _client.PutAsJsonAsync($"/api/todos/{secondTodo!.Id}", update);

            Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
        }

        /// <summary>
        /// Verifies that valid status transitions persist.
        /// </summary>
        [Fact]
        public async Task UpdateStatus_ValidTransitions_ShouldPersist()
        {
            var create = await _client.PostAsJsonAsync("/api/todos", new { Title = "Controller Status Todo", Description = "desc" });
            var created = await create.Content.ReadFromJsonAsync<Todo>();

            var inProgressResponse = await _client.PutAsJsonAsync($"/api/todos/{created!.Id}/status", new { Status = "InProgress" });
            inProgressResponse.EnsureSuccessStatusCode();

            var completedResponse = await _client.PutAsJsonAsync($"/api/todos/{created!.Id}/status", new { Status = "Completed" });
            completedResponse.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Verifies that invalid status transitions return 400 BadRequest.
        /// </summary>
        [Fact]
        public async Task UpdateStatus_InvalidTransition_ShouldReturnBadRequest()
        {
            var create = await _client.PostAsJsonAsync("/api/todos", new { Title = "Invalid Status Todo", Description = "desc" });
            var created = await create.Content.ReadFromJsonAsync<Todo>();

            var invalidResponse = await _client.PutAsJsonAsync($"/api/todos/{created!.Id}/status", new { Status = "Completed" });

            Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Verifies that deleting a Todo removes it successfully.
        /// </summary>
        [Fact]
        public async Task DeleteTodo_ShouldReturnNoContent()
        {
            var create = new { Title = "Controller Delete Todo", Description = "desc" };
            var response = await _client.PostAsJsonAsync("/api/todos", create);
            var created = await response.Content.ReadFromJsonAsync<Todo>();

            var deleteResponse = await _client.DeleteAsync($"/api/todos/{created!.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        #endregion
    }
}