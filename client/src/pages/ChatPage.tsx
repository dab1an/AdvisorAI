import { useState } from "react";

import Sidebar from "../components/ChatPage/Sidebar/Sidebar";
import ChatWindow from "../components/ChatPage/ChatWindow/ChatWindow";

function ChatPage() {
<<<<<<< HEAD
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [newChatSignal, setNewChatSignal] = useState(0);

  const handleNewChat = () => {
    setNewChatSignal((prev) => prev + 1);
    setSidebarOpen(false);
  };

  return (
=======
<<<<<<< HEAD
  return (
    <div className="flex h-screen w-screen">
      <Sidebar />
      <ChatWindow />
=======
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [newChatSignal, setNewChatSignal] = useState(0);

  const handleNewChat = () => {
    setNewChatSignal((prev) => prev + 1);
    setSidebarOpen(false);
  };

  return (
>>>>>>> ff84053 (New Chat button functionality)
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
<<<<<<< HEAD
=======
>>>>>>> a59bb79 (New Chat button functionality)
>>>>>>> ff84053 (New Chat button functionality)
    </div>
  );
}

export default ChatPage;
