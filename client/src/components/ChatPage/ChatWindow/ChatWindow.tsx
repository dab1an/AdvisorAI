import { useState, useEffect, useRef } from "react";

import { useCyclingWords } from "../../../hooks/useCyclingWords";
import { useChatMessages } from "../../../hooks/useChatMessages";

import PreChatScreen from "./PreChatScreen";
import ChatMessages from "./ChatMessages";
import InputBox from "./InputBox";

import { CYCLING_WORDS, WORD_INTERVAL_MS } from "../../../hooks/chat";

const ChatWindow = () => {
  const [hasSentFirstMessage, setHasSentFirstMessage] = useState(false);
  const [inputValue, setInputValue] = useState("");
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const { messages, addMessage, resetMessages } = useChatMessages();

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

  const handleSend = () => {
    const trimmed = inputValue.trim();
    if (!trimmed) return;

    if (!hasSentFirstMessage) {
      setTimeout(() => {
        setHasSentFirstMessage(true);
        resetMessages(trimmed);
        setInputValue("");
      }, 400);
    } else {
      addMessage(trimmed);
      setInputValue("");
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  return (
    <div className="relative flex h-full w-full flex-col overflow-hidden bg-white">
      {!hasSentFirstMessage ? (
        <PreChatScreen
          currentWord={CYCLING_WORDS[index]}
          inputValue={inputValue}
          setInputValue={setInputValue}
          onSend={handleSend}
          onKeyDown={handleKeyDown}
        />
      ) : (
        <div className="flex h-full flex-col px-20">
          <ChatMessages messages={messages} messagesEndRef={messagesEndRef} />

          <div className="px-8 pt-2 pb-6">
            <InputBox
              value={inputValue}
              onChange={setInputValue}
              onKeyDown={handleKeyDown}
              onSend={handleSend}
              placeholder="Ask a question..."
            />
          </div>
        </div>
      )}
    </div>
  );
};

export default ChatWindow;
