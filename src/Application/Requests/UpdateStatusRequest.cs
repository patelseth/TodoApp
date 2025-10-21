using System.Text.Json.Serialization;
using Domain.Entities;

namespace Application.Requests
{
    /// <summary>
    /// Request object for updating the status of an existing Todo.
    /// </summary>
    public record UpdateStatusRequest(TodoStatus Status);
}