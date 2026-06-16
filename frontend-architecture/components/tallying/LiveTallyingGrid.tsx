import React, { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
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
    title: 'أعضاء مجلس الإدارة (محكمة استئناف القاهرة)',
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
    // Simulate generation time
    setTimeout(() => setIsGeneratingReport(false), 2000);
  };

  return (
    <div dir="rtl" className="min-h-screen bg-slate-50 font-arabic flex flex-col">
      {/* Header Controls */}
      <div className="bg-white p-4 border-b border-slate-200 shadow-sm flex items-center justify-between sticky top-0 z-20">
        <div>
          <h2 className="text-xl font-bold text-[#0B1C3C]">شاشة الفرز المباشر (Live Tallying)</h2>
          <p className="text-sm text-slate-500">تحديث فوري للنتائج بناءً على الخوارزميات المعتمدة.</p>
        </div>

        <div className="flex items-center gap-4">
          <Tabs value={activeTab} onValueChange={setActiveTab} className="w-[400px]">
            <TabsList className="grid w-full grid-cols-2 bg-slate-100 p-1 rounded-md">
              <TabsTrigger value="general" className="data-[state=active]:bg-white data-[state=active]:text-[#0B1C3C] data-[state=active]:shadow-sm text-slate-600 font-bold">
                النقابة العامة
              </TabsTrigger>
              <TabsTrigger value="branches" className="data-[state=active]:bg-white data-[state=active]:text-[#0B1C3C] data-[state=active]:shadow-sm text-slate-600 font-bold">
                النقابات الفرعية
              </TabsTrigger>
            </TabsList>
          </Tabs>

          <Button
            onClick={handleGenerateReport}
            disabled={isGeneratingReport}
            className="bg-[#C5A059] hover:bg-[#b08d4b] text-[#0B1C3C] font-bold"
          >
            {isGeneratingReport ? 'جاري إنشاء المحضر...' : 'إصدار محضر الفرز (PDF)'}
            {!isGeneratingReport && (
              <svg className="w-4 h-4 ms-2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path></svg>
            )}
          </Button>
        </div>
      </div>

      {/* Grid Content */}
      <main className="flex-1 p-6">
        <TabsContent value="general" className="mt-0 outline-none">
          <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">

            {MOCK_CATEGORIES.map((category) => (
              <Card key={category.id} className="shadow-md border-slate-200 overflow-hidden flex flex-col">
                {/* Category Header */}
                <div className="bg-[#0B1C3C] p-4 text-white flex justify-between items-center border-b-4 border-[#C5A059]">
                  <div>
                    <h3 className="text-lg font-bold">{category.title}</h3>
                    <p className="text-xs text-slate-300">عدد المقاعد المتاحة: {category.availableSeats}</p>
                  </div>
                  <div className="text-end">
                    <p className="text-xs text-slate-300">إجمالي الأصوات الصحيحة</p>
                    <p className="text-xl font-numerals font-bold">{category.totalVotesCast.toLocaleString('ar-EG')}</p>
                  </div>
                </div>

                {/* Candidates List */}
                <CardContent className="p-0 flex-1 bg-white">
                  <div className="flex flex-col">
                    {category.candidates.map((candidate, index) => (
                      <motion.div
                        key={candidate.id}
                        initial={{ opacity: 0, x: 20 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ delay: index * 0.1 }}
                        className={`flex items-center gap-4 p-4 border-b border-slate-100 last:border-0 relative ${
                          candidate.isWinner ? 'bg-green-50/50' : 'hover:bg-slate-50'
                        }`}
                      >
                        {/* Winner Indicator / Rank */}
                        <div className="w-8 flex justify-center items-center font-numerals font-bold text-slate-400">
                          {index + 1}
                        </div>

                        {/* Avatar */}
                        <div className={`w-12 h-12 rounded-full bg-slate-200 flex-shrink-0 border-2 overflow-hidden flex items-center justify-center ${
                          candidate.isWinner ? 'border-[#C5A059]' : 'border-transparent'
                        }`}>
                          <span className="text-slate-400 text-xs">صورة</span>
                        </div>

                        {/* Details & Progress */}
                        <div className="flex-1 min-w-0">
                          <div className="flex justify-between items-end mb-1">
                            <h4 className="font-bold text-[#0B1C3C] truncate pr-1">
                              {candidate.name}
                              {candidate.isWinner && (
                                <Badge className="ms-2 bg-[#C5A059] text-[#0B1C3C] hover:bg-[#C5A059] border-none text-[10px]">
                                  حسم المقعد
                                </Badge>
                              )}
                            </h4>
                            <div className="text-end">
                              <span className="font-numerals font-bold text-lg text-[#0B1C3C]">{candidate.percentage}%</span>
                            </div>
                          </div>

                          <div className="flex items-center gap-3">
                            <Progress
                              value={candidate.percentage}
                              className={`h-2 flex-1 ${candidate.isWinner ? '[&>div]:bg-[#C5A059]' : '[&>div]:bg-[#0B1C3C]'}`}
                            />
                            <span className="text-xs font-numerals text-slate-500 min-w-[60px] text-end">
                              {candidate.votes.toLocaleString('ar-EG')} صوت
                            </span>
                          </div>
                        </div>

                        {/* Trend Indicator */}
                        <div className="w-8 flex justify-center">
                           {candidate.trend === 'up' && <svg className="w-5 h-5 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"></path></svg>}
                           {candidate.trend === 'down' && <svg className="w-5 h-5 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 17h8m0 0v-8m0 8l-8-8-4 4-6-6"></path></svg>}
                           {candidate.trend === 'stable' && <svg className="w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 12h14"></path></svg>}
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
          <div className="flex items-center justify-center h-64 text-slate-500 border-2 border-dashed border-slate-200 rounded-lg">
            يرجى تحديد النقابة الفرعية من القائمة الجانبية لعرض النتائج (قيد التطوير)
          </div>
        </TabsContent>
      </main>
    </div>
  );
}
