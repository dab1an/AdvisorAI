import "../index.css";

import Sidebar from "../components/ChatPage/Sidebar/Sidebar";
import ChatWindow from "../components/ChatPage/ChatWindow/ChatWindow";

function ChatPage() {
  return (
    <div className="flex h-screen w-screen">
      <Sidebar />
      <ChatWindow />
    </div>
  );
}

export default ChatPage;
