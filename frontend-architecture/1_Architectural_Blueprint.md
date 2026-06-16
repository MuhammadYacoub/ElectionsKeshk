# Electronic Elections Management System (EEMS)
## Frontend Architectural Blueprint

**Institution:** Egyptian State Lawsuits Authority & National Election Authority
**Stack:** React 18+, TypeScript, Tailwind CSS, Shadcn UI, Zustand, React Query, Framer Motion
**Direction:** RTL Native (Arabic First)

---

### 1. Optimal Folder Structure

To manage the high data density and complex workflows of judicial elections, we adopt a domain-driven, feature-based architecture (`Feature-Sliced Design` inspired).

```text
src/
├── app/                      # Application shell, global providers, and layouts
│   ├── layout/               # Main layout wrappers (DashboardLayout, KioskLayout)
│   ├── providers/            # React Query, Theme, Context providers
│   └── router/               # Route definitions
├── assets/                   # Static assets (fonts, SVGs, judicial crests)
├── components/               # Global shared UI components
│   ├── ui/                   # Shadcn UI primitives (Button, Table, Dialog, etc.)
│   ├── data-display/         # High-density tables, sparklines, progress bars
│   └── typography/           # Specialized judicial typographic components
├── config/                   # Global constants and configuration
│   └── site-config.ts        # Theming, API endpoints, feature flags
├── features/                 # Domain-driven feature modules
│   ├── elections/            # Election lifecycle management
│   ├── judicial-dashboard/   # The core Control Center
│   ├── live-tallying/        # Real-time results and algorithms
│   └── voting-kiosk/         # The voter-facing interface
├── hooks/                    # Global utility hooks (useKeyPress, useWindowSize)
├── lib/                      # Core libraries and utilities
│   ├── api-client.ts         # Axios/Fetch wrapper
│   ├── cn.ts                 # Tailwind class merger (clsx + tailwind-merge)
│   └── utils.ts              # General formatting utilities (dates, numbers)
├── store/                    # Global state management
│   └── useSystemState.ts     # Zustand global store
├── styles/                   # Global CSS and Tailwind directives
│   └── globals.css           # Contains global RTL overrides and CSS variables
└── types/                    # Global TypeScript interfaces
    └── domain.ts             # Core business models (Voter, Candidate, Seat)
```

### 2. State Management Strategy

We require absolute real-time precision and rock-solid reliability.

1.  **Server State (React Query / TanStack Query):**
    *   **Purpose:** Handling asynchronous data, caching, background polling, and real-time updates.
    *   **Usage:** Live turnout rates, cryptographic logs, candidate lists, and tallying results.
    *   **Configuration:** Aggressive polling (`refetchInterval: 5000`) for the Live-Tallying Grid, combined with WebSocket subscriptions for instant updates.
2.  **Client State (Zustand):**
    *   **Purpose:** Global UI state, user sessions, active election context, and temporary overrides.
    *   **Usage:** Storing the currently active "Constituency ID", global "Lockdown Mode" toggle, or judicial authorization tokens.
    *   **Configuration:** Immutable state updates and potential persistence to `sessionStorage` for crash recovery.

### 3. Theming & Tailwind Configuration

The aesthetic is grounded in a "Judicial Prestige" palette: Deep Navy, Gold Accents, Clean White, and Charcoal.

**`tailwind.config.ts` Extension:**
```typescript
import type { Config } from "tailwindcss"

const config = {
  darkMode: ["class"],
  content: [
    './pages/**/*.{ts,tsx}',
    './components/**/*.{ts,tsx}',
    './app/**/*.{ts,tsx}',
    './src/**/*.{ts,tsx}',
    './features/**/*.{ts,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        // Core Judicial Palette
        judicial: {
          navy: '#0B1C3C', // Primary Deep Blue
          gold: '#C5A059', // Official Seals, Accents
          charcoal: '#1E293B', // High contrast text
          slate: '#F8FAFC', // App Backgrounds
          danger: '#DC2626', // Overrides, Alerts
          success: '#16A34A', // Quorum Met, Validated
        },
        // Shadcn semantic mapping
        primary: {
          DEFAULT: '#0B1C3C',
          foreground: '#FFFFFF',
        },
        secondary: {
          DEFAULT: '#C5A059',
          foreground: '#000000',
        },
        background: '#F8FAFC',
        foreground: '#1E293B',
        // ... (standard shadcn borders, inputs, etc.)
      },
      fontFamily: {
        arabic: ['"Cairo"', 'sans-serif'], // Primary legible font
        numerals: ['"Inter"', 'sans-serif'], // Clear numerals for tallying
      },
      // ... (animations)
    },
  },
  plugins: [require("tailwindcss-animate")],
}
export default config
```

### 4. RTL First Approach

*   **Global Layout:** The HTML tag must include `dir="rtl"`.
*   **Tailwind Logical Properties:** We will use logical properties (e.g., `ms-4` instead of `ml-4`, `pe-2` instead of `pr-2`) universally. Tailwind v3/v4 supports these natively.
*   **Framer Motion:** Directional animations (slide-in) must be RTL-aware.
