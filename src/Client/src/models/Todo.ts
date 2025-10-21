// The Todo type and allowed statuses
export type TodoStatus = 'Pending' | 'InProgress' | 'Completed';

export interface Todo {
  id: string;
  title: string;
  description?: string;
  status: TodoStatus;
  createdDate: string;
  updatedDate: string;
}