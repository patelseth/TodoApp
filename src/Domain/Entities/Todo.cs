using System;

namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Represents a Todo item.
    /// </summary>
    public class Todo
    {
        public string Title { get; set; } // Required field used in duplicate-title validation
        public string? Description { get; set; } // Optional description

        /// <summary>
        /// Initializes a new Todo with title and optional description.
        /// </summary>
        /// <param name="title">Title of the todo</param>
        /// <param name="description">Optional description</param>
        public Todo(string title, string? description = null)
        {
            Title = title;
            Description = description;
        }
    }
}