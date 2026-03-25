import "../index.css";
import { useState } from "react";

import Sidebar from "../components/ChatPage/Sidebar/Sidebar";
import ChatWindow from "../components/ChatPage/ChatWindow/ChatWindow";

function ChatPage() {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [newChatSignal, setNewChatSignal] = useState(0);

  const handleNewChat = () => {
    setNewChatSignal((prev) => prev + 1);
    setSidebarOpen(false);
  };

  return (
    <div className="flex h-screen w-screen overflow-hidden">
      <Sidebar
        isOpen={sidebarOpen}
        onClose={() => setSidebarOpen(false)}
        onNewChat={handleNewChat}
      />
      <ChatWindow
        onMenuClick={() => setSidebarOpen(true)}
        newChatSignal={newChatSignal}
      />
    </div>
  );
}

export default ChatPage;
