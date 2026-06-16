import React, { useState } from 'react';
import { motion } from 'framer-motion';
// Simulated Shadcn UI Imports
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/components/ui/tabs';

// --- Types ---
type Candidate = {
  id: string;
  name: string;
  photoUrl: string;
  votes: number;
  percentage: number;
  trend: 'up' | 'down' | 'stable';
  isWinner?: boolean;
};

type SeatCategory = {
  id: string;
  title: string;
  availableSeats: number;
  totalVotesCast: number;
  candidates: Candidate[];
};

// --- Mock Data ---
const MOCK_CATEGORIES: SeatCategory[] = [
  {
    id: 'general-chair',
    title: 'مقعد النقيب العام',
    availableSeats: 1,
    totalVotesCast: 14500,
    candidates: [
      { id: 'c1', name: 'المستشار / أحمد محمود', photoUrl: '/avatars/1.jpg', votes: 8500, percentage: 58.6, trend: 'up', isWinner: true },
      { id: 'c2', name: 'المستشار / طارق السيد', photoUrl: '/avatars/2.jpg', votes: 4200, percentage: 28.9, trend: 'down' },
      { id: 'c3', name: 'المستشارة / فاطمة الزهراء', photoUrl: '/avatars/3.jpg', votes: 1800, percentage: 12.5, trend: 'stable' },
    ]
  },
  {
    id: 'board-members-cairo',
    title: 'أعضاء مجلس الإدارة (استئناف القاهرة)',
    availableSeats: 3,
    totalVotesCast: 12000,
    candidates: [
      { id: 'c4', name: 'المستشار / خالد مصطفى', photoUrl: '/avatars/4.jpg', votes: 9000, percentage: 75.0, trend: 'up' },
      { id: 'c5', name: 'المستشار / عمرو عبد الله', photoUrl: '/avatars/5.jpg', votes: 8500, percentage: 70.8, trend: 'up' },
      { id: 'c6', name: 'المستشار / سامي إبراهيم', photoUrl: '/avatars/6.jpg', votes: 7200, percentage: 60.0, trend: 'stable' },
      { id: 'c7', name: 'المستشارة / نهى سعيد', photoUrl: '/avatars/7.jpg', votes: 4000, percentage: 33.3, trend: 'down' },
      { id: 'c8', name: 'المستشار / وليد حامد', photoUrl: '/avatars/8.jpg', votes: 3800, percentage: 31.6, trend: 'down' },
    ]
  }
];

