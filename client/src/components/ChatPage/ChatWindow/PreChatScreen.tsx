import React from "react";
import InputBox from "./InputBox";

interface PreChatScreenProps {
  currentWord: string;
  inputValue: string;
  setInputValue: (v: string) => void;
  onSend: () => void;
  onKeyDown: (e: React.KeyboardEvent<HTMLTextAreaElement>) => void;
}

const PreChatScreen = ({
  currentWord,
  inputValue,
  setInputValue,
  onSend,
  onKeyDown,
}: PreChatScreenProps) => (
  <div className="flex h-full flex-col items-center justify-center gap-10 px-8">
    <div className="font-instrument flex items-baseline gap-2 select-none">
      <h1 className="text-app-blue text-6xl font-normal">Need Info on</h1>
      <span className="text-app-gold inline-block text-6xl font-normal">
        <p>{currentWord}?</p>
      </span>
    </div>

    <div className="w-full max-w-xl">
      <InputBox
        value={inputValue}
        onChange={setInputValue}
        onKeyDown={onKeyDown}
        onSend={onSend}
        placeholder="Ask a question..."
      />
    </div>
  </div>
);

export default PreChatScreen;
