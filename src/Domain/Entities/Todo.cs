using System;
using TodoApp.Domain.Exceptions;

namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Represents a Todo item with title, optional description, status, and timestamps.
    /// </summary>
    public class Todo
    {
        /// <summary>
        /// Required field used in duplicate-title validation.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Optional field for additional description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Current status of the Todo.
        /// </summary>
        public TodoStatus Status { get; private set; }

        /// <summary>
        /// Date and time when the Todo was created.
        /// Set at instantiation.
        /// </summary>
        public DateTime CreatedDate { get; private set; }

        /// <summary>
        /// Date and time when the Todo was last updated.
        /// Updated on any domain-level change (e.g., status change).
        /// </summary>
        public DateTime UpdatedDate { get; private set; }

        /// <summary>
        /// Initializes a new Todo with a title and optional description.
        /// Status is set to Pending by default, and timestamps are set to now.
        /// </summary>
        /// <param name="title">Title of the todo</param>
        /// <param name="description">Optional description</param>
        public Todo(string title, string? description = null)
        {
            Title = title;
            Description = description;
            Status = TodoStatus.Pending;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Attempts to change the status of the Todo.
        /// Throws InvalidStatusTransitionException if the transition is not allowed.
        /// Updates UpdatedDate on successful change.
        /// </summary>
        /// <param name="newStatus">The new status to apply</param>
        public void ChangeStatus(TodoStatus newStatus)
        {
            if (!IsValidTransition(newStatus))
                throw new InvalidStatusTransitionException();

            Status = newStatus;
            UpdatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Determines whether a transition from current status to newStatus is valid.
        /// Only allows Pending -> InProgress -> Completed.
        /// </summary>
        /// <param name="newStatus">The status to transition to</param>
        /// <returns>True if transition is valid, false otherwise</returns>
        private bool IsValidTransition(TodoStatus newStatus)
        {
            return (Status == TodoStatus.Pending && newStatus == TodoStatus.InProgress) ||
                   (Status == TodoStatus.InProgress && newStatus == TodoStatus.Completed);
        }
    }
}