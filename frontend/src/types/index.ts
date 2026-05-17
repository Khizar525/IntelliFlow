export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresIn: string;
  username: string;
}

export interface TaskRequest {
  topic: string;
  notifyEmail: string;
}

export interface TaskResult {
  taskId: string;
  status: string;
  reportUrl: string;
  txHash: string;
  message: string;
}

export interface PipelineStep {
  name: string;
  agent: string;
  owner: string;
  status: 'pending' | 'active' | 'done' | 'error';
  detail?: string;
}

export interface HealthResponse {
  status: string;
}
