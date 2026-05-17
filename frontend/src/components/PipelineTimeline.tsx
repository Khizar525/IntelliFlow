import { useEffect, useState, useRef } from 'react';
import type { PipelineStep } from '../types';

interface Props {
  steps: PipelineStep[];
  phase: number;
}

const ICONS: Record<string, string> = {
  Research: '🔍',
  Summarize: '🧠',
  Report: '📄',
  Notify: '📧',
  Blockchain: '⛓️',
};

export default function PipelineTimeline({ steps, phase }: Props) {
  return (
    <div className="bg-white/5 backdrop-blur-xl rounded-2xl border border-white/10 p-6 shadow-xl">
      <h3 className="text-white font-semibold text-lg mb-6">Pipeline Progress</h3>
      <div className="space-y-0">
        {steps.map((step, i) => {
          const isActive = i === phase && phase < steps.length;
          const isDone = i < phase;
          const isPending = i > phase;

          return (
            <div key={step.name} className="relative flex items-start gap-4 pb-6 last:pb-0">
              {i < steps.length - 1 && (
                <div className={`absolute left-5 top-10 w-0.5 h-8 -translate-x-1/2 transition-colors duration-500 ${
                  isDone ? 'bg-indigo-500' : 'bg-white/10'
                }`} />
              )}
              <div className={`relative z-10 w-10 h-10 rounded-full flex items-center justify-center text-lg shrink-0 transition-all duration-500 ${
                isDone
                  ? 'bg-indigo-500 shadow-lg shadow-indigo-500/30'
                  : isActive
                  ? 'bg-gradient-to-r from-indigo-500 to-cyan-500 animate-pulse shadow-lg shadow-indigo-500/30'
                  : 'bg-white/5 border border-white/10'
              }`}>
                {isDone ? '✓' : ICONS[step.name] || '○'}
              </div>
              <div className="pt-1.5 flex-1 min-w-0">
                <div className="flex items-center gap-2">
                  <span className={`font-medium transition-colors duration-300 ${
                    isDone ? 'text-white' : isActive ? 'text-indigo-300' : 'text-gray-500'
                  }`}>
                    {step.name}
                  </span>
                  {isActive && (
                    <span className="text-[10px] font-medium text-cyan-400 bg-cyan-500/10 px-2 py-0.5 rounded-full border border-cyan-500/20 animate-pulse">
                      RUNNING
                    </span>
                  )}
                  {isDone && (
                    <span className="text-[10px] font-medium text-green-400 bg-green-500/10 px-2 py-0.5 rounded-full border border-green-500/20">
                      DONE
                    </span>
                  )}
                </div>
                <div className="text-xs text-gray-500 mt-0.5">{step.agent} &middot; {step.owner}</div>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
