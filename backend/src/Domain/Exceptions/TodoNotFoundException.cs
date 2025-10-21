namespace Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a Todo with the specified ID is not found in the system.
    /// </summary>
    public class TodoNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the TodoNotFoundException
        /// with a standard message including the missing Todo's ID.
        /// </summary>
        /// <param name="id">The ID of the missing Todo</param>
        public TodoNotFoundException(string id)
            : base($"Todo with ID '{id}' was not found") { }
    }
}
