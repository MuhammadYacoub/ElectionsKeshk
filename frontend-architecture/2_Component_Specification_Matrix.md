# Component Specification Matrix

This matrix defines the high-level layout, components, and interactions for the two most critical views of the Electronic Elections Management System (EEMS).

---

## 1. Judicial Control Center Dashboard

**Purpose:** Real-time operational overview, anomaly detection, and emergency overrides.
**Layout Type:** High-Density Analytical Dashboard (`100vh` locked, no body scrolling).

| Region / Component | Shadcn UI Elements | Tailwind Classes & Theming | Interaction & State |
| :--- | :--- | :--- | :--- |
| **Top Navigation Bar** | `NavigationMenu`, `Avatar`, `DropdownMenu` | `bg-judicial-navy text-white h-16 shadow-md border-b-judicial-gold/20` | Displays current active election phase, Judicial Officer ID. Includes `CommandPalette` (`Cmd+K`) entry point. |
| **Global Metrics Ribbon** | `Card`, `Progress` | `grid grid-cols-4 gap-4 p-4 bg-judicial-slate` | 4 Cards displaying: Total Turnout, Active Terminals, Alerts, Quorum Progress. Progress bars use `bg-judicial-success` if legal quorum met. |
| **Main Content Grid** | `ResizablePanelGroup` | `flex-1 overflow-hidden grid grid-cols-12 gap-4 p-4` | Split into Main Log (8 cols) and Live Tally/Status (4 cols). |
| **Cryptographic Log Table** | `Table` (virtualized), `Badge` | `bg-white rounded-lg shadow-sm border border-gray-200` | Displays live voting events. Uses `Badge` (Solid/Outline) for event types (e.g., 'Voted', 'Anomaly'). Custom row renderers. |
| **Terminal Status Map** | `ScrollArea`, `HoverCard` | `bg-white p-4 rounded-lg` | List of connected Kiosks. Green/Red indicator dots. Hovering shows IP, latency, and battery/power status. |
| **Emergency Speed Dial** | Custom component or `DropdownMenu` positioned fixed. | `fixed bottom-6 end-6 bg-judicial-danger text-white rounded-full shadow-2xl` | Requires high friction (e.g., hold-to-confirm or double-validation dialog) to trigger "Pause Election" or "Lockdown". |
| **System Loading State** | Custom `Skeleton` wrappers | `animate-pulse bg-gray-200/50` | Replaces full-page spinners to prevent layout shift during aggressive background polling. |

---

## 2. Real-Time Live-Tallying Grid

**Purpose:** Algorithmic calculation of votes, distribution tracking, and final legal report generation.
**Layout Type:** Fluid Grid with Data-Dense Tables and Chart overlays.

| Region / Component | Shadcn UI Elements | Tailwind Classes & Theming | Interaction & State |
| :--- | :--- | :--- | :--- |
| **Control Header** | `Select`, `Tabs`, `Button` | `flex justify-between items-center bg-white p-4 border-b` | Segmented controls (`Tabs`) to switch between Syndical Tiers (e.g., General Assembly vs. Branch). Export PDF button. |
| **Constituency Tally Card** | `Card` | `grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 p-6` | Each card represents a specific seat or quota category. |
| **Candidate Row (Inside Card)** | `Avatar`, `Progress` | `flex items-center space-x-reverse space-x-4 py-3 border-b last:border-0` | Uses RTL logical spacing (`space-x-reverse`). Displays candidate photo, name, and vote bar. The leading candidate receives a `judicial-gold` border or crown icon. |
| **Micro-Sparkline** | Custom SVG / Recharts | `h-8 w-24 opacity-80` | Displays the velocity of incoming votes for that specific candidate over the last 30 minutes. |
| **Winner Declaration Banner** | `Alert` | `bg-judicial-success/10 border-judicial-success text-judicial-success mt-4` | Highlighted alert box when math eliminates all challengers (e.g., > 50% + 1 secured). |
| **Report Generation Dialog** | `Dialog`, `ScrollArea` | `max-w-4xl max-h-[80vh]` | A highly formal, PDF-like preview modal requiring digital signature confirmation before locking the results. |
