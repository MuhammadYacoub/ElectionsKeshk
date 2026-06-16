import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import {
  AreaChart, Area, ResponsiveContainer,
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip as RechartsTooltip, Cell,
} from 'recharts';
import {
  ShieldCheck, Activity, MapPin,
  Settings, FileText, Lock, BarChart3, Fingerprint,
  Command, Server, ShieldAlert
} from 'lucide-react';

// Simulated Shadcn UI Imports
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent
} from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/components/ui/tabs';

// --- Mock Data ---
const TALLY_DATA = [
  { name: 'القاهرة', votes: 12450 },
  { name: 'الإسكندرية', votes: 8230 },
  { name: 'الجيزة', votes: 9100 },
  { name: 'الدقهلية', votes: 5400 },
  { name: 'الشرقية', votes: 6700 },
  { name: 'القليوبية', votes: 4200 },
];

const SPARK_DATA_1 = Array.from({ length: 20 }, (_, i) => ({ value: 100 + Math.random() * 50 + i * 5 }));
const SPARK_DATA_2 = Array.from({ length: 20 }, (_, i) => ({ value: 40 + Math.random() * 10 }));
const SPARK_DATA_3 = Array.from({ length: 20 }, (_, i) => ({ value: 120 + Math.random() * 5 }));
const SPARK_DATA_4 = Array.from({ length: 20 }, (_, i) => ({ value: 800 + Math.random() * 100 }));

type LogEvent = {
  id: string;
  timestamp: string;
  voterHash: string;
  terminalId: string;
  status: 'VOTED' | 'VERIFIED' | 'ANOMALY';
};

const INITIAL_LOGS: LogEvent[] = [
  { id: '1', timestamp: '10:42:01', voterHash: '0x3F...9A2B', terminalId: 'T-04', status: 'VOTED' },
  { id: '2', timestamp: '10:41:55', voterHash: '0x8C...11EF', terminalId: 'T-12', status: 'VERIFIED' },
  { id: '3', timestamp: '10:41:50', voterHash: '0x1A...B8C3', terminalId: 'T-04', status: 'VOTED' },
  { id: '4', timestamp: '10:40:12', voterHash: 'INVALID_SIG', terminalId: 'T-09', status: 'ANOMALY' },
];

