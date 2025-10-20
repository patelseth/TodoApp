namespace TodoApp.UnitTests.Services
{
    public class TodoServiceTests
    {
        [Fact]
        public async Task CreateAsync_WhenTitleAlreadyExists_ShouldThrowValidationException()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();

            // Repo returns a todo if title already exists
            mockRepo.Setup(r => r.GetByTitleAsync("Test Title"))
                    .ReturnsAsync(new Todo { Title = "Test Title" });

            var service = new TodoService(mockRepo.Object);

            // Act + Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                service.CreateAsync("Test Title", "desc"));
        }
    }
}
