import React from "react";

export interface InputBoxProps {
  value: string;
  onChange: (v: string) => void;
  onKeyDown: (e: React.KeyboardEvent<HTMLTextAreaElement>) => void;
  onSend: () => void;
  placeholder: string;
}

const InputBox = ({
  value,
  onChange,
  onKeyDown,
  onSend,
  placeholder,
}: InputBoxProps) => (
  <div className="relative w-full rounded-2xl border border-gray-200 bg-white px-4 pt-3 pb-12 shadow-sm">
    <textarea
      className="w-full resize-none bg-transparent text-sm leading-relaxed text-gray-700 placeholder-gray-400 outline-none no-scrollbar"
      rows={2}
      placeholder={placeholder}
      value={value}
      onChange={(e) => onChange(e.target.value)}
      onKeyDown={onKeyDown}
    />

    <div className="absolute right-3 bottom-3 flex items-center gap-2">
      <button
        className="p-1 transition-colors text-app-blue hover:text-gray-600"
        aria-label="Attach file"
      >
        <svg
          width="18"
          height="18"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
          strokeLinecap="round"
          strokeLinejoin="round"
        >
          <path d="M21.44 11.05l-9.19 9.19a6 6 0 0 1-8.49-8.49l9.19-9.19a4 4 0 0 1 5.66 5.66L9.41 17.41a2 2 0 0 1-2.83-2.83l8.49-8.48" />
        </svg>
      </button>

      <button
        onClick={onSend}
        disabled={!value.trim()}
        className="flex bg-app-blue rounded-xl p-2 text-white transition-all hover:opacity-90 disabled:opacity-40 justify-center items-center"
        aria-label="Send"
      >
        <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
          <path d="M2.01 21L23 12 2.01 3 2 10l15 2-15 2z" />
        </svg>
      </button>
    </div>
  </div>
);

export default InputBox;
