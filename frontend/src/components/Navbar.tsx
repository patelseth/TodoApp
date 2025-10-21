import React from 'react';
import { TodoStatus } from '../models/Todo';

interface Props {
  filter: TodoStatus | '';
  setFilter: (status: TodoStatus | '') => void;
}

// Navbar for filtering todos by status
export const Navbar: React.FC<Props> = ({ filter, setFilter }) => (
  <nav className="navbar">
    <span>Filter:</span>
    <select value={filter} onChange={e => setFilter(e.target.value as TodoStatus | '')}>
      <option value="">All</option>
      <option value="Pending">Pending</option>
      <option value="InProgress">In Progress</option>
      <option value="Completed">Completed</option>
    </select>
  </nav>
);