import api from './client';
import type { LoginRequest, LoginResponse, HealthResponse } from '../types';

export async function login(data: LoginRequest) {
  const res = await api.post<LoginResponse>('/auth/login', data);
  return res.data;
}

export async function checkHealth() {
  const res = await api.get<HealthResponse>('/tasks/health');
  return res.data;
}
