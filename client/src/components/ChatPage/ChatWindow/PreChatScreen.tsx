import type { KeyboardEvent } from "react";
import InputBox from "./InputBox";
import FilePreview from "./FilePreview";
import DegreeAuditGuide from "./DegreeAuditGuide";
import type { UploadedFile } from "./FileUploadPopover";

interface PreChatScreenProps {
  currentWord: string;
  inputValue: string;
  setInputValue: (v: string) => void;
  onSend: () => void;
  onKeyDown: (e: KeyboardEvent<HTMLTextAreaElement>) => void;
  onAttachClick: () => void;
  onGuideClick: () => void;
  uploadedFile?: UploadedFile | null;
  onRemoveFile?: () => void;
}

const PreChatScreen = ({
  currentWord,
  inputValue,
  setInputValue,
  onSend,
  onKeyDown,
  onAttachClick,
  onGuideClick,
  uploadedFile,
  onRemoveFile,
}: PreChatScreenProps) => (
  <div className="relative flex h-full flex-col items-center justify-center gap-10 px-8 pt-14 md:pt-0">
    <div className="font-instrument flex flex-wrap items-baseline justify-center gap-2 select-none text-center">
      <h1 className="text-app-blue dark:text-white text-4xl md:text-6xl font-normal">Need Info on</h1>
      <span className="text-app-gold inline-block text-4xl md:text-6xl font-normal">
        <p>{currentWord}?</p>
      </span>
    </div>

    <div className="w-full max-w-xl">
      <div className="relative">
        <InputBox
          value={inputValue}
          onChange={setInputValue}
          onKeyDown={onKeyDown}
          onSend={onSend}
          placeholder="What advising questions do you have?"
          onAttachClick={onAttachClick}
        />
        {/* Bubble below the input box — hidden once a degree audit is uploaded */}
        {uploadedFile?.fileType !== "audit" && (
          <div className="absolute top-full mt-2 right-[33px]">
            <DegreeAuditGuide onClick={onGuideClick} />
          </div>
        )}
        {uploadedFile && onRemoveFile && (
          <div className="absolute top-full mt-2 left-0">
            <FilePreview uploadedFile={uploadedFile} onRemove={onRemoveFile} />
          </div>
        )}
      </div>
    </div>
  </div>
);

export default PreChatScreen;