export default function LiveTallyingGrid() {
  const [activeTab, setActiveTab] = useState('general');
  const [isGeneratingReport, setIsGeneratingReport] = useState(false);

  const handleGenerateReport = () => {
    setIsGeneratingReport(true);
    setTimeout(() => setIsGeneratingReport(false), 2000);
  };

  return (
    <div dir="rtl" className="min-h-screen bg-background font-arabic flex flex-col selection:bg-primary/30">
      {/* Header Controls - Premium Glass/Solid Treatment */}
      <div className="bg-card/95 backdrop-blur supports-[backdrop-filter]:bg-card/60 p-4 border-b border-border shadow-sm flex items-center justify-between sticky top-0 z-20">
        <div>
          <h2 className="text-xl font-bold text-foreground tracking-tight">شاشة الفرز المباشر (Live Tallying)</h2>
          <p className="text-xs text-muted-foreground font-medium mt-0.5">تحديث فوري للنتائج بناءً على الخوارزميات المعتمدة.</p>
        </div>

        <div className="flex items-center gap-6">
          <Tabs value={activeTab} onValueChange={setActiveTab} className="w-[380px]">
            <TabsList className="grid w-full grid-cols-2 bg-muted p-1 rounded-lg border border-border">
              <TabsTrigger value="general" className="data-[state=active]:bg-background data-[state=active]:text-foreground data-[state=active]:shadow-sm text-muted-foreground font-bold text-xs">
                النقابة العامة
              </TabsTrigger>
              <TabsTrigger value="branches" className="data-[state=active]:bg-background data-[state=active]:text-foreground data-[state=active]:shadow-sm text-muted-foreground font-bold text-xs">
                النقابات الفرعية
              </TabsTrigger>
            </TabsList>
          </Tabs>

          <Button
            onClick={handleGenerateReport}
            disabled={isGeneratingReport}
            className="bg-primary hover:bg-primary/90 text-primary-foreground font-bold shadow-md shadow-primary/20 transition-all active:scale-95"
          >
            {isGeneratingReport ? 'جاري الإنشاء...' : 'إصدار المحضر (PDF)'}
            {!isGeneratingReport && (
              <svg className="w-4 h-4 ms-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path></svg>
            )}
          </Button>
        </div>
      </div>

      {/* Grid Content - High Density Cards */}
      <main className="flex-1 p-6 bg-background/50">
        <TabsContent value="general" className="mt-0 outline-none focus-visible:ring-0">
          <div className="grid grid-cols-1 xl:grid-cols-2 gap-8">

            {MOCK_CATEGORIES.map((category) => (
              <Card key={category.id} className="bg-card border-border shadow-lg overflow-hidden flex flex-col hover:shadow-xl transition-shadow duration-300">
                {/* Category Header */}
                <div className="bg-muted/40 p-5 border-b border-border relative overflow-hidden">
                  <div className="absolute top-0 right-0 w-full h-1 bg-gradient-to-l from-primary to-transparent"></div>
                  <div className="flex justify-between items-start">
                    <div>
                      <h3 className="text-lg font-black text-foreground tracking-tight">{category.title}</h3>
                      <div className="flex items-center gap-2 mt-2">
                        <Badge variant="outline" className="bg-background text-xs font-medium text-muted-foreground">
                          المقاعد: {category.availableSeats}
                        </Badge>
                      </div>
                    </div>
                    <div className="text-end bg-background px-4 py-2 rounded-lg border border-border shadow-sm">
                      <p className="text-[10px] text-muted-foreground uppercase font-bold tracking-wider mb-0.5">إجمالي الأصوات</p>
                      <p className="text-2xl font-numerals font-black text-primary leading-none">{category.totalVotesCast.toLocaleString('ar-EG')}</p>
                    </div>
                  </div>
                </div>

                {/* Candidates List */}
                <CardContent className="p-0 flex-1 bg-card">
                  <div className="flex flex-col">
                    {category.candidates.map((candidate, index) => (
                      <motion.div
                        key={candidate.id}
                        initial={{ opacity: 0, x: 10 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ delay: index * 0.05 }}
                        className={`flex items-center gap-4 p-4 border-b border-border/50 last:border-0 relative group transition-colors ${
                          candidate.isWinner ? 'bg-primary/5 hover:bg-primary/10' : 'hover:bg-muted/50'
                        }`}
                      >
                        {candidate.isWinner && (
                          <div className="absolute top-0 bottom-0 right-0 w-1 bg-primary"></div>
                        )}

                        {/* Rank */}
                        <div className="w-6 flex justify-center items-center font-numerals font-bold text-muted-foreground text-sm">
                          {index + 1}
                        </div>

                        {/* Avatar / Placeholder */}
                        <div className={`w-12 h-12 rounded-full bg-muted flex-shrink-0 flex items-center justify-center ring-2 ring-offset-2 ring-offset-card transition-all ${
                          candidate.isWinner ? 'ring-primary' : 'ring-transparent group-hover:ring-border'
                        }`}>
                          <svg className="w-6 h-6 text-muted-foreground/50" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path></svg>
                        </div>

                        {/* Details & Progress */}
                        <div className="flex-1 min-w-0 pr-2">
                          <div className="flex justify-between items-center mb-2">
                            <h4 className="font-bold text-foreground truncate text-sm flex items-center gap-2">
                              {candidate.name}
                              {candidate.isWinner && (
                                <Badge className="bg-primary text-primary-foreground hover:bg-primary/90 border-transparent text-[9px] px-1.5 py-0 uppercase tracking-widest">
                                  حسم
                                </Badge>
                              )}
                            </h4>
                            <div className="text-end pl-2">
                              <span className={`font-numerals font-black text-lg ${candidate.isWinner ? 'text-primary' : 'text-foreground'}`}>
                                {candidate.percentage}%
                              </span>
                            </div>
                          </div>

                          <div className="flex items-center gap-4">
                            <Progress
                              value={candidate.percentage}
                              className={`h-2 flex-1 bg-muted/80 overflow-hidden ${candidate.isWinner ? '[&>div]:bg-primary' : '[&>div]:bg-foreground/40 group-hover:[&>div]:bg-foreground/60 transition-colors'}`}
                            />
                            <span className="text-[11px] font-numerals font-medium text-muted-foreground min-w-[70px] text-end bg-background px-2 py-0.5 rounded border border-border/50">
                              {candidate.votes.toLocaleString('ar-EG')}
                            </span>
                          </div>
                        </div>

                        {/* Trend Indicator */}
                        <div className="w-8 flex justify-center items-center pl-2">
                           {candidate.trend === 'up' && <div className="w-6 h-6 rounded-full bg-chart-2/10 flex items-center justify-center"><svg className="w-3.5 h-3.5 text-chart-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="3" d="M5 10l7-7m0 0l7 7m-7-7v18"></path></svg></div>}
                           {candidate.trend === 'down' && <div className="w-6 h-6 rounded-full bg-destructive/10 flex items-center justify-center"><svg className="w-3.5 h-3.5 text-destructive" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="3" d="M19 14l-7 7m0 0l-7-7m7 7V3"></path></svg></div>}
                           {candidate.trend === 'stable' && <div className="w-6 h-6 rounded-full bg-muted flex items-center justify-center"><svg className="w-3.5 h-3.5 text-muted-foreground" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="3" d="M5 12h14"></path></svg></div>}
                        </div>
                      </motion.div>
                    ))}
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </TabsContent>

        <TabsContent value="branches">
          <div className="flex items-center justify-center h-[60vh] bg-card rounded-xl border border-dashed border-border/60 shadow-sm">
            <div className="text-center">
              <div className="w-16 h-16 bg-muted rounded-full flex items-center justify-center mx-auto mb-4 text-muted-foreground">
                <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="1.5" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path></svg>
              </div>
              <h3 className="text-lg font-bold text-foreground mb-1">حدد النقابة الفرعية</h3>
              <p className="text-sm text-muted-foreground">يرجى اختيار النقابة من القائمة لعرض نتائج الفرز.</p>
            </div>
          </div>
        </TabsContent>
      </main>
    </div>
  );
}
