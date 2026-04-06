import { useState, useEffect } from "react";

export function useCyclingWords(
  words: string[],
  intervalMs: number,
  isActive: boolean,
) {
  const [index, setIndex] = useState(0);

  useEffect(() => {
    if (!isActive) return;

    const interval = setInterval(() => {
      setIndex((prev) => (prev + 1) % words.length);
    }, intervalMs);

    return () => clearInterval(interval);
  }, [isActive, words.length, intervalMs]);

  return { index };
}
