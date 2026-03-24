import "../index.css";
import { useState } from "react";

import Sidebar from "../components/ChatPage/Sidebar/Sidebar";
import ChatWindow from "../components/ChatPage/ChatWindow/ChatWindow";

function ChatPage() {
  const [sidebarOpen, setSidebarOpen] = useState(false);

  return (
    <div className="flex h-screen w-screen overflow-hidden">
      <Sidebar isOpen={sidebarOpen} onClose={() => setSidebarOpen(false)} />
      <ChatWindow onMenuClick={() => setSidebarOpen(true)} />
    </div>
  );
}

export default ChatPage;
