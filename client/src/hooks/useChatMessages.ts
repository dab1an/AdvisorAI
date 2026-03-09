import { useRef, useState } from "react";
import type { UploadedFile } from "../components/ChatPage/ChatWindow/FileUploadPopover";

export interface Message {
  id: number;
  text: string;
  sender: "user" | "ai";
  loading?: boolean;
  attachedFile?: UploadedFile | null;
}

async function fetchAIResponse(
  message: string,
  file?: File | null,
): Promise<string> {
  const formData = new FormData();
  formData.append("message", message);
  if (file) formData.append("file", file);

  const response = await fetch("http://localhost:5099/api/Chat", {
    method: "POST",
    body: formData,
  });
  if (!response.ok) throw new Error("Failed to get response");
  const data = await response.json();
  return data.response;
}

export function useChatMessages() {
  const [messages, setMessages] = useState<Message[]>([]);
  const messageIdRef = useRef(0);

  const sendAndGetResponse = async (text: string, file?: File | null) => {
    const thinkingId = ++messageIdRef.current;
    setMessages((prev) => [
      ...prev,
      { id: thinkingId, text: "", sender: "ai", loading: true },
    ]);

    try {
      const response = await fetchAIResponse(text, file);
      setMessages((prev) =>
        prev.map((msg) =>
          msg.id === thinkingId
            ? { ...msg, text: response, loading: false }
            : msg,
        ),
      );
    } catch {
      setMessages((prev) =>
        prev.map((msg) =>
          msg.id === thinkingId
            ? { ...msg, text: "Sorry, something went wrong.", loading: false }
            : msg,
        ),
      );
    }
  };

  const addMessage = (text: string, attachedFile?: UploadedFile | null) => {
    setMessages((prev) => [
      ...prev,
      { id: ++messageIdRef.current, text, sender: "user", attachedFile },
    ]);
    sendAndGetResponse(text, attachedFile?.file);
  };

  const resetMessages = (
    firstMessage: string,
    attachedFile?: UploadedFile | null,
  ) => {
    messageIdRef.current = 0;
    const userMsg: Message = {
      id: ++messageIdRef.current,
      text: firstMessage,
      sender: "user",
      attachedFile,
    };
    setMessages([userMsg]);
    sendAndGetResponse(firstMessage, attachedFile?.file);
  };

  return { messages, addMessage, resetMessages };
}
