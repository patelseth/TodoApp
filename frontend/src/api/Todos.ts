import { Todo, TodoStatus } from '../models/Todo';

const API_URL = '/api/todo';

// Fetch all todos, optionally filtered by status
export async function fetchTodos(status?: TodoStatus): Promise<Todo[]> {
  const url = status ? `${API_URL}?status=${status}` : API_URL;
  const res = await fetch(url);
  if (!res.ok) throw new Error('Failed to fetch todos');
  return res.json();
}

// Fetch a single todo by ID
export async function fetchTodo(id: string): Promise<Todo> {
  const res = await fetch(`${API_URL}/${id}`);
  if (!res.ok) throw new Error('Todo not found');
  return res.json();
}

// Create a new todo
export async function createTodo(title: string, description?: string): Promise<Todo> {
  const res = await fetch(API_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ title, description }),
  });
  if (!res.ok) throw new Error((await res.json()).message || 'Failed to create todo');
  return res.json();
}

// Update an existing todo
export async function updateTodo(id: string, title: string, description?: string): Promise<Todo> {
  if (!title || !title.trim()) throw new Error('Title is required');
  const res = await fetch(`${API_URL}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ title, description }),
  });
  if (!res.ok) throw new Error((await res.json()).message || 'Failed to update todo');
  return res.json();
}

// Update the status of a todo
export async function updateTodoStatus(id: string, status: TodoStatus): Promise<Todo> {
  const res = await fetch(`${API_URL}/${id}/status`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ status }),
  });
  if (!res.ok) throw new Error((await res.json()).message || 'Invalid status transition');
  return res.json();
}

// Delete a todo
export async function deleteTodo(id: string): Promise<void> {
  const res = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
  if (!res.ok) throw new Error('Failed to delete todo');
}