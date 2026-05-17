import { createContext, useContext, useState, useCallback, type ReactNode } from 'react';
import type { LoginRequest } from '../types';
import { login as loginApi } from '../api/auth';

interface AuthState {
  token: string;
  username: string;
}

interface AuthContextType {
  auth: AuthState | null;
  login: (data: LoginRequest) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [auth, setAuth] = useState<AuthState | null>(() => {
    const token = localStorage.getItem('intelliflow_token');
    const username = localStorage.getItem('intelliflow_user');
    return token && username ? { token, username } : null;
  });

  const login = useCallback(async (data: LoginRequest) => {
    const res = await loginApi(data);
    localStorage.setItem('intelliflow_token', res.token);
    localStorage.setItem('intelliflow_user', res.username);
    setAuth({ token: res.token, username: res.username });
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('intelliflow_token');
    localStorage.removeItem('intelliflow_user');
    setAuth(null);
  }, []);

  return (
    <AuthContext.Provider value={{ auth, login, logout, isAuthenticated: !!auth }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
