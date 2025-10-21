import React, { useState } from 'react';
import { Todo, TodoStatus } from '../models/Todo';

interface Props {
  todo: Todo;
  onClose: () => void;
  onUpdate: (id: string, title: string, description?: string) => void;
  onDelete: (id: string) => void;
  onStatusChange: (id: string, status: TodoStatus) => void;
}

// Modal for viewing/editing a single todo
export const TodoItem: React.FC<Props> = ({ todo, onClose, onUpdate, onDelete, onStatusChange }) => {
  const [edit, setEdit] = useState(false);
  const [title, setTitle] = useState(todo.title);
  const [description, setDescription] = useState(todo.description || '');

  // Determine the next valid status
  const nextStatus = todo.status === 'Pending'
    ? 'InProgress'
    : todo.status === 'InProgress'
      ? 'Completed'
      : null;

  return (
    <div className="todo-modal">
      <button onClick={onClose}>Close</button>
      {edit ? (
        <form onSubmit={e => { e.preventDefault(); onUpdate(todo.id, title, description); setEdit(false); }}>
          <input value={title} onChange={e => setTitle(e.target.value)} required />
          <textarea value={description} onChange={e => setDescription(e.target.value)} />
          <button type="submit">Save</button>
        </form>
      ) : (
        <>
          <h2>{todo.title}</h2>
          <p>{todo.description}</p>
          <p>Status: {todo.status}</p>
          <p>Created: {new Date(todo.createdDate).toLocaleString()}</p>
          <p>Updated: {new Date(todo.updatedDate).toLocaleString()}</p>
          <button onClick={() => setEdit(true)}>Edit</button>
          <button onClick={() => onDelete(todo.id)}>Delete</button>
          {nextStatus && (
            <button onClick={() => onStatusChange(todo.id, nextStatus as TodoStatus)}>
              Mark as {nextStatus}
            </button>
          )}
        </>
      )}
    </div>
  );
};