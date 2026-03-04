import { useRef, useState } from "react";

export interface Message {
  id: number;
  text: string;
  sender: "user";
}

export function useChatMessages() {
  const [messages, setMessages] = useState<Message[]>([]);
  const messageIdRef = useRef(0);

  const addMessage = (text: string) => {
    setMessages((prev) => [
      ...prev,
      { id: ++messageIdRef.current, text, sender: "user" },
    ]);
  };

  const resetMessages = (firstMessage: string) => {
    setMessages([
      { id: ++messageIdRef.current, text: firstMessage, sender: "user" },
    ]);
  };

  return { messages, addMessage, resetMessages };
}
