import React, { useRef, useCallback } from "react";
import CloseModalIcon from "../../../assets/icons/CloseModalIcon";
import type { UploadedFile } from "./FileUploadPopover";

const ACCEPTED = ["application/pdf", "image/png", "image/jpeg"];

interface DegreeAuditModalProps {
  onClose: () => void;
  onFileSelect: (uploaded: UploadedFile) => void;
}

const DegreeAuditModal = ({ onClose, onFileSelect }: DegreeAuditModalProps) => {
  const inputRef = useRef<HTMLInputElement>(null);

  const processFile = useCallback(
    (file: File) => {
      if (!ACCEPTED.includes(file.type)) return;
      if (file.type.startsWith("image/")) {
        const previewUrl = URL.createObjectURL(file);
        onFileSelect({ file, previewUrl, fileType: "image" });
      } else {
        onFileSelect({ file, previewUrl: null, fileType: "audit" });
      }
      onClose();
    },
    [onFileSelect, onClose],
  );

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) processFile(file);
  };

  return (
    <div
      className="absolute inset-0 z-50 flex items-center justify-center bg-black/30"
      onClick={onClose}
    >
      <div
        className="relative w-[90vw] max-w-[440px] rounded-2xl bg-white p-6 md:p-8 shadow-xl dark-glass"
        onClick={(e) => e.stopPropagation()}
      >
        <button
          onClick={onClose}
          className="absolute top-4 right-4 text-gray-400 hover:text-gray-600 dark:hover:text-gray-200 transition-colors cursor-pointer"
          aria-label="Close"
        >
          <CloseModalIcon />
        </button>

        <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
          Upload Your Degree Audit
        </h2>
        <p className="mt-1 text-sm text-gray-500 dark:text-slate-400">
          Get personalized advising based on your academic progress
        </p>

        <ol className="mt-5 space-y-2 text-sm text-gray-700 dark:text-gray-300">
          <li>
            1. Go to{" "}
            <a
              href="https://my.fiu.edu"
              target="_blank"
              rel="noopener noreferrer"
              className="font-semibold text-app-gold hover:underline"
            >
              MyFIU
            </a>
          </li>
          <li>2. Navigate to Academic Advising, then Degree Audit</li>
          <li>3. Download your audit as a PDF</li>
        </ol>

        <button
          onClick={() => inputRef.current?.click()}
          className="mt-6 w-full rounded-xl bg-app-blue py-3 text-sm font-semibold text-white transition-colors hover:bg-app-blue/90 cursor-pointer"
        >
          Upload Degree Audit
        </button>

        <input
          ref={inputRef}
          type="file"
          accept={ACCEPTED.join(",")}
          className="hidden"
          onChange={handleInputChange}
        />
      </div>
    </div>
  );
};

export default DegreeAuditModal;
