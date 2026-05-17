import { useState, type FormEvent } from 'react';
import { submitTask } from '../api/tasks';
import type { TaskResult, PipelineStep } from '../types';

interface TaskFormProps {
  onStart: (steps: PipelineStep[]) => void;
  onResult: (result: TaskResult) => void;
  onError: (error: string) => void;
}

const PIPELINE_DEF: PipelineStep[] = [
  { name: 'Research', agent: 'DuckDuckGo API', owner: 'Hamza Khaliq', status: 'pending' },
  { name: 'Summarize', agent: 'OpenRouter (Llama 3)', owner: 'Hamza Khaliq', status: 'pending' },
  { name: 'Report', agent: 'Supabase Storage + PostgreSQL', owner: 'Hassan Asif', status: 'pending' },
  { name: 'Notify', agent: 'SMTP Email', owner: 'Shamraiz', status: 'pending' },
  { name: 'Blockchain', agent: 'Ethereum Sepolia', owner: 'Shamraiz', status: 'pending' },
];

export default function TaskForm({ onStart, onResult, onError }: TaskFormProps) {
  const [topic, setTopic] = useState('');
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!topic.trim() || !email.trim()) return;
    setLoading(true);
    onStart(PIPELINE_DEF);
    try {
      const result = await submitTask({ topic, notifyEmail: email });
      onResult(result);
    } catch (err: any) {
      onError(err.response?.data?.error || err.message || 'Pipeline failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="bg-white/5 backdrop-blur-xl rounded-2xl border border-white/10 p-6 shadow-xl">
      <h3 className="text-white font-semibold text-lg mb-4">Submit New Task</h3>
      <div className="space-y-4">
        <div>
          <label className="text-gray-300 text-sm font-medium block mb-1.5">Research Topic</label>
          <input
            value={topic}
            onChange={(e) => setTopic(e.target.value)}
            className="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white placeholder:text-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/50 focus:border-indigo-500 transition-all"
            placeholder="e.g. Quantum Computing, Climate Change..."
            required
            disabled={loading}
          />
        </div>
        <div>
          <label className="text-gray-300 text-sm font-medium block mb-1.5">Notification Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white placeholder:text-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/50 focus:border-indigo-500 transition-all"
            placeholder="you@example.com"
            required
            disabled={loading}
          />
        </div>
        <button
          type="submit"
          disabled={loading}
          className="w-full bg-gradient-to-r from-indigo-500 to-cyan-500 hover:from-indigo-400 hover:to-cyan-400 text-white font-semibold py-3 px-4 rounded-xl transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-indigo-500/25"
        >
          {loading ? (
            <span className="flex items-center justify-center gap-2">
              <svg className="animate-spin h-4 w-4" viewBox="0 0 24 24"><circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" /><path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" /></svg>
              Running pipeline...
            </span>
          ) : 'Launch Pipeline'}
        </button>
      </div>
    </form>
  );
}
