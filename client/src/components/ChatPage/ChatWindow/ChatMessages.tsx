import React from "react";
import ReactMarkdown from "react-markdown";
import type { Components } from "react-markdown";
import type { Message } from "../../../hooks/useChatMessages";
import FilePreview from "./FilePreview";
import ThinkingIndicator from "./ThinkingIndicator";

interface ChatMessagesProps {
  messages: Message[];
  messagesEndRef: React.RefObject<HTMLDivElement | null>;
}

const markdownComponents: Components = {
  p: ({ children }) => <p className="mb-4 last:mb-0">{children}</p>,
  ul: ({ children }) => <ul className="mb-4 list-disc pl-5 last:mb-0">{children}</ul>,
  ol: ({ children }) => <ol className="mb-4 list-decimal pl-5 last:mb-0">{children}</ol>,
  li: ({ children }) => <li className="mb-1">{children}</li>,
  a: ({ href, children }) => (
    <a
      href={href}
      className="text-blue-600 underline hover:text-blue-800"
      target="_blank"
      rel="noreferrer"
    >
      {children}
    </a>
  ),
  strong: ({ children }) => <strong className="font-semibold">{children}</strong>,
};

const ChatMessages = ({ messages, messagesEndRef }: ChatMessagesProps) => (
  <div className="flex flex-1 flex-col gap-4 overflow-y-auto px-8 py-6 no-scrollbar">
    {messages.map((msg) => {
      const isUser = msg.sender === "user";
      return (
        <div
          key={msg.id}
          className={`flex flex-col gap-1.5 ${isUser ? "items-end" : "items-start"}`}
        >
          {isUser ? (
            <div className="animate-slide-up max-w-[80%] md:max-w-sm rounded-2xl rounded-tr-sm bg-gray-100 px-4 py-3 text-sm leading-relaxed text-gray-800">
              {msg.text}
            </div>
          ) : msg.loading ? (
            <div className="animate-slide-up">
              <ThinkingIndicator />
            </div>
          ) : (
            <div className="animate-slide-up max-w-xl text-sm leading-relaxed text-gray-800">
              <ReactMarkdown components={markdownComponents}>{msg.text}</ReactMarkdown>
            </div>
          )}
          {msg.attachedFile && (
            <FilePreview uploadedFile={msg.attachedFile} onRemove={null} />
          )}
        </div>
      );
    })}
    <div ref={messagesEndRef} />
  </div>
);

export default ChatMessages;
