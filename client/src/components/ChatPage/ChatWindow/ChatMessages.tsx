import type { Message } from "../../../hooks/useChatMessages";

interface ChatMessagesProps {
  messages: Message[];
  messagesEndRef: React.RefObject<HTMLDivElement | null>;
}

const ChatMessages = ({ messages, messagesEndRef }: ChatMessagesProps) => (
  <div className="flex flex-1 flex-col gap-4 overflow-y-auto px-8 py-6 no-scrollbar">
    {messages.map((msg) => (
      <div key={msg.id} className="flex justify-end">
        <div className="animate-slide-up max-w-sm rounded-2xl rounded-tr-sm bg-gray-100 px-4 py-3 text-sm leading-relaxed text-gray-800">
          {msg.text}
        </div>
      </div>
    ))}
    <div ref={messagesEndRef} />
  </div>
);

export default ChatMessages;
