using System;

namespace TodoApp.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when an invalid status transition is attempted on a Todo.
    /// </summary>
    public class InvalidStatusTransitionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the InvalidStatusTransitionException
        /// with a standard message indicating the status transition is invalid.
        /// </summary>
        public InvalidStatusTransitionException()
            : base("Invalid status transition attempted on Todo item.") { }
    }
}