import type { TaskResult } from '../types';

interface Props {
  result: TaskResult;
  topic: string;
}

export default function ResultCard({ result, topic }: Props) {
  return (
    <div className="bg-white/5 backdrop-blur-xl rounded-2xl border border-white/10 p-6 shadow-xl">
      <div className="flex items-center gap-3 mb-5">
        <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-green-400 to-emerald-500 flex items-center justify-center text-white text-lg">✓</div>
        <div>
          <h3 className="text-white font-semibold text-lg">Pipeline Complete</h3>
          <p className="text-gray-400 text-sm">Task executed successfully</p>
        </div>
      </div>

      <div className="space-y-3">
        <div className="bg-white/5 rounded-xl border border-white/5 p-4">
          <div className="text-gray-400 text-xs font-medium uppercase tracking-wider mb-1">Topic</div>
          <div className="text-white font-medium">{topic}</div>
        </div>

        <div className="bg-white/5 rounded-xl border border-white/5 p-4">
          <div className="text-gray-400 text-xs font-medium uppercase tracking-wider mb-1">Task ID</div>
          <code className="text-indigo-300 text-sm font-mono">{result.taskId}</code>
        </div>

        <div className="bg-white/5 rounded-xl border border-white/5 p-4">
          <div className="text-gray-400 text-xs font-medium uppercase tracking-wider mb-1">Report URL</div>
          <a href={result.reportUrl} target="_blank" rel="noreferrer"
             className="text-cyan-400 hover:text-cyan-300 text-sm break-all transition-colors">
            {result.reportUrl}
          </a>
        </div>

        <div className="bg-white/5 rounded-xl border border-white/5 p-4">
          <div className="text-gray-400 text-xs font-medium uppercase tracking-wider mb-1">Blockchain Transaction</div>
          <div className="flex items-center gap-2">
            <code className="text-amber-300 text-sm font-mono truncate">{result.txHash}</code>
            <span className="text-[10px] font-medium text-amber-500 bg-amber-500/10 px-2 py-0.5 rounded-full border border-amber-500/20 shrink-0">SEPOLIA</span>
          </div>
        </div>
      </div>
    </div>
  );
}
