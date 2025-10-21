import React, { useState } from 'react';
import { useTodos } from '../hooks/useTodos';
import { Todo, TodoStatus } from '../models/Todo';
import { TodoGrid } from '../components/TodoGrid';
import { TodoItem } from '../components/TodoItem';
import { TodoForm } from '../components/TodoForm';
import { Navbar } from '../components/Navbar';
import * as api from '../api/Todos';

// Main page for displaying and managing todos
export const Home: React.FC = () => {
  const [selected, setSelected] = useState<Todo | null>(null);
  const [filter, setFilter] = useState<TodoStatus | ''>('');
  const { todos, loading, error, fetchAll, setTodos, setError } = useTodos();

  // Add a new todo
  const handleAdd = async (title: string, description?: string) => {
    try {
      const todo = await api.createTodo(title, description);
      setTodos(prev => [...prev, todo]);
    } catch (e: any) {
      setError(e.message);
    }
  };

  // Update a todo
  const handleUpdate = async (id: string, title: string, description?: string) => {
    try {
      const updated = await api.updateTodo(id, title, description);
      setTodos(prev => prev.map(t => t.id === id ? updated : t));
      setSelected(updated);
    } catch (e: any) {
      setError(e.message);
    }
  };

  // Delete a todo
  const handleDelete = async (id: string) => {
    try {
      await api.deleteTodo(id);
      setTodos(prev => prev.filter(t => t.id !== id));
      setSelected(null);
    } catch (e: any) {
      setError(e.message);
    }
  };

  // Change status of a todo
  const handleStatusChange = async (id: string, status: TodoStatus) => {
    try {
      const updated = await api.updateTodoStatus(id, status);
      setTodos(prev => prev.map(t => t.id === id ? updated : t));
      setSelected(updated);
    } catch (e: any) {
      setError(e.message);
    }
  };

  // Filter todos by status
  const filteredTodos = filter ? todos.filter(t => t.status === filter) : todos;

  return (
    <div>
      <Navbar filter={filter} setFilter={setFilter} />
      <TodoForm onAdd={handleAdd} />
      {loading && <p>Loading...</p>}
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <TodoGrid todos={filteredTodos} onSelect={setSelected} />
      {selected && (
        <TodoItem
          todo={selected}
          onClose={() => setSelected(null)}
          onUpdate={handleUpdate}
          onDelete={handleDelete}
          onStatusChange={handleStatusChange}
        />
      )}
    </div>
  );
};