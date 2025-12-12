'use client';

import { useEffect } from 'react';
import { ThemeProvider } from '@/contexts/theme-context';
import { useAppKeyboardShortcuts } from '@/hooks/use-keyboard-shortcuts';
import './globals.css';

function KeyboardShortcutsWrapper({ children }: { children: React.ReactNode }) {
  useAppKeyboardShortcuts();
  return <>{children}</>;
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body>
        <ThemeProvider>
          <KeyboardShortcutsWrapper>
            {children}
          </KeyboardShortcutsWrapper>
        </ThemeProvider>
      </body>
    </html>
  );
}
