import React from 'react';
import { Todo } from '../models/Todo';
import './TodoGrid.css';

interface Props {
  todos: Todo[];
  onSelect: (todo: Todo) => void;
}

// Grid to display todos
export const TodoGrid: React.FC<Props> = ({ todos, onSelect }) => (
  <div className="todo-grid">
    {todos.map(todo => (
      <div key={todo.id} className="todo-card" onClick={() => onSelect(todo)}>
        <h3>{todo.title}</h3>
        <p>Status: {todo.status}</p>
      </div>
    ))}
  </div>
);