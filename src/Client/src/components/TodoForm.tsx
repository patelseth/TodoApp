import React, { useState } from 'react';

interface Props {
  onAdd: (title: string, description?: string) => void;
}

// Form for adding a new todo
export const TodoForm: React.FC<Props> = ({ onAdd }) => {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');

  return (
    <form
      onSubmit={e => {
        e.preventDefault();
        if (!title.trim()) return;
        onAdd(title, description);
        setTitle('');
        setDescription('');
      }}
    >
      <input
        value={title}
        onChange={e => setTitle(e.target.value)}
        placeholder="Title"
        required
      />
      <input
        value={description}
        onChange={e => setDescription(e.target.value)}
        placeholder="Description (optional)"
      />
      <button type="submit">Add Todo</button>
    </form>
  );
};