import { useCallback, useRef, useState } from "react";
import type { UploadedFile } from "../components/ChatPage/ChatWindow/FileUploadPopover";

export interface Message {
  id: number;
  text: string;
  sender: "user" | "ai";
  loading?: boolean;
  attachedFile?: UploadedFile | null;
}

async function fetchAIResponse(
  conversationId: string,
  message: string,
  file?: File | null,
  fileType?: string | null,
): Promise<string> {
  const formData = new FormData();
  formData.append("message", message);
  formData.append("conversationId", conversationId);
  if (file) formData.append("file", file);
  if (fileType) formData.append("fileType", fileType);

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
  const conversationIdRef = useRef(crypto.randomUUID());

  const sendAndGetResponse = async (text: string, file?: File | null, fileType?: string | null) => {
    const thinkingId = ++messageIdRef.current;
    setMessages((prev) => [
      ...prev,
      { id: thinkingId, text: "", sender: "ai", loading: true },
    ]);

    try {
      const response = await fetchAIResponse(
        conversationIdRef.current,
        text,
        file,
        fileType,
      );
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
    sendAndGetResponse(text, attachedFile?.file, attachedFile?.fileType);
  };

  const resetMessages = (
    firstMessage: string,
    attachedFile?: UploadedFile | null,
  ) => {
    messageIdRef.current = 0;
    conversationIdRef.current = crypto.randomUUID();
    const userMsg: Message = {
      id: ++messageIdRef.current,
      text: firstMessage,
      sender: "user",
      attachedFile,
    };
    setMessages([userMsg]);
    sendAndGetResponse(firstMessage, attachedFile?.file, attachedFile?.fileType);
  };

  const startNewChat = useCallback(() => {
    messageIdRef.current = 0;
    conversationIdRef.current = crypto.randomUUID();
    setMessages([]);
  }, []);

  return { messages, addMessage, resetMessages, startNewChat };
}
