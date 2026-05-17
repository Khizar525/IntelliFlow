import api from './client';
import type { TaskRequest, TaskResult } from '../types';

export async function submitTask(data: TaskRequest) {
  const res = await api.post<TaskResult>('/tasks', data);
  return res.data;
}
