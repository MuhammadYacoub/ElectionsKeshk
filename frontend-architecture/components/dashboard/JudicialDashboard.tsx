import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
// Simulated Shadcn UI Imports - Assuming standard accessible primitives
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent
} from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Progress } from '@/components/ui/progress';
import { ScrollArea } from '@/components/ui/scroll-area';

// --- Mock Data & Types ---
type LogEvent = {
  id: string;
  timestamp: string;
  voterHash: string;
  terminalId: string;
  status: 'VOTED' | 'VERIFIED' | 'ANOMALY';
};

type TerminalStatus = {
  id: string;
  location: string;
  status: 'ONLINE' | 'OFFLINE' | 'WARNING';
  latency: number;
};

const MOCK_LOGS: LogEvent[] = [
  { id: '1', timestamp: '10:42:01', voterHash: '0x3F...9A2B', terminalId: 'T-04', status: 'VOTED' },
  { id: '2', timestamp: '10:41:55', voterHash: '0x8C...11EF', terminalId: 'T-12', status: 'VERIFIED' },
  { id: '3', timestamp: '10:41:50', voterHash: '0x1A...B8C3', terminalId: 'T-04', status: 'VOTED' },
  { id: '4', timestamp: '10:40:12', voterHash: 'INVALID_SIG', terminalId: 'T-09', status: 'ANOMALY' },
  { id: '5', timestamp: '10:39:44', voterHash: '0x99...4D22', terminalId: 'T-01', status: 'VOTED' },
];

const MOCK_TERMINALS: TerminalStatus[] = [
  { id: 'T-01', location: 'القاعة الرئيسية - أ', status: 'ONLINE', latency: 12 },
  { id: 'T-04', location: 'القاعة الرئيسية - ب', status: 'ONLINE', latency: 15 },
  { id: 'T-09', location: 'مدخل كبار الزوار', status: 'WARNING', latency: 150 },
  { id: 'T-12', location: 'اللجنة الفرعية 3', status: 'OFFLINE', latency: 0 },
];

