import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
// Simulated Shadcn UI Imports
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

      setLogs(prev => [newLog, ...prev].slice(0, 50));
    }, 3000);
    return () => clearInterval(interval);
  }, [isEmergencyLocked]);

  return (
    <div dir="rtl" className="min-h-screen bg-background text-foreground font-arabic overflow-hidden flex flex-col selection:bg-primary/30">
      {/* Top Navigation / Status Bar - Premium Dark/Gold Treatment */}
      <header className="h-16 bg-card border-b border-border shadow-sm flex items-center justify-between px-6 z-10">
        <div className="flex items-center gap-4">
          <div className="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center font-bold text-primary ring-1 ring-primary/30">
            هـ.ق
          </div>
          <div>
            <h1 className="font-bold text-lg leading-tight tracking-tight text-foreground">هيئة قضايا الدولة</h1>
            <p className="text-xs text-muted-foreground font-medium">لوحة التحكم القضائية المركزية</p>
          </div>
        </div>
        <div className="flex items-center gap-6">
          <div className="text-sm bg-muted px-3 py-1.5 rounded-md border border-border flex items-center gap-2">
            <span className="text-muted-foreground text-xs">المعرف:</span>
            <span className="font-mono font-bold text-primary">JUD-8842-A</span>
          </div>
          <Badge variant={isEmergencyLocked ? 'destructive' : 'default'} className={`px-3 py-1 ${isEmergencyLocked ? 'bg-destructive text-destructive-foreground animate-pulse' : 'bg-chart-2 text-primary-foreground'}`}>
            {isEmergencyLocked ? 'النظام متوقف (طوارئ)' : 'الانتخابات جارية'}
          </Badge>
        </div>
      </header>

      {/* Main Content Grid - High Density Architecture */}
      <main className="flex-1 p-6 grid grid-cols-1 xl:grid-cols-12 gap-6 h-[calc(100vh-4rem)] overflow-hidden bg-background/50">

        {/* Left Column: Metrics & Terminals (4 Cols) */}
        <div className="xl:col-span-4 flex flex-col gap-6 h-full">

          {/* Quorum Metric Card */}
          <Card className="bg-card border-border shadow-md overflow-hidden relative group">
            <div className="absolute top-0 left-0 w-1 h-full bg-chart-1"></div>
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-bold text-muted-foreground uppercase tracking-wider">النصاب القانوني للجمعية</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex justify-between items-end mb-3">
                <span className="text-5xl font-numerals font-black text-foreground tracking-tighter">{quorumPercentage.toFixed(1)}<span className="text-2xl text-muted-foreground">%</span></span>
                <span className="text-sm text-muted-foreground font-medium mb-1">المطلوب: 50% + 1</span>
              </div>
              <Progress
                value={quorumPercentage}
                className={`h-2.5 bg-muted ${quorumPercentage >= 50 ? '[&>div]:bg-chart-2' : '[&>div]:bg-primary'}`}
              />
              {quorumPercentage >= 50 && (
                <motion.p
                  initial={{ opacity: 0, y: 5 }} animate={{ opacity: 1, y: 0 }}
                  className="text-xs text-chart-2 mt-3 font-bold flex items-center gap-1.5"
                >
                  <div className="w-2 h-2 rounded-full bg-chart-2 animate-pulse"></div>
                  اكتمل النصاب القانوني - جاهز للفرز
                </motion.p>
              )}
            </CardContent>
          </Card>

          {/* Terminal Status Card */}
          <Card className="flex-1 flex flex-col bg-card border-border shadow-md overflow-hidden">
            <CardHeader className="bg-muted/30 pb-3 border-b border-border">
              <CardTitle className="text-sm font-bold text-foreground flex items-center justify-between">
                <span>حالة اللجان الفرعية</span>
                <Badge variant="outline" className="text-xs bg-background">٤ لجان نشطة</Badge>
              </CardTitle>
            </CardHeader>
            <CardContent className="p-0 flex-1 overflow-hidden">
              <ScrollArea className="h-full">
                <div className="flex flex-col">
                  {MOCK_TERMINALS.map(term => (
                    <div key={term.id} className="flex items-center justify-between p-4 border-b border-border/50 hover:bg-accent/50 transition-colors group">
                      <div className="flex items-center gap-3">
                        <div className={`w-2.5 h-2.5 rounded-full ring-2 ring-offset-2 ring-offset-background ${
                          term.status === 'ONLINE' ? 'bg-chart-2 ring-chart-2/30' :
                          term.status === 'WARNING' ? 'bg-chart-1 ring-chart-1/30 animate-pulse' : 'bg-destructive ring-destructive/30'
                        }`} />
                        <div>
                          <p className="text-sm font-bold text-foreground group-hover:text-primary transition-colors">{term.location}</p>
                          <p className="text-[11px] font-mono text-muted-foreground mt-0.5">{term.id}</p>
                        </div>
                      </div>
                      <div className="text-end flex flex-col items-end">
                        <Badge variant="outline" className={`text-[10px] px-2 py-0 border-transparent ${
                          term.status === 'ONLINE' ? 'text-chart-2 bg-chart-2/10' :
                          term.status === 'WARNING' ? 'text-chart-1 bg-chart-1/10' : 'text-destructive bg-destructive/10'
                        }`}>
                          {term.status === 'ONLINE' ? 'متصل' : term.status === 'WARNING' ? 'بطء استجابة' : 'غير متصل'}
                        </Badge>
                        <span className="text-[10px] text-muted-foreground mt-1 font-numerals bg-muted px-1.5 rounded">{term.latency}ms</span>
                      </div>
                    </div>
                  ))}
                </div>
              </ScrollArea>
            </CardContent>
          </Card>
        </div>

        {/* Right Column: Live Cryptographic Log (8 Cols) */}
        <div className="xl:col-span-8 h-full flex flex-col">
          <Card className="flex-1 flex flex-col bg-card border-border shadow-md overflow-hidden relative">
            <CardHeader className="bg-muted/30 pb-3 border-b border-border flex flex-row items-center justify-between">
              <div>
                <CardTitle className="text-sm font-bold text-foreground flex items-center gap-2">
                  <svg className="w-4 h-4 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"></path></svg>
                  سجل التدقيق المباشر (Audit Log)
                </CardTitle>
                <p className="text-[11px] text-muted-foreground mt-1">تشفير من طرف إلى طرف. المزامنة الحية مفعلة.</p>
              </div>
              <div className="flex items-center gap-2 bg-background px-3 py-1.5 rounded-full border border-border shadow-sm">
                <span className="relative flex h-2 w-2">
                  {!isEmergencyLocked && <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-chart-2 opacity-75"></span>}
                  <span className={`relative inline-flex rounded-full h-2 w-2 ${isEmergencyLocked ? 'bg-destructive' : 'bg-chart-2'}`}></span>
                </span>
                <span className="text-[10px] font-bold text-foreground tracking-wider">{isEmergencyLocked ? 'موقوف' : 'نشط'}</span>
              </div>
            </CardHeader>
            <CardContent className="p-0 flex-1 overflow-hidden relative bg-card">

              {/* Table Header */}
              <div className="grid grid-cols-6 text-[11px] font-bold text-muted-foreground bg-muted/50 border-b border-border px-4 py-2 sticky top-0 z-10 uppercase tracking-wider">
                <div className="col-span-1">الوقت</div>
                <div className="col-span-3">البصمة المشفرة (Hash Signature)</div>
                <div className="col-span-1 text-center">المحطة</div>
                <div className="col-span-1 text-end">الحالة</div>
              </div>

              {/* Virtualized Log List Simulation */}
              <ScrollArea className="h-full">
                <div className="flex flex-col">
                  <AnimatePresence initial={false}>
                    {logs.map((log) => (
                      <motion.div
                        key={log.id}
                        initial={{ opacity: 0, x: -10, backgroundColor: 'var(--muted)' }}
                        animate={{ opacity: 1, x: 0, backgroundColor: 'transparent' }}
                        transition={{ duration: 0.2 }}
                        className="grid grid-cols-6 items-center text-sm px-4 py-3 border-b border-border/40 hover:bg-accent/30 transition-colors"
                      >
                        <div className="col-span-1 font-numerals text-xs text-muted-foreground">{log.timestamp}</div>
                        <div className="col-span-3 font-mono text-[11px] text-foreground flex items-center">
                          <span className="bg-muted px-2 py-0.5 rounded border border-border/50 text-muted-foreground truncate max-w-[200px]">
                            {log.voterHash}
                          </span>
                        </div>
                        <div className="col-span-1 text-center font-mono text-xs text-foreground bg-background border border-border rounded px-1 w-fit mx-auto">{log.terminalId}</div>
                        <div className="col-span-1 flex justify-end">
                          <Badge variant="outline" className={`text-[10px] px-2 py-0.5 border-transparent ${
                            log.status === 'VOTED' ? 'bg-primary/10 text-primary' :
                            log.status === 'VERIFIED' ? 'bg-chart-2/10 text-chart-2' :
                            'bg-destructive/10 text-destructive ring-1 ring-destructive/50'
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
                <div className="absolute inset-0 bg-background/80 backdrop-blur-md flex items-center justify-center z-20">
                  <div className="bg-card p-8 rounded-xl shadow-2xl border border-destructive/50 text-center max-w-md ring-4 ring-destructive/10">
                    <div className="w-16 h-16 bg-destructive/10 rounded-full flex items-center justify-center mx-auto mb-4">
                      <svg className="w-8 h-8 text-destructive" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"></path></svg>
                    </div>
                    <h2 className="text-xl font-bold text-foreground mb-2">تم تفعيل القفل الآمن (طوارئ)</h2>
                    <p className="text-sm text-muted-foreground mb-6">بناءً على بروتوكول الأمن القضائي، تم تعليق كافة العمليات. يرجى مراجعة سجلات التدقيق فوراً.</p>
                    <Button onClick={() => setIsEmergencyLocked(false)} className="w-full bg-foreground text-background hover:bg-foreground/90 font-bold">
                      رفع الحظر (يتطلب توقيع رقمي)
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
            size="icon"
            className="w-14 h-14 rounded-full bg-destructive hover:bg-destructive/90 text-destructive-foreground shadow-[0_0_30px_rgba(220,38,38,0.3)] hover:shadow-[0_0_40px_rgba(220,38,38,0.5)] transition-all hover:scale-105 border-2 border-background"
            title="إيقاف طوارئ"
          >
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"></path></svg>
          </Button>
        </div>
      )}
    </div>
  );
}
