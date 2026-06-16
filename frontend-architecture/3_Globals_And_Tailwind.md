# Global Styling & Tailwind Configuration

To achieve the precise, sovereign aesthetic required by the Egyptian Judiciary, we use modern `oklch` color spaces for flawless Light/Dark mode transitions.

## 1. Global CSS (`globals.css`)

Inject the approved design tokens to control the Shadcn UI primitives and custom elements.

```css
@tailwind base;
@tailwind components;
@tailwind utilities;

@layer base {
  :root {
    --background: oklch(1 0 0);
    --foreground: oklch(0.148 0.004 228.8);
    --card: oklch(1 0 0);
    --card-foreground: oklch(0.148 0.004 228.8);
    --popover: oklch(1 0 0);
    --popover-foreground: oklch(0.148 0.004 228.8);
    --primary: oklch(0.555 0.163 48.998);
    --primary-foreground: oklch(0.987 0.022 95.277);
    --secondary: oklch(0.967 0.001 286.375);
    --secondary-foreground: oklch(0.21 0.006 285.885);
    --muted: oklch(0.963 0.002 197.1);
    --muted-foreground: oklch(0.56 0.021 213.5);
    --accent: oklch(0.963 0.002 197.1);
    --accent-foreground: oklch(0.218 0.008 223.9);
    --destructive: oklch(0.577 0.245 27.325);
    --border: oklch(0.925 0.005 214.3);
    --input: oklch(0.925 0.005 214.3);
    --ring: oklch(0.723 0.014 214.4);
    --chart-1: oklch(0.879 0.169 91.605);
    --chart-2: oklch(0.769 0.188 70.08);
    --chart-3: oklch(0.666 0.179 58.318);
    --chart-4: oklch(0.555 0.163 48.998);
    --chart-5: oklch(0.473 0.137 46.201);
    --radius: 0.625rem;
    --sidebar: oklch(0.987 0.002 197.1);
    --sidebar-foreground: oklch(0.148 0.004 228.8);
    --sidebar-primary: oklch(0.666 0.179 58.318);
    --sidebar-primary-foreground: oklch(0.987 0.022 95.277);
    --sidebar-accent: oklch(0.963 0.002 197.1);
    --sidebar-accent-foreground: oklch(0.218 0.008 223.9);
    --sidebar-border: oklch(0.925 0.005 214.3);
    --sidebar-ring: oklch(0.723 0.014 214.4);
  }

  .dark {
    --background: oklch(0.148 0.004 228.8);
    --foreground: oklch(0.987 0.002 197.1);
    --card: oklch(0.218 0.008 223.9);
    --card-foreground: oklch(0.987 0.002 197.1);
    --popover: oklch(0.218 0.008 223.9);
    --popover-foreground: oklch(0.987 0.002 197.1);
    --primary: oklch(0.473 0.137 46.201);
    --primary-foreground: oklch(0.987 0.022 95.277);
    --secondary: oklch(0.274 0.006 286.033);
    --secondary-foreground: oklch(0.985 0 0);
    --muted: oklch(0.275 0.011 216.9);
    --muted-foreground: oklch(0.723 0.014 214.4);
    --accent: oklch(0.275 0.011 216.9);
    --accent-foreground: oklch(0.987 0.002 197.1);
    --destructive: oklch(0.704 0.191 22.216);
    --border: oklch(1 0 0 / 10%);
    --input: oklch(1 0 0 / 15%);
    --ring: oklch(0.56 0.021 213.5);
    --chart-1: oklch(0.879 0.169 91.605);
    --chart-2: oklch(0.769 0.188 70.08);
    --chart-3: oklch(0.666 0.179 58.318);
    --chart-4: oklch(0.555 0.163 48.998);
    --chart-5: oklch(0.473 0.137 46.201);
    --sidebar: oklch(0.218 0.008 223.9);
    --sidebar-foreground: oklch(0.987 0.002 197.1);
    --sidebar-primary: oklch(0.769 0.188 70.08);
    --sidebar-primary-foreground: oklch(0.279 0.077 45.635);
    --sidebar-accent: oklch(0.275 0.011 216.9);
    --sidebar-accent-foreground: oklch(0.987 0.002 197.1);
    --sidebar-border: oklch(1 0 0 / 10%);
    --sidebar-ring: oklch(0.56 0.021 213.5);
  }
}

@layer base {
  * {
    @apply border-border;
  }
  body {
    @apply bg-background text-foreground font-arabic;
  }
}
```

## 2. Tailwind Config Integration (`tailwind.config.ts`)

```typescript
import type { Config } from "tailwindcss"

const config = {
  darkMode: ["class"],
  content: [
    './pages/**/*.{ts,tsx}',
    './components/**/*.{ts,tsx}',
    './app/**/*.{ts,tsx}',
    './src/**/*.{ts,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        border: "var(--border)",
        input: "var(--input)",
        ring: "var(--ring)",
        background: "var(--background)",
        foreground: "var(--foreground)",
        primary: {
          DEFAULT: "var(--primary)",
          foreground: "var(--primary-foreground)",
        },
        secondary: {
          DEFAULT: "var(--secondary)",
          foreground: "var(--secondary-foreground)",
        },
        destructive: {
          DEFAULT: "var(--destructive)",
          foreground: "var(--destructive-foreground)", // Fallback to white if needed
        },
        muted: {
          DEFAULT: "var(--muted)",
          foreground: "var(--muted-foreground)",
        },
        accent: {
          DEFAULT: "var(--accent)",
          foreground: "var(--accent-foreground)",
        },
        popover: {
          DEFAULT: "var(--popover)",
          foreground: "var(--popover-foreground)",
        },
        card: {
          DEFAULT: "var(--card)",
          foreground: "var(--card-foreground)",
        },
        chart: {
          '1': 'var(--chart-1)',
          '2': 'var(--chart-2)',
          '3': 'var(--chart-3)',
          '4': 'var(--chart-4)',
          '5': 'var(--chart-5)',
        }
      },
      borderRadius: {
        lg: "var(--radius)",
        md: "calc(var(--radius) - 2px)",
        sm: "calc(var(--radius) - 4px)",
      },
      fontFamily: {
        arabic: ['"Cairo"', 'sans-serif'],
        numerals: ['"Inter"', 'sans-serif'],
      },
    },
  },
  plugins: [require("tailwindcss-animate")],
}
export default config
```
