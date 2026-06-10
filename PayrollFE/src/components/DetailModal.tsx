import React, { useState, useEffect } from 'react';

// Explicit type models mapping your backend API structure
interface ClassTaughtRecord {
  className: string;
  status: 'Completed' | 'Cancelled';
  bookingsCount: number;
  calculatedPayout: number;
}

interface CommissionRecord {
  itemType: string;
  amount: number;
  earned: number;
}

interface RefundAdjustmentRecord {
  itemType: string;
  amount: number;
  penalty: number;
}

interface BonusRecord {
  className: string;
  bonus: number;
}

interface InstructorDetailPayload {
  instructorName: string;
  classesTaught: ClassTaughtRecord[];
  commissionRecords: CommissionRecord[];
  refundAdjustments: RefundAdjustmentRecord[];
  bonusRecords: BonusRecord[];
}

interface Props {
  instructorId: number;
  startDate: string;
  endDate: string;
  onClose: () => void;
}

const DetailModal: React.FC<Props> = ({ instructorId, startDate, endDate, onClose }) => {
  // Configured state using explicit types instead of any
  const [details, setDetails] = useState<InstructorDetailPayload | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    fetch(`https://localhost:7289/api/payroll/${instructorId}?startDate=${startDate}&endDate=${endDate}`)
      .then((res) => res.json())
      .then((data: InstructorDetailPayload) => {
        setDetails(data);
        setLoading(false);
      })
      .catch(() => {
        setLoading(false);
      });
  }, [instructorId, startDate, endDate]);

  if (loading) {
    return (
      <div className="fixed inset-0 bg-slate-950/80 backdrop-blur-md flex items-center justify-center z-50">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-emerald-400"></div>
      </div>
    );
  }

  return (
    <div className="fixed inset-0 bg-slate-950/80 backdrop-blur-md flex items-center justify-center p-4 z-50">
      <div className="bg-slate-800 border border-slate-700 rounded-2xl w-full max-w-2xl max-h-[85vh] overflow-y-auto shadow-2xl p-6 flex flex-col">
        
        {/* Header */}
        <div className="flex justify-between items-center border-b border-slate-700 pb-4 mb-4">
          <h2 className="text-xl font-bold text-slate-100">
            Breakdown Matrix: <span className="text-emerald-400">{details?.instructorName}</span>
          </h2>
          <button onClick={onClose} className="text-slate-400 hover:text-white font-bold text-lg transition p-1">✕</button>
        </div>

        {/* Content Lists */}
        <div className="space-y-6 overflow-y-auto pr-1 flex-1">
          {/* Classes Taught */}
          <div>
            <h3 className="text-xs uppercase tracking-wider text-slate-400 font-bold mb-2">Classes Processed</h3>
            <div className="bg-slate-900 rounded-xl p-3 divide-y divide-slate-800 text-xs space-y-2">
              {!details?.classesTaught || details.classesTaught.length === 0 ? (
                <p className="text-slate-500 py-1">No classes taught.</p>
              ) : (
                details.classesTaught.map((c, i) => (
                  <div key={i} className="py-2 flex justify-between items-center first:pt-0 last:pb-0">
                    <div>
                      <p className="font-semibold text-slate-200">{c.className}</p>
                      <p className="text-slate-500">
                        {c.bookingsCount} bookings • status:{' '}
                        <span className={c.status === 'Cancelled' ? 'text-rose-400' : 'text-slate-400'}>
                          {c.status}
                        </span>
                      </p>
                    </div>
                    <span className="font-mono font-medium text-slate-300">SGD {c.calculatedPayout.toFixed(2)}</span>
                  </div>
                ))
              )}
            </div>
          </div>

          {/* Commissions */}
          <div>
            <h3 className="text-xs uppercase tracking-wider text-slate-400 font-bold mb-2">Commissions Earned</h3>
            <div className="bg-slate-900 rounded-xl p-3 text-xs space-y-2">
              {!details?.commissionRecords || details.commissionRecords.length === 0 ? (
                <p className="text-slate-500">None registered.</p>
              ) : (
                details.commissionRecords.map((s, i) => (
                  <div key={i} className="flex justify-between py-1 border-b border-slate-800/50 last:border-0">
                    <span className="text-slate-300">{s.itemType} (SGD {s.amount})</span>
                    <span className="font-mono text-cyan-400">+SGD {s.earned.toFixed(2)}</span>
                  </div>
                ))
              )}
            </div>
          </div>

          {/* Refund Adjustments */}
          <div>
            <h3 className="text-xs uppercase tracking-wider text-slate-400 font-bold mb-2">Refund Adjustments</h3>
            <div className="bg-slate-900 rounded-xl p-3 text-xs space-y-2">
              {!details?.refundAdjustments || details.refundAdjustments.length === 0 ? (
                <p className="text-slate-500">No adjustments recorded.</p>
              ) : (
                details.refundAdjustments.map((r, i) => (
                  <div key={i} className="flex justify-between text-rose-400 font-mono py-1 border-b border-slate-800/50 last:border-0">
                    <span>Clawback: {r.itemType} (SGD {r.amount})</span>
                    <span>-SGD {Math.abs(r.penalty).toFixed(2)}</span>
                  </div>
                ))
              )}
            </div>
          </div>

          {/* Attendance Bonus */}
          <div>
            <h3 className="text-xs uppercase tracking-wider text-slate-400 font-bold mb-2">Attendance Bonuses</h3>
            <div className="bg-slate-900 rounded-xl p-3 text-xs space-y-2">
              {!details?.bonusRecords || details.bonusRecords.length === 0 ? (
                <p className="text-slate-500">No threshold limits breached.</p>
              ) : (
                details.bonusRecords.map((b, i) => (
                  <div key={i} className="flex justify-between text-purple-400 py-1 border-b border-slate-800/50 last:border-0">
                    <span>High capacity tier bonus ({b.className})</span>
                    <span className="font-mono font-bold">+SGD {b.bonus.toFixed(2)}</span>
                  </div>
                ))
              )}
            </div>
          </div>
        </div>

        {/* Footer */}
        <div className="mt-6 border-t border-slate-700 pt-4 flex justify-end">
          <button 
            onClick={onClose} 
            className="bg-slate-700 hover:bg-slate-600 px-4 py-2 rounded-lg text-sm font-semibold transition text-slate-200"
          >
            Dismiss Breakdown
          </button>
        </div>
      </div>
    </div>
  );
};

export default DetailModal;