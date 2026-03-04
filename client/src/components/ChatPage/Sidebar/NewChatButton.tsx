// ik it looks too simple to be a component but we'll add functionality with hooks later

const NewChatButton = () => {
  return (
    <button className="text-app-blue bg-app-yellow font flex h-[80] w-[27.5] items-center justify-center rounded-lg px-5 py-2 font-bold drop-shadow-lg hover:cursor-pointer">
      <p>New Chat</p>
    </button>
  );
};

export default NewChatButton;
