import { useState, useEffect, useRef } from "react";

import { useCyclingWords } from "../../../hooks/useCyclingWords";
import { useChatMessages } from "../../../hooks/useChatMessages";

import PreChatScreen from "./PreChatScreen";
import ChatMessages from "./ChatMessages";
import InputBox from "./InputBox";
import FileUploadPopover from "./FileUploadPopover";
import DegreeAuditModal from "./DegreeAuditModal";
import FilePreview from "./FilePreview";
import type { UploadedFile } from "./FileUploadPopover";

import { CYCLING_WORDS, WORD_INTERVAL_MS } from "../../../hooks/chat";

interface ChatWindowProps {
  onMenuClick: () => void;
  newChatSignal: number;
}

const ChatWindow = ({ onMenuClick, newChatSignal }: ChatWindowProps) => {
  const [hasSentFirstMessage, setHasSentFirstMessage] = useState(false);
  const [inputValue, setInputValue] = useState("");
  const [showUploadPopover, setShowUploadPopover] = useState(false);
  const [showAuditGuide, setShowAuditGuide] = useState(false);
  const [uploadedFile, setUploadedFile] = useState<UploadedFile | null>(null);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const { messages, addMessage, resetMessages, startNewChat } = useChatMessages();

  const { index } = useCyclingWords(
    CYCLING_WORDS,
    WORD_INTERVAL_MS,
    !hasSentFirstMessage,
  );

  useEffect(() => {
    if (hasSentFirstMessage) {
      messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    }
  }, [messages, hasSentFirstMessage]);

  useEffect(() => {
    setHasSentFirstMessage(false);
    setInputValue("");
    setShowUploadPopover(false);
    setUploadedFile(null);
    startNewChat();
  }, [newChatSignal, startNewChat]);

  const handleSend = () => {
    const trimmed = inputValue.trim();
    if (!trimmed) return;

    if (!hasSentFirstMessage) {
      setTimeout(() => {
        setHasSentFirstMessage(true);
        resetMessages(trimmed, uploadedFile);
        setInputValue("");
        setUploadedFile(null);
      }, 400);
    } else {
      addMessage(trimmed, uploadedFile);
      setInputValue("");
      setUploadedFile(null);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  return (
    <div className="relative flex h-full w-full flex-col overflow-hidden bg-white dark:bg-slate-950">
      {/* Hamburger button — mobile only */}
      <button
        onClick={onMenuClick}
        className="absolute top-4 left-4 z-20 flex flex-col gap-1 p-1 md:hidden"
        aria-label="Open menu"
      >
        <span className="block h-0.5 w-5 bg-gray-600 dark:bg-gray-300" />
        <span className="block h-0.5 w-5 bg-gray-600 dark:bg-gray-300" />
        <span className="block h-0.5 w-5 bg-gray-600 dark:bg-gray-300" />
      </button>

      {showUploadPopover && (
        <FileUploadPopover
          onClose={() => setShowUploadPopover(false)}
          onFileSelect={(f) => setUploadedFile(f)}
        />
      )}

      {showAuditGuide && (
        <DegreeAuditModal
          onClose={() => setShowAuditGuide(false)}
          onFileSelect={(f) => setUploadedFile(f)}
        />
      )}

      {!hasSentFirstMessage ? (
        <PreChatScreen
          currentWord={CYCLING_WORDS[index]}
          inputValue={inputValue}
          setInputValue={setInputValue}
          onSend={handleSend}
          onKeyDown={handleKeyDown}
          onAttachClick={() => setShowUploadPopover(true)}
          onGuideClick={() => setShowAuditGuide(true)}
          uploadedFile={uploadedFile}
          onRemoveFile={() => setUploadedFile(null)}
        />
      ) : (
        <div className="flex h-full flex-col px-4 md:px-20">
          <ChatMessages messages={messages} messagesEndRef={messagesEndRef} />
          <div className="px-8 pt-2 pb-6">
            {uploadedFile && (
              <div className="mb-2 flex">
                <FilePreview
                  uploadedFile={uploadedFile}
                  onRemove={() => setUploadedFile(null)}
                />
              </div>
            )}
            <InputBox
              value={inputValue}
              onChange={setInputValue}
              onKeyDown={handleKeyDown}
              onSend={handleSend}
              placeholder="Ask a question..."
              onAttachClick={() => setShowUploadPopover(true)}
            />
          </div>
        </div>
      )}
    </div>
  );
};

export default ChatWindow;
