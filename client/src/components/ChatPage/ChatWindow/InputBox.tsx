import type { KeyboardEvent } from "react";
import AttachmentIcon from "../../../assets/icons/AttachmentIcon";
import SendIcon from "../../../assets/icons/SendIcon";

interface InputBoxProps {
  value: string;
  onChange: (v: string) => void;
  onKeyDown: (e: KeyboardEvent<HTMLTextAreaElement>) => void;
  onSend: () => void;
  placeholder: string;
  onAttachClick: () => void;
}

const InputBox = ({
  value,
  onChange,
  onKeyDown,
  onSend,
  placeholder,
  onAttachClick,
}: InputBoxProps) => (
  <div className="relative w-full rounded-2xl border border-gray-200 bg-white px-4 pt-3 pb-12 shadow-sm dark-glass">
    <textarea
      className="w-full resize-none bg-transparent text-sm leading-relaxed text-gray-700 dark:text-gray-200 placeholder-gray-300 dark:placeholder-slate-500 outline-none no-scrollbar"
      rows={2}
      placeholder={placeholder}
      value={value}
      onChange={(e) => onChange(e.target.value)}
      onKeyDown={onKeyDown}
    />

    <div className="absolute right-3 bottom-3 flex items-center gap-2">
      <button
        onClick={onAttachClick}
        className="p-1 transition-colors text-app-blue dark:text-gray-200 hover:text-gray-600 dark:hover:text-white"
        aria-label="Attach file"
      >
        <AttachmentIcon />
      </button>

      <button
        onClick={onSend}
        disabled={!value.trim()}
        className="flex bg-app-blue dark:bg-app-gold rounded-xl p-2 text-white transition-colors hover:opacity-90 cursor-pointer disabled:opacity-40 justify-center items-center"
        aria-label="Send"
      >
        <SendIcon />
      </button>
    </div>
  </div>
);

export default InputBox;
