import { useCallback, useEffect, useState } from 'react';
import { Todo, TodoStatus } from '../models/Todo';
import * as api from '../api/Todos';

// Hook to fetch and manage todos
export function useTodos(initialStatus?: TodoStatus) {
  const [todos, setTodos] = useState<Todo[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchAll = useCallback(async (status?: TodoStatus) => {
    setLoading(true);
    setError(null);
    try {
      const data = await api.fetchTodos(status);
      setTodos(data);
    } catch (e: any) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchAll(initialStatus);
  }, [fetchAll, initialStatus]);

  return { todos, loading, error, fetchAll, setTodos, setError };
}