export default function JudicialDashboard() {
  const [logs, setLogs] = useState<LogEvent[]>(INITIAL_LOGS);
  const [isEmergencyLocked, setIsEmergencyLocked] = useState(false);

  // Simulate real-time logs
  useEffect(() => {
    if (isEmergencyLocked) return;
    const interval = setInterval(() => {
      const newLog: LogEvent = {
        id: Math.random().toString(36).substr(2, 9),
        timestamp: new Date().toLocaleTimeString('ar-EG', { hour12: false }),
        voterHash: `0x${Math.floor(Math.random() * 16777215).toString(16).toUpperCase()}...`,
        terminalId: `T-0${Math.floor(Math.random() * 5) + 1}`,
        status: Math.random() > 0.95 ? 'ANOMALY' : 'VOTED'
      };
      setLogs(prev => [newLog, ...prev].slice(0, 50));
    }, 2500);
    return () => clearInterval(interval);
  }, [isEmergencyLocked]);

  return (
    <div dir="rtl" className="h-screen w-full bg-background text-foreground font-arabic flex overflow-hidden selection:bg-primary/30">

      {/* 1. The Sidebar (Professional & Dark) */}
      <aside className="w-64 bg-sidebar border-l border-sidebar-border hidden lg:flex flex-col z-20 flex-shrink-0 shadow-xl">
        <div className="h-16 flex items-center px-6 border-b border-sidebar-border bg-sidebar/50 backdrop-blur-sm">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-lg bg-sidebar-primary flex items-center justify-center text-sidebar-primary-foreground shadow-inner">
              <ShieldCheck size={18} className="drop-shadow-md" />
            </div>
            <div className="font-black text-sidebar-foreground tracking-tight">المنصة القضائية</div>
          </div>
        </div>
        <div className="p-4 flex-1 space-y-1.5 overflow-y-auto">
          <SidebarItem icon={<Activity size={18} />} label="المراقبة الحية" active />
          <SidebarItem icon={<BarChart3 size={18} />} label="الفرز والإحصاء" />
          <SidebarItem icon={<MapPin size={18} />} label="اللجان الفرعية" />
          <SidebarItem icon={<FileText size={18} />} label="سجلات التدقيق" />
          <SidebarItem icon={<Settings size={18} />} label="إعدادات النظام" />
        </div>
        <div className="p-4 border-t border-sidebar-border bg-sidebar/30">
          <div className="bg-sidebar-accent/50 rounded-xl p-3.5 flex items-center gap-3 border border-sidebar-border/50 shadow-sm backdrop-blur-md">
            <div className="relative flex h-2.5 w-2.5">
              <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-chart-2 opacity-75"></span>
              <span className="relative inline-flex rounded-full h-2.5 w-2.5 bg-chart-2"></span>
            </div>
            <div>
              <p className="text-xs font-bold text-sidebar-foreground">اتصال آمن نشط</p>
              <p className="text-[10px] text-muted-foreground mt-0.5">E2E مفعل - خادم #01</p>
            </div>
          </div>
        </div>
      </aside>

      {/* Main Content Area */}
      <div className="flex-1 flex flex-col h-full overflow-hidden relative">
        {/* Top Header */}
        <header className="h-16 bg-background/80 backdrop-blur-md border-b border-border flex items-center justify-between px-4 md:px-8 z-10 flex-shrink-0">
          <div>
            <h1 className="font-black text-lg leading-tight tracking-tight text-foreground">هيئة قضايا الدولة</h1>
            <p className="text-[11px] text-muted-foreground font-bold uppercase tracking-wider">لوحة التحكم القضائية المركزية</p>
          </div>
          <div className="flex items-center gap-4">
            <div className="hidden md:flex items-center gap-2 bg-muted px-3 py-1.5 rounded-lg border border-border shadow-sm">
              <Command size={14} className="text-muted-foreground" />
              <span className="font-mono text-xs font-bold text-primary">JUD-8842-A</span>
            </div>
            <Badge variant={isEmergencyLocked ? 'destructive' : 'default'} className={`px-3 py-1.5 text-xs font-bold shadow-sm ${isEmergencyLocked ? 'bg-destructive text-destructive-foreground animate-pulse' : 'bg-chart-2/10 text-chart-2 border border-chart-2/20'}`}>
              {isEmergencyLocked ? 'النظام متوقف (طوارئ)' : 'الانتخابات جارية'}
            </Badge>
          </div>
        </header>

        {/* Scrollable Dashboard Grid */}
        <main className="flex-1 overflow-auto p-4 md:p-8 bg-muted/20">

          {/* Advanced Metrics & Analytics Cards */}
          <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-4 md:gap-6 mb-6">
            <MetricCard title="إجمالي الناخبين" value="142,593" trend="+12%" data={SPARK_DATA_1} color="var(--chart-1)" />
            <MetricCard title="نسبة المشاركة" value="45.2%" trend="+2.4%" data={SPARK_DATA_2} color="var(--chart-2)" isPercent />
            <MetricCard title="اللجان النشطة" value="124" trend="128 إجمالي" data={SPARK_DATA_3} color="var(--chart-3)" />
            <MetricCard title="معدل الاقتراع / دقيقة" value="843" trend="+54" data={SPARK_DATA_4} color="var(--chart-4)" />
          </div>

          <div className="grid grid-cols-1 xl:grid-cols-4 gap-4 md:gap-6 h-[480px]">

            {/* Live Vote Tallying Chart (Col Span 3) */}
            <Card className="xl:col-span-3 bg-card border-border shadow-md flex flex-col overflow-hidden">
              <CardHeader className="pb-2 border-b border-border/40 bg-muted/10">
                <div className="flex justify-between items-center">
                  <CardTitle className="text-sm font-bold flex items-center gap-2">
                    <BarChart3 className="w-4 h-4 text-primary" />
                    الفرز المباشر للأصوات
                  </CardTitle>
                  <Badge variant="outline" className="text-[10px] bg-background">تحديث لحظي</Badge>
                </div>
              </CardHeader>
              <CardContent className="flex-1 p-4 pt-6">
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart data={TALLY_DATA} margin={{ top: 10, right: 10, left: -20, bottom: 0 }}>
                    <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" vertical={false} opacity={0.5} />
                    <XAxis dataKey="name" stroke="var(--muted-foreground)" fontSize={12} tickLine={false} axisLine={false} dy={10} />
                    <YAxis stroke="var(--muted-foreground)" fontSize={12} tickLine={false} axisLine={false} tickFormatter={(val) => `${val/1000}k`} dx={-10} />
                    <RechartsTooltip
                      cursor={{ fill: 'var(--muted)', opacity: 0.4 }}
                      contentStyle={{ backgroundColor: 'var(--popover)', border: '1px solid var(--border)', borderRadius: '12px', boxShadow: '0 10px 25px -5px rgba(0,0,0,0.1)' }}
                      itemStyle={{ color: 'var(--foreground)', fontWeight: 'bold' }}
                      labelStyle={{ color: 'var(--muted-foreground)', marginBottom: '4px' }}
                    />
                    <Bar dataKey="votes" radius={[6, 6, 0, 0]} maxBarSize={60}>
                      {TALLY_DATA.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={`var(--chart-${(index % 5) + 1})`} />
                      ))}
                    </Bar>
                  </BarChart>
                </ResponsiveContainer>
              </CardContent>
            </Card>

            {/* Cryptographic Quick Action Panel (Col Span 1) */}
            <Card className="xl:col-span-1 bg-card border-border shadow-md flex flex-col overflow-hidden">
              <CardHeader className="pb-2 border-b border-border/40 bg-muted/10">
                <CardTitle className="text-sm font-bold flex items-center gap-2 text-primary">
                  <Lock className="w-4 h-4" />
                  التحكم المشفّر (Crypto Control)
                </CardTitle>
              </CardHeader>
              <CardContent className="flex-1 p-4 flex flex-col">
                <Tabs defaultValue="verify" className="w-full flex-1 flex flex-col">
                  <TabsList className="w-full grid grid-cols-2 mb-4 bg-muted/50 p-1 rounded-xl">
                    <TabsTrigger value="verify" className="text-xs font-bold rounded-lg data-[state=active]:bg-card data-[state=active]:shadow-sm">مصادقة QR</TabsTrigger>
                    <TabsTrigger value="commands" className="text-xs font-bold rounded-lg data-[state=active]:bg-card data-[state=active]:shadow-sm">الأوامر</TabsTrigger>
                  </TabsList>

                  <TabsContent value="verify" className="flex-1 flex flex-col items-center justify-center mt-0 data-[state=active]:flex">
                    <div className="bg-white p-4 rounded-2xl border border-border shadow-sm mb-5 relative group ring-1 ring-primary/10">
                      <div className="absolute inset-0 bg-primary/5 rounded-2xl opacity-0 group-hover:opacity-100 transition-opacity pointer-events-none" />
                      {/* Premium Inline SVG QR Mock */}
                      <svg width="140" height="140" viewBox="0 0 100 100" xmlns="http://www.w3.org/2000/svg">
                        <rect width="100" height="100" fill="#fff" rx="8"/>
                        {/* QR Corners */}
                        <path d="M10,10 h22 v22 h-22 z M14,14 h14 v14 h-14 z M18,18 h6 v6 h-6 z" fill="#0f172a"/>
                        <path d="M68,10 h22 v22 h-22 z M72,14 h14 v14 h-14 z M76,18 h6 v6 h-6 z" fill="#0f172a"/>
                        <path d="M10,68 h22 v22 h-22 z M14,72 h14 v14 h-14 z M18,76 h6 v6 h-6 z" fill="#0f172a"/>
                        {/* Internal Pattern */}
                        <path d="M40,10 h20 v10 h-20 z" fill="#0f172a"/>
                        <path d="M45,25 h10 v20 h-10 z" fill="#0f172a"/>
                        <path d="M68,40 h22 v10 h-22 z" fill="#0f172a"/>
                        <path d="M10,40 h22 v10 h-22 z" fill="#0f172a"/>
                        <path d="M40,55 h30 v10 h-30 z" fill="#0f172a"/>
                        <path d="M76,60 h14 v30 h-14 z" fill="#0f172a"/>
                        <path d="M40,76 h20 v14 h-20 z" fill="#0f172a"/>
                        <path d="M20,55 h10 v8 h-10 z" fill="#0f172a"/>
                      </svg>
                    </div>
                    <p className="text-[11px] text-center text-muted-foreground mb-6 leading-relaxed px-4 font-medium">
                      امسح الرمز ضوئياً للتحقق من الهوية القضائية بصلاحيات كاملة
                    </p>
                    <Button className="w-full gap-2 font-bold bg-primary text-primary-foreground hover:bg-primary/90 shadow-md h-11 rounded-xl">
                      <Fingerprint size={16} /> توقيع رقمي (Sign)
                    </Button>
                  </TabsContent>

                  <TabsContent value="commands" className="flex-1 mt-0">
                    <div className="space-y-3 pt-2">
                      <Button variant="outline" className="w-full justify-start text-xs font-bold border-border/60 bg-card hover:bg-accent hover:text-accent-foreground text-foreground h-11 rounded-xl">
                        <Server size={14} className="ms-0 me-3 text-chart-2" /> مزامنة العقدة المركزية
                      </Button>
                      <Button variant="outline" className="w-full justify-start text-xs font-bold border-border/60 bg-card hover:bg-accent hover:text-accent-foreground text-foreground h-11 rounded-xl">
                        <FileText size={14} className="ms-0 me-3 text-primary" /> تصدير تقرير التشفير
                      </Button>
                      <div className="pt-6 mt-4 border-t border-border/40">
                        <Button
                          onClick={() => setIsEmergencyLocked(true)}
                          variant="destructive"
                          className="w-full justify-start text-xs font-bold bg-destructive text-destructive-foreground hover:bg-destructive/90 h-11 rounded-xl shadow-sm"
                        >
                          <ShieldAlert size={14} className="ms-0 me-3" /> قفل طوارئ (Lockdown)
                        </Button>
                      </div>
                    </div>
                  </TabsContent>
                </Tabs>
              </CardContent>
            </Card>
          </div>

          {/* Minimal Live Audit Log - Just to give density */}
          <div className="mt-4 md:mt-6">
            <Card className="bg-card border-border shadow-sm overflow-hidden">
              <CardHeader className="py-3 px-4 border-b border-border/40 bg-muted/10">
                <CardTitle className="text-xs font-bold text-foreground flex items-center justify-between">
                  <span className="flex items-center gap-2"><Lock className="w-3.5 h-3.5 text-muted-foreground" /> سجل التدقيق المصغر</span>
                  <span className="text-[10px] text-muted-foreground font-mono">E2E ENCRYPTED STREAM</span>
                </CardTitle>
              </CardHeader>
              <CardContent className="p-0">
                <div className="grid grid-cols-4 text-[10px] font-bold text-muted-foreground bg-muted/30 px-4 py-2 border-b border-border/30 uppercase">
                  <div>الوقت</div>
                  <div className="col-span-2">تجزئة الكتلة (Hash)</div>
                  <div className="text-end">الحالة</div>
                </div>
                <div className="h-[120px] overflow-hidden relative">
                  <AnimatePresence initial={false}>
                    {logs.slice(0, 4).map((log) => (
                      <motion.div
                        key={log.id}
                        initial={{ opacity: 0, y: -10 }}
                        animate={{ opacity: 1, y: 0 }}
                        className="grid grid-cols-4 items-center text-[11px] px-4 py-2.5 border-b border-border/20"
                      >
                        <div className="font-mono text-muted-foreground">{log.timestamp}</div>
                        <div className="col-span-2 font-mono text-foreground">{log.voterHash}</div>
                        <div className="flex justify-end">
                          <Badge variant="outline" className={`text-[9px] px-1.5 py-0 border-transparent ${
                            log.status === 'VOTED' ? 'bg-primary/10 text-primary' :
                            log.status === 'VERIFIED' ? 'bg-chart-2/10 text-chart-2' :
                            'bg-destructive/10 text-destructive'
                          }`}>
                            {log.status}
                          </Badge>
                        </div>
                      </motion.div>
                    ))}
                  </AnimatePresence>
                </div>
              </CardContent>
            </Card>
          </div>

        </main>

        {/* Emergency Lock Overlay */}
        {isEmergencyLocked && (
          <div className="absolute inset-0 bg-background/90 backdrop-blur-sm flex items-center justify-center z-50 p-4">
            <motion.div
              initial={{ scale: 0.9, opacity: 0 }} animate={{ scale: 1, opacity: 1 }}
              className="bg-card p-8 rounded-3xl shadow-2xl border border-destructive/50 text-center max-w-md w-full ring-4 ring-destructive/10"
            >
              <div className="w-20 h-20 bg-destructive/10 rounded-full flex items-center justify-center mx-auto mb-6">
                <ShieldAlert className="w-10 h-10 text-destructive" />
              </div>
              <h2 className="text-2xl font-black text-foreground mb-3">النظام في وضع الإغلاق</h2>
              <p className="text-sm text-muted-foreground mb-8 leading-relaxed font-medium">
                بناءً على بروتوكول الأمن القضائي، تم تعليق كافة العمليات. يرجى مراجعة سجلات التدقيق فوراً لتأكيد السلامة.
              </p>
              <Button onClick={() => setIsEmergencyLocked(false)} className="w-full h-12 text-sm bg-foreground text-background hover:bg-foreground/90 font-bold rounded-xl shadow-lg">
                رفع الحظر (مفتاح الإدارة)
              </Button>
            </motion.div>
          </div>
        )}
      </div>
    </div>
  );
}

// --- Subcomponents ---

function SidebarItem({ icon, label, active = false }: { icon: React.ReactNode, label: string, active?: boolean }) {
  return (
    <div className={`flex items-center gap-3 px-3 py-2.5 rounded-xl cursor-pointer transition-all duration-200 group ${
      active ? 'bg-sidebar-accent text-sidebar-accent-foreground shadow-sm' : 'text-sidebar-foreground/70 hover:bg-sidebar-accent/50 hover:text-sidebar-foreground'
    }`}>
      <div className={`${active ? 'text-sidebar-primary' : 'text-sidebar-foreground/50 group-hover:text-sidebar-foreground/80'}`}>
        {icon}
      </div>
      <span className="text-sm font-bold">{label}</span>
      {active && (
        <div className="me-auto w-1.5 h-1.5 rounded-full bg-sidebar-primary shadow-[0_0_8px_var(--sidebar-primary)]" />
      )}
    </div>
  );
}

function MetricCard({ title, value, trend, data, color, isPercent = false }: any) {
  return (
    <Card className="bg-card border-border shadow-sm relative overflow-hidden group">
      <div className="absolute top-0 right-0 w-1 h-full" style={{ backgroundColor: color }}></div>
      <CardHeader className="pb-1 pt-5 px-5">
        <CardTitle className="text-xs font-bold text-muted-foreground uppercase tracking-wider">{title}</CardTitle>
      </CardHeader>
      <CardContent className="px-5 pb-5">
        <div className="flex justify-between items-end mb-2">
          <span className="text-3xl font-black text-foreground tracking-tighter font-mono">
            {value}
          </span>
          <span className="text-xs font-bold px-2 py-1 rounded bg-muted text-muted-foreground mb-1">
            {trend}
          </span>
        </div>
        <div className="h-[40px] mt-4 w-full">
          <ResponsiveContainer width="100%" height="100%">
            <AreaChart data={data}>
              <defs>
                <linearGradient id={`grad-${title.replace(/\s+/g, "-")}`} x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor={color} stopOpacity={0.3}/>
                  <stop offset="95%" stopColor={color} stopOpacity={0}/>
                </linearGradient>
              </defs>
              <Area
                type="monotone"
                dataKey="value"
                stroke={color}
                strokeWidth={2}
                fillOpacity={1}
                fill={`url(#grad-${title.replace(/\s+/g, "-")})`}
                isAnimationActive={false}
              />
            </AreaChart>
          </ResponsiveContainer>
        </div>
      </CardContent>
    </Card>
  );
}
