using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Domain.Exceptions;
using Application.Requests;
using Application.Services;

namespace Api.Controllers
{
    /// <summary>
    /// REST API controller for managing Todo entities.
    /// Delegates all business logic to the TodoService to maintain SRP and clean separation of concerns.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController(TodoService todoService) : ControllerBase
    {
        /// <summary>
        /// Retrieves all todos, optionally filtered by status.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetAll([FromQuery] TodoStatus? status = null)
        {
            var todos = await todoService.GetTodosAsync(status);
            return Ok(todos);
        }

        /// <summary>
        /// Retrieves a single Todo by its unique identifier.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetById(string id)
        {
            try
            {
                // Service layer handles existence validation
                var todo = (await todoService.GetTodosAsync()).FirstOrDefault(t => t.Id == id);

                if (todo == null)
                    throw new TodoNotFoundException(id);

                return Ok(todo);
            }
            catch (TodoNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new Todo.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Todo>> Create([FromBody] CreateTodoRequest request)
        {
            try
            {
                var todo = await todoService.CreateAsync(request.Title, request.Description);
                return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
            }
            catch (DuplicateTitleException ex)
            {
                // 409 Conflict: duplicate title
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing Todo’s title and/or description.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Todo>> Update(string id, [FromBody] UpdateTodoRequest request)
        {
            try
            {
                var updated = await todoService.UpdateAsync(id, request.Title, request.Description);
                return Ok(updated);
            }
            catch (DuplicateTitleException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (TodoNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the status of a Todo.
        /// Status transitions are validated in the domain layer.
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult<Todo>> UpdateStatus(string id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                // Fetch entity through service first
                var todo = (await todoService.GetTodosAsync()).FirstOrDefault(t => t.Id == id);
                if (todo == null)
                    throw new TodoNotFoundException(id);

                var updated = await todoService.UpdateStatusAsync(todo, request.Status);
                return Ok(updated);
            }
            catch (TodoNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidStatusTransitionException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a Todo by its unique identifier.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await todoService.DeleteAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (TodoNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