export default function JudicialDashboard() {
  const [logs, setLogs] = useState<LogEvent[]>(MOCK_LOGS);
  const [isEmergencyLocked, setIsEmergencyLocked] = useState(false);
  const [quorumPercentage, setQuorumPercentage] = useState(45);

  // Simulate real-time data influx
  useEffect(() => {
    if (isEmergencyLocked) return;

    const interval = setInterval(() => {
      setQuorumPercentage(prev => Math.min(prev + 0.5, 100));

      const newLog: LogEvent = {
        id: Math.random().toString(36).substr(2, 9),
        timestamp: new Date().toLocaleTimeString('ar-EG', { hour12: false }),
        voterHash: `0x${Math.floor(Math.random()*16777215).toString(16).toUpperCase()}...`,
        terminalId: `T-0${Math.floor(Math.random() * 5) + 1}`,
        status: Math.random() > 0.9 ? 'ANOMALY' : 'VOTED'
      };

      setLogs(prev => [newLog, ...prev].slice(0, 50)); // Keep last 50
    }, 3000);
    return () => clearInterval(interval);
  }, [isEmergencyLocked]);

  return (
    <div dir="rtl" className="min-h-screen bg-slate-50 text-slate-900 font-arabic overflow-hidden flex flex-col">
      {/* Top Navigation / Status Bar */}
      <header className="h-16 bg-[#0B1C3C] text-white shadow-md flex items-center justify-between px-6 border-b-4 border-[#C5A059]">
        <div className="flex items-center gap-4">
          <div className="w-10 h-10 bg-[#C5A059] rounded-full flex items-center justify-center font-bold text-[#0B1C3C]">
            هـ.ق
          </div>
          <div>
            <h1 className="font-bold text-lg leading-tight">هيئة قضايا الدولة</h1>
            <p className="text-xs text-slate-300 opacity-80">لوحة التحكم القضائية المركزية</p>
          </div>
        </div>
        <div className="flex items-center gap-6">
          <div className="text-sm">
            <span className="opacity-70 mx-2">المعرف القضائي:</span>
            <span className="font-mono bg-slate-800 px-2 py-1 rounded text-[#C5A059]">JUD-8842-A</span>
          </div>
          <Badge variant={isEmergencyLocked ? 'destructive' : 'default'} className={isEmergencyLocked ? 'bg-red-600 animate-pulse' : 'bg-green-600'}>
            {isEmergencyLocked ? 'النظام متوقف (طوارئ)' : 'الانتخابات جارية'}
          </Badge>
        </div>
      </header>

      {/* Main Content Grid */}
      <main className="flex-1 p-6 grid grid-cols-12 gap-6 h-[calc(100vh-4rem)] overflow-hidden">

        {/* Left Column: Metrics & Terminals (4 Cols) */}
        <div className="col-span-12 lg:col-span-4 flex flex-col gap-6 h-full">

          {/* Quorum Metric */}
          <Card className="shadow-sm border-slate-200">
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-bold text-slate-500">النصاب القانوني للجمعية العمومية</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex justify-between items-end mb-2">
                <span className="text-4xl font-numerals font-bold text-[#0B1C3C]">{quorumPercentage.toFixed(1)}%</span>
                <span className="text-sm text-slate-500">المطلوب: 50% + 1</span>
              </div>
              <Progress
                value={quorumPercentage}
                className={`h-3 ${quorumPercentage >= 50 ? '[&>div]:bg-green-600' : '[&>div]:bg-[#0B1C3C]'}`}
              />
              {quorumPercentage >= 50 && (
                <p className="text-xs text-green-600 mt-2 font-bold flex items-center gap-1">
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 13l4 4L19 7"></path></svg>
                  اكتمل النصاب القانوني
                </p>
              )}
            </CardContent>
          </Card>

          {/* Terminal Status */}
          <Card className="flex-1 flex flex-col shadow-sm border-slate-200 overflow-hidden">
            <CardHeader className="bg-slate-100/50 pb-3 border-b">
              <CardTitle className="text-md font-bold text-[#0B1C3C]">حالة اللجان الفرعية (الأكشاك)</CardTitle>
            </CardHeader>
            <CardContent className="p-0 flex-1 overflow-hidden">
              <ScrollArea className="h-full px-4 py-2">
                <div className="space-y-3">
                  {MOCK_TERMINALS.map(term => (
                    <div key={term.id} className="flex items-center justify-between p-3 rounded-md bg-white border border-slate-100 shadow-sm transition-colors hover:bg-slate-50">
                      <div className="flex items-center gap-3">
                        <div className={`w-3 h-3 rounded-full ${
                          term.status === 'ONLINE' ? 'bg-green-500 shadow-[0_0_8px_rgba(34,197,94,0.5)]' :
                          term.status === 'WARNING' ? 'bg-yellow-500 animate-pulse' : 'bg-red-500'
                        }`} />
                        <div>
                          <p className="text-sm font-bold text-slate-800">{term.location}</p>
                          <p className="text-xs font-mono text-slate-500">{term.id}</p>
                        </div>
                      </div>
                      <div className="text-end">
                        <Badge variant="outline" className={`text-[10px] ${
                          term.status === 'ONLINE' ? 'text-green-700 border-green-200 bg-green-50' :
                          term.status === 'WARNING' ? 'text-yellow-700 border-yellow-200 bg-yellow-50' : 'text-red-700 border-red-200 bg-red-50'
                        }`}>
                          {term.status === 'ONLINE' ? 'متصل' : term.status === 'WARNING' ? 'بطء استجابة' : 'غير متصل'}
                        </Badge>
                        <p className="text-[10px] text-slate-400 mt-1 font-numerals">{term.latency}ms</p>
                      </div>
                    </div>
                  ))}
                </div>
              </ScrollArea>
            </CardContent>
          </Card>
        </div>

        {/* Right Column: Live Cryptographic Log (8 Cols) */}
        <div className="col-span-12 lg:col-span-8 h-full flex flex-col">
          <Card className="flex-1 flex flex-col shadow-sm border-slate-200 overflow-hidden relative">
            <CardHeader className="bg-slate-100/50 pb-3 border-b flex flex-row items-center justify-between">
              <div>
                <CardTitle className="text-md font-bold text-[#0B1C3C]">سجل التدقيق المباشر (Cryptographic Log)</CardTitle>
                <p className="text-xs text-slate-500 mt-1">يتم عرض أحدث العمليات بشكل فوري وآمن.</p>
              </div>
              <div className="flex items-center gap-2">
                <span className="relative flex h-3 w-3">
                  {!isEmergencyLocked && <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-green-400 opacity-75"></span>}
                  <span className={`relative inline-flex rounded-full h-3 w-3 ${isEmergencyLocked ? 'bg-red-500' : 'bg-green-500'}`}></span>
                </span>
                <span className="text-xs font-bold text-slate-600">{isEmergencyLocked ? 'متوقف' : 'مزامنة حية...'}</span>
              </div>
            </CardHeader>
            <CardContent className="p-0 flex-1 overflow-hidden relative bg-white">

              {/* Table Header */}
              <div className="grid grid-cols-5 text-xs font-bold text-slate-500 bg-slate-50 border-b border-slate-200 p-3 sticky top-0 z-10">
                <div className="col-span-1">الوقت</div>
                <div className="col-span-2">البصمة المشفرة (Hash)</div>
                <div className="col-span-1 text-center">المحطة</div>
                <div className="col-span-1 text-end">الحالة</div>
              </div>

              {/* Virtualized Log List Simulation */}
              <ScrollArea className="h-full">
                <div className="p-2">
                  <AnimatePresence initial={false}>
                    {logs.map((log) => (
                      <motion.div
                        key={log.id}
                        initial={{ opacity: 0, y: -20, backgroundColor: '#f8fafc' }}
                        animate={{ opacity: 1, y: 0, backgroundColor: '#ffffff' }}
                        transition={{ duration: 0.3 }}
                        className="grid grid-cols-5 items-center text-sm p-3 border-b border-slate-100 hover:bg-slate-50 transition-colors"
                      >
                        <div className="col-span-1 font-numerals text-slate-500">{log.timestamp}</div>
                        <div className="col-span-2 font-mono text-xs text-slate-700 bg-slate-100 py-1 px-2 rounded w-fit">{log.voterHash}</div>
                        <div className="col-span-1 text-center font-mono text-xs">{log.terminalId}</div>
                        <div className="col-span-1 flex justify-end">
                          <Badge className={`${
                            log.status === 'VOTED' ? 'bg-blue-100 text-blue-800 hover:bg-blue-200 border-transparent' :
                            log.status === 'VERIFIED' ? 'bg-green-100 text-green-800 hover:bg-green-200 border-transparent' :
                            'bg-red-100 text-red-800 hover:bg-red-200 border-transparent'
                          }`}>
                            {log.status === 'VOTED' ? 'تم التصويت' : log.status === 'VERIFIED' ? 'تم التحقق' : 'شذوذ أمني'}
                          </Badge>
                        </div>
                      </motion.div>
                    ))}
                  </AnimatePresence>
                </div>
              </ScrollArea>

              {/* Emergency Overlay */}
              {isEmergencyLocked && (
                <div className="absolute inset-0 bg-red-900/10 backdrop-blur-sm flex items-center justify-center z-20">
                  <div className="bg-white p-6 rounded-lg shadow-2xl border-2 border-red-500 text-center max-w-md">
                    <svg className="w-16 h-16 text-red-600 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"></path></svg>
                    <h2 className="text-2xl font-bold text-red-700 mb-2">تم إيقاف النظام (قفل طوارئ)</h2>
                    <p className="text-slate-600 mb-6">بناءً على أمر قضائي، تم تعليق استقبال الأصوات. يرجى مراجعة سجلات التدقيق.</p>
                    <Button onClick={() => setIsEmergencyLocked(false)} className="bg-[#0B1C3C] hover:bg-[#1a2f5e] w-full">
                      استئناف النظام (يتطلب توقيع رقمي)
                    </Button>
                  </div>
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </main>

      {/* Speed Dial / Emergency Override Button */}
      {!isEmergencyLocked && (
        <div className="fixed bottom-8 left-8 z-50">
          <Button
            onClick={() => setIsEmergencyLocked(true)}
            size="lg"
            className="w-16 h-16 rounded-full bg-red-600 hover:bg-red-700 shadow-[0_0_20px_rgba(220,38,38,0.5)] flex items-center justify-center hover:scale-105 transition-transform"
            title="إيقاف طوارئ"
          >
            <svg className="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 10a1 1 0 011-1h4a1 1 0 011 1v4a1 1 0 01-1 1h-4a1 1 0 01-1-1v-4z"></path></svg>
          </Button>
        </div>
      )}
    </div>
  );
}
