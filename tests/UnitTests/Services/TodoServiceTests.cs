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
    }
}