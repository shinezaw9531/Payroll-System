import React, { useState, useEffect } from 'react';
import DetailModal from './DetailModal';

export interface PayrollSummary {
  instructorId: number;
  instructorName: string;
  classEarnings: number;
  commission: number;
  bonus: number;
  adjustment: number;
  finalPayout: number;
}

const PayrollDashboard: React.FC = () => {
  const [startDate, setStartDate] = useState<string>('2026-06-01');
  const [endDate, setEndDate] = useState<string>('2026-06-30');
  const [summaries, setSummaries] = useState<PayrollSummary[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [sortBy, setSortBy] = useState<'name' | 'payout'>('name');
  const [selectedInstructorId, setSelectedInstructorId] = useState<number | null>(null);

  const API_BASE = 'https://localhost:7289/api/payroll';

  const fetchPayroll = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await fetch(`${API_BASE}?startDate=${startDate}&endDate=${endDate}`);
      if (!res.ok) throw new Error('Failed to fetch summary data records.');
      const data = await res.json() as PayrollSummary[];
      setSummaries(data);
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('Something went wrong');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleGeneratePayroll = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await fetch(`${API_BASE}/generate`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ startDate, endDate }),
      });
      if (!res.ok) throw new Error('Generation failed.');
      await fetchPayroll();
    } catch (err) {
      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('Generation pipeline execution failed.');
      }
      setLoading(false);
    }
  };

  // Fixed useEffect lifecycle loop initialization block
  useEffect(() => {
    let isMounted = true;

    const initializeDashboard = async () => {
      // Defers initial synchronization until the next execution macro-task frame
      await new Promise((resolve) => setTimeout(resolve, 0));
      
      if (isMounted) {
        fetchPayroll();
      }
    };

    initializeDashboard();

    return () => {
      isMounted = false;
    };
  }, []);

  const sortedSummaries = [...summaries].sort((a, b) => {
    if (sortBy === 'name') return a.instructorName.localeCompare(b.instructorName);
    return b.finalPayout - a.finalPayout;
  });

  return (
    <div className="min-h-screen bg-slate-900 text-slate-100 p-4 sm:p-6 font-sans">
      <div className="max-w-6xl mx-auto">
        
        {/* Header Panel */}
        <header className="mb-8 flex flex-col lg:flex-row justify-between items-start lg:items-center gap-4 border-b border-slate-800 pb-6">
          <div>
            <h1 className="text-3xl font-extrabold text-emerald-400 tracking-tight">Rezerv Studio Payroll</h1>
            <p className="text-slate-400 mt-1">Manage instructor processing engines and ledger controls.</p>
          </div>

          <div className="flex flex-wrap items-center gap-3 bg-slate-800 p-3 rounded-xl border border-slate-700 shadow-xl w-full lg:w-auto">
            <input 
              type="date" 
              value={startDate} 
              onChange={(e) => setStartDate(e.target.value)} 
              className="bg-slate-900 text-white rounded p-2 border border-slate-600 text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500 flex-1 sm:flex-none"
            />
            <input 
              type="date" 
              value={endDate} 
              onChange={(e) => setEndDate(e.target.value)} 
              className="bg-slate-900 text-white rounded p-2 border border-slate-600 text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500 flex-1 sm:flex-none"
            />
            <button 
              onClick={handleGeneratePayroll}
              disabled={loading}
              className="bg-emerald-500 hover:bg-emerald-600 disabled:opacity-50 text-slate-950 font-semibold text-sm px-4 py-2 rounded-lg transition w-full sm:w-auto"
            >
              Run Calculations
            </button>
          </div>
        </header>

        {error && (
          <div className="bg-rose-500/10 border border-rose-500/30 text-rose-400 p-4 rounded-xl mb-6 text-sm">
            ⚠️ {error}
          </div>
        )}

        {/* Sorting Toggles */}
        <div className="flex gap-4 mb-4 items-center justify-end">
          <span className="text-xs font-semibold uppercase tracking-wider text-slate-400">Sort Matrix:</span>
          <button 
            onClick={() => setSortBy('name')} 
            className={`text-xs px-3 py-1 rounded border transition ${sortBy === 'name' ? 'bg-emerald-500 text-slate-950 border-emerald-400 font-bold' : 'border-slate-700 hover:bg-slate-800 text-slate-400'}`}
          >
            Name
          </button>
          <button 
            onClick={() => setSortBy('payout')} 
            className={`text-xs px-3 py-1 rounded border transition ${sortBy === 'payout' ? 'bg-emerald-500 text-slate-950 border-emerald-400 font-bold' : 'border-slate-700 hover:bg-slate-800 text-slate-400'}`}
          >
            Final Payout
          </button>
        </div>

        {/* Content Canvas */}
        {loading ? (
          <div className="flex flex-col items-center justify-center py-20 bg-slate-800/40 rounded-2xl border border-slate-800">
            <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-emerald-400"></div>
            <p className="text-slate-400 mt-4 text-sm font-medium">Re-computing balances and auditing items...</p>
          </div>
        ) : sortedSummaries.length === 0 ? (
          <div className="text-center py-20 bg-slate-800/40 rounded-2xl border border-slate-800 text-slate-400">
            📭 No operational summaries computed for this period block configuration.
          </div>
        ) : (
          <div className="overflow-x-auto bg-slate-800/60 rounded-2xl border border-slate-700 shadow-2xl backdrop-blur-sm">
            <table className="w-full text-left border-collapse min-w-[700px]">
              <thead>
                <tr className="border-b border-slate-700 bg-slate-800/70 text-slate-300 text-xs uppercase tracking-wider font-semibold">
                  <th className="p-4">Instructor</th>
                  <th className="p-4 text-right">Class Earnings</th>
                  <th className="p-4 text-right">Commission</th>
                  <th className="p-4 text-right">Bonus</th>
                  <th className="p-4 text-right">Adjustment</th>
                  <th className="p-4 text-right text-emerald-400">Final Payout</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-700/50 text-sm">
                {sortedSummaries.map((item) => (
                  <tr 
                    key={item.instructorId} 
                    onClick={() => setSelectedInstructorId(item.instructorId)}
                    className="hover:bg-slate-700/40 cursor-pointer transition duration-150 group"
                  >
                    <td className="p-4 font-semibold text-slate-200 group-hover:text-emerald-400 transition">
                      {item.instructorName}
                    </td>
                    <td className="p-4 text-right font-mono">SGD {item.classEarnings.toFixed(2)}</td>
                    <td className="p-4 text-right font-mono text-cyan-400">SGD {item.commission.toFixed(2)}</td>
                    <td className="p-4 text-right font-mono text-purple-400">SGD {item.bonus.toFixed(2)}</td>
                    <td className="p-4 text-right font-mono text-rose-400">
                      {item.adjustment < 0 ? `-SGD ${Math.abs(item.adjustment).toFixed(2)}` : `SGD ${item.adjustment.toFixed(2)}`}
                    </td>
                    <td className="p-4 text-right font-bold font-mono text-emerald-400">SGD {item.finalPayout.toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {selectedInstructorId && (
        <DetailModal 
          instructorId={selectedInstructorId} 
          startDate={startDate}
          endDate={endDate}
          onClose={() => setSelectedInstructorId(null)} 
        />
      )}
    </div>
  );
};

export default PayrollDashboard;