import { useState, type FormEvent } from 'react';
import { useAuth } from '../context/AuthContext';

const STATS = [
  { label: 'AI Models', value: 'Llama 3 / Gemma' },
  { label: 'Blockchain', value: 'Ethereum Sepolia' },
  { label: 'Cloud', value: 'Supabase + Render' },
  { label: 'Storage', value: 'PostgreSQL + Blob' },
];

export default function LoginPage() {
  const { login } = useAuth();
  const [username, setUsername] = useState('admin');
  const [password, setPassword] = useState('intelliflow123');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try {
      await login({ username, password });
    } catch {
      setError('Invalid credentials. Try admin / intelliflow123');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex relative overflow-hidden bg-gray-950">
      <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-indigo-900/20 via-transparent to-cyan-900/20" />
      <div className="absolute top-[-20%] right-[-10%] w-[40rem] h-[40rem] bg-indigo-500/10 rounded-full blur-3xl" />
      <div className="absolute bottom-[-20%] left-[-10%] w-[40rem] h-[40rem] bg-cyan-500/10 rounded-full blur-3xl" />

      <div className="hidden lg:flex flex-1 flex-col justify-center px-20 relative">
        <div className="max-w-lg">
          <div className="flex items-center gap-3 mb-8">
            <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-indigo-400 to-cyan-400 flex items-center justify-center text-white font-bold text-lg">IF</div>
            <span className="text-white font-semibold text-xl tracking-tight">IntelliFlow</span>
          </div>
          <h1 className="text-5xl font-extrabold text-white leading-tight mb-4">
            Multi-Agent AI<br />
            <span className="bg-gradient-to-r from-indigo-400 via-purple-400 to-cyan-400 bg-clip-text text-transparent">Task Automation</span>
          </h1>
          <p className="text-gray-400 text-lg leading-relaxed mb-10">
            Submit a research topic. Our AI agents research, summarize, generate a report,
            deliver it via email, and log an immutable audit trail on the Ethereum blockchain.
          </p>
          <div className="grid grid-cols-2 gap-4">
            {STATS.map((s) => (
              <div key={s.label} className="bg-white/5 rounded-xl border border-white/10 p-4">
                <div className="text-gray-400 text-sm">{s.label}</div>
                <div className="text-white font-semibold mt-1">{s.value}</div>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className="flex-1 flex items-center justify-center relative">
        <div className="w-full max-w-sm px-6">
          <div className="lg:hidden flex items-center gap-3 mb-10 justify-center">
            <div className="w-9 h-9 rounded-xl bg-gradient-to-br from-indigo-400 to-cyan-400 flex items-center justify-center text-white font-bold">IF</div>
            <span className="text-white font-semibold text-lg">IntelliFlow</span>
          </div>

          <div className="bg-white/5 backdrop-blur-xl rounded-2xl border border-white/10 p-8 shadow-2xl">
            <h2 className="text-white text-2xl font-bold mb-1">Welcome back</h2>
            <p className="text-gray-400 text-sm mb-8">Sign in to access the pipeline</p>

            <form onSubmit={handleSubmit} className="space-y-5">
              <div>
                <label className="text-gray-300 text-sm font-medium block mb-2">Username</label>
                <input
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  className="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white placeholder:text-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/50 focus:border-indigo-500 transition-all"
                  placeholder="Enter username"
                  required
                />
              </div>
              <div>
                <label className="text-gray-300 text-sm font-medium block mb-2">Password</label>
                <input
                  type="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white placeholder:text-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/50 focus:border-indigo-500 transition-all"
                  placeholder="Enter password"
                  required
                />
              </div>

              {error && (
                <div className="bg-red-500/10 border border-red-500/30 rounded-lg px-4 py-3 text-red-400 text-sm">
                  {error}
                </div>
              )}

              <button
                type="submit"
                disabled={loading}
                className="w-full bg-gradient-to-r from-indigo-500 to-cyan-500 hover:from-indigo-400 hover:to-cyan-400 text-white font-semibold py-3 px-4 rounded-xl transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-indigo-500/25"
              >
                {loading ? (
                  <span className="flex items-center justify-center gap-2">
                    <svg className="animate-spin h-4 w-4" viewBox="0 0 24 24"><circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" /><path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" /></svg>
                    Signing in...
                  </span>
                ) : 'Sign in'}
              </button>
            </form>

            <div className="mt-6 pt-6 border-t border-white/5 text-center">
              <p className="text-gray-500 text-xs">
                SEL 401 Cloud Computing Lab &middot; Bahria University Karachi
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
