import { useState, useRef, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import TaskForm from './TaskForm';
import PipelineTimeline from './PipelineTimeline';
import ResultCard from './ResultCard';
import type { TaskResult, PipelineStep } from '../types';

function usePipelineTimer(steps: PipelineStep[], phase: number) {
  return steps.map((s, i) => ({
    ...s,
    status: i < phase ? 'done' as const : i === phase ? 'active' as const : 'pending' as const,
  }));
}

export default function Dashboard() {
  const { auth, logout } = useAuth();
  const [steps, setSteps] = useState<PipelineStep[]>([]);
  const [phase, setPhase] = useState(0);
  const [result, setResult] = useState<TaskResult | null>(null);
  const [error, setError] = useState('');
  const [topic, setTopic] = useState('');
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const stopTimer = () => {
    if (timerRef.current !== null) {
      clearInterval(timerRef.current);
      timerRef.current = null;
    }
  };

  const handleStart = (pipeline: PipelineStep[]) => {
    setSteps(pipeline);
    setPhase(0);
    setResult(null);
    setError('');
    setTopic('');
    stopTimer();
    timerRef.current = setInterval(() => {
      setPhase((p) => {
        if (p >= pipeline.length - 1) {
          if (timerRef.current !== null) {
            clearInterval(timerRef.current);
            timerRef.current = null;
          }
        }
        return Math.min(p + 1, pipeline.length);
      });
    }, 1800);
  };

  const handleResult = (res: TaskResult) => {
    stopTimer();
    setPhase(steps.length);
    setResult(res);
  };

  const handleError = (msg: string) => {
    stopTimer();
    setError(msg);
  };

  useEffect(() => {
    return () => stopTimer();
  }, []);

  const displaySteps = usePipelineTimer(steps, phase);

  return (
    <div className="min-h-screen bg-gray-950">
      <header className="border-b border-white/5 bg-gray-950/80 backdrop-blur-xl sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-6 h-16 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-indigo-400 to-cyan-400 flex items-center justify-center text-white font-bold text-sm">IF</div>
            <span className="text-white font-semibold text-lg tracking-tight">IntelliFlow</span>
            <span className="hidden sm:inline text-[10px] font-medium text-indigo-400 bg-indigo-500/10 px-2 py-0.5 rounded-full border border-indigo-500/20 ml-2">Multi-Agent Pipeline</span>
          </div>
          <div className="flex items-center gap-4">
            <span className="text-gray-400 text-sm hidden sm:inline">{auth?.username}</span>
            <button onClick={logout} className="text-gray-400 hover:text-white text-sm transition-colors">Sign out</button>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-6 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-extrabold text-white">AI Research Pipeline</h1>
          <p className="text-gray-400 mt-1">Submit a topic and watch the multi-agent workflow execute in real-time</p>
        </div>

        <div className="grid lg:grid-cols-5 gap-6">
          <div className="lg:col-span-2 space-y-6">
            <TaskForm
              onStart={(pipeline) => {
                setTopic(pipeline.find(() => true) ? '' : '');
                handleStart(pipeline);
              }}
              onResult={handleResult}
              onError={handleError}
            />

            {result && <ResultCard result={result} topic={topic} />}

            {error && (
              <div className="bg-red-500/10 backdrop-blur-xl rounded-2xl border border-red-500/30 p-6">
                <div className="flex items-center gap-3 mb-2">
                  <span className="text-red-400 text-lg">✕</span>
                  <h3 className="text-red-300 font-semibold">Pipeline Failed</h3>
                </div>
                <p className="text-red-400/80 text-sm">{error}</p>
              </div>
            )}
          </div>

          <div className="lg:col-span-3">
            {steps.length > 0 && <PipelineTimeline steps={displaySteps} phase={phase} />}

            {steps.length === 0 && (
              <div className="bg-white/5 backdrop-blur-xl rounded-2xl border border-white/10 p-12 text-center">
                <div className="w-16 h-16 mx-auto mb-4 rounded-2xl bg-gradient-to-br from-indigo-500/20 to-cyan-500/20 flex items-center justify-center text-3xl">
                  🚀
                </div>
                <h3 className="text-white font-semibold text-lg mb-2">Ready for your first task</h3>
                <p className="text-gray-400 text-sm max-w-md mx-auto">
                  Enter a research topic and your email, then launch the pipeline. Our AI agents will handle the rest — from research to blockchain verification.
                </p>
              </div>
            )}
          </div>
        </div>
      </main>

      <footer className="border-t border-white/5 mt-12 py-6">
        <div className="max-w-7xl mx-auto px-6 text-center text-gray-600 text-xs">
          SEL 401 Cloud Computing Lab &middot; Bahria University Karachi &middot; Spring 2026
        </div>
      </footer>
    </div>
  );
}
