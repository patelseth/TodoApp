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
    }
}