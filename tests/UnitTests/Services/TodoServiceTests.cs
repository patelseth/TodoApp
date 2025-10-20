using System.Threading.Tasks;
using Moq;
using Xunit;

using TodoApp.Application.Services;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Exceptions;

namespace TodoApp.UnitTests.Services
{
    /// <summary>
    /// Unit tests for TodoService using mocked repository.
    /// </summary>
    public class TodoServiceTests
    {
        #region Create Tests

        /// <summary>
        /// Verifies that duplicate titles are not allowed.
        /// </summary>
        [Fact]
        public async Task CreateAsync_WhenTitleAlreadyExists_ShouldThrowValidationException()
        {
            // Arrange: create mock repository
            var mockRepo = new Mock<ITodoRepository>();

            // Setup repository to simulate an existing Todo with same title
            mockRepo.Setup(r => r.GetByTitleAsync("Test Title"))
                    .ReturnsAsync(new Todo("Test Title" ));

            var service = new TodoService(mockRepo.Object);

            // Act + Assert: creation with duplicate title should throw
            await Assert.ThrowsAsync<DuplicateTitleException>(() =>
                service.CreateAsync("Test Title", "desc"));
        }

        #endregion

        #region Status Tests

        /// <summary>
        /// Verifies that attempting an invalid status transition
        /// (e.g., Pending -> Completed or InProgress -> Pending)
        /// throws an InvalidStatusTransitionException.
        /// </summary>
        [Fact]
        public void ChangeStatus_InvalidTransition_ShouldThrowInvalidStatusTransitionException()
        {
            // Arrange
            var todo = new Todo("Test Todo");

            // Act + Assert
            Assert.Throws<InvalidStatusTransitionException>(() => todo.ChangeStatus(TodoStatus.Completed));

            todo.ChangeStatus(TodoStatus.InProgress);

            Assert.Throws<InvalidStatusTransitionException>(() => todo.ChangeStatus(TodoStatus.Pending));
        }

        /// <summary>
        /// Verifies that valid status transitions (Pending -> InProgress -> Completed)
        /// succeed and update the Todo's status property accordingly.
        /// </summary>
        [Fact]
        public void ChangeStatus_ValidTransitions_ShouldUpdateStatus()
        {
            var todo = new Todo("Test Todo");

            todo.ChangeStatus(TodoStatus.InProgress);
            Assert.Equal(TodoStatus.InProgress, todo.Status);

            todo.ChangeStatus(TodoStatus.Completed);
            Assert.Equal(TodoStatus.Completed, todo.Status);
        }

        #endregion

        #region Update Tests

        /// <summary>
        /// Verifies that updating a Todo's title to one that already exists
        /// triggers a DuplicateTitleException.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenNewTitleAlreadyExists_ShouldThrowDuplicateTitleException()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();

            var existingTodo = new Todo("Original Title", "Original description");

            mockRepo.Setup(r => r.GetByIdAsync("1"))
                    .ReturnsAsync(existingTodo);

            mockRepo.Setup(r => r.GetByTitleAsync("Duplicate Title"))
                    .ReturnsAsync(new Todo("Duplicate Title"));

            var service = new TodoService(mockRepo.Object);

            // Act + Assert
            await Assert.ThrowsAsync<DuplicateTitleException>(() =>
                service.UpdateAsync("1", "Duplicate Title", "Updated description"));
        }

        /// <summary>
        /// Verifies that updating a Todo's title and description
        /// succeeds when validation passes.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenValid_ShouldUpdateTitleAndDescription()
        {
            var mockRepo = new Mock<ITodoRepository>();

            var existingTodo = new Todo("Old Title", "Old description");
            mockRepo.Setup(r => r.GetByIdAsync("1"))
                    .ReturnsAsync(existingTodo);

            mockRepo.Setup(r => r.GetByTitleAsync("New Title"))
                    .ReturnsAsync((Todo?)null);

            var service = new TodoService(mockRepo.Object);

            await service.UpdateAsync("1", "New Title", "New description");

            Assert.Equal("New Title", existingTodo.Title);
            Assert.Equal("New description", existingTodo.Description);
            mockRepo.Verify(r => r.UpdateAsync(existingTodo), Times.Once);
        }

        /// <summary>
        /// Verifies that if the user saves a Todo without changing
        /// its title or description, no duplicate validation occurs
        /// and no update operation is performed.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WhenValuesUnchanged_ShouldNotTriggerValidationOrUpdate()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var existingTodo = new Todo("Same Title", "Same description");

            mockRepo.Setup(r => r.GetByIdAsync("1"))
                    .ReturnsAsync(existingTodo);

            var service = new TodoService(mockRepo.Object);

            // Act
            await service.UpdateAsync("1", "Same Title", "Same description");

            // Assert
            mockRepo.Verify(r => r.GetByTitleAsync(It.IsAny<string>()), Times.Never);
            mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Todo>()), Times.Never);
        }

        #endregion
    }
}