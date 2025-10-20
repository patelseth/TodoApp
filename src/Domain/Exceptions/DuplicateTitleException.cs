using System;

namespace TodoApp.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when attempting to create a Todo
    /// with a title that already exists in the system.
    /// </summary>
    public class DuplicateTitleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the DuplicateTitleException
        /// with a standard message indicating the title is already taken.
        /// </summary>
        public DuplicateTitleException()
            : base("A Todo with the same title already exists") { }
    }
}