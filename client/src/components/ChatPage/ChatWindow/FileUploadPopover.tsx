import React, { useRef, useCallback, useState } from "react";
import UploadImageIcon from "../../../assets/icons/UploadImageIcon";
import CloseModalIcon from "../../../assets/icons/CloseModalIcon";

export interface UploadedFile {
  file: File;
  previewUrl: string | null;
  fileType: "audit" | "document" | "image";
}

interface FileUploadPopoverProps {
  onClose: () => void;
  onFileSelect: (uploaded: UploadedFile) => void;
}

const ACCEPTED = ["application/pdf", "image/png", "image/jpeg"];
const ACCEPTED_LABEL = "pdf, png, jpeg";

const FileUploadPopover = ({
  onClose,
  onFileSelect,
}: FileUploadPopoverProps) => {
  const inputRef = useRef<HTMLInputElement>(null);
  const [selectedType, setSelectedType] = useState<"audit" | "document">("audit");
  const [showGuide, setShowGuide] = useState(false);

  const processFile = useCallback(
  (file: File) => {
    if (!ACCEPTED.includes(file.type)) return;

    if (file.type.startsWith("image/")) {
      const previewUrl = URL.createObjectURL(file);
      onFileSelect({ file, previewUrl, fileType: "image" });
      onClose();
      return;
    }

    onFileSelect({ file, previewUrl: null, fileType: selectedType });
    onClose();
  },
  [onFileSelect, onClose, selectedType],
);

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    const file = e.dataTransfer.files?.[0];
    if (file) processFile(file);
  };

  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
  };

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
        className="relative w-[90vw] max-w-[480px] rounded-2xl bg-white p-6 md:p-8 shadow-xl dark-glass"
        onClick={(e) => e.stopPropagation()}
      >
        <button
          onClick={onClose}
          className="absolute top-4 right-4 text-gray-400 hover:text-gray-600 dark:hover:text-gray-200 transition-colors cursor-pointer"
          aria-label="Close"
        >
          <CloseModalIcon />
        </button>

        {showGuide ? (
          <>
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
              onClick={() => {
                setShowGuide(false);
                inputRef.current?.click();
              }}
              className="mt-6 w-full rounded-xl bg-app-blue py-3 text-sm font-semibold text-white transition-colors hover:bg-app-blue/90 cursor-pointer"
            >
              Upload Degree Audit
            </button>
          </>
        ) : (
          <>
            <h2 className="mb-5 text-xl font-semibold text-gray-900 dark:text-white">
              File Upload
            </h2>

            <div className="mb-5 flex gap-6">
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="radio"
                  name="fileType"
                  value="audit"
                  checked={selectedType === "audit"}
                  onChange={() => setSelectedType("audit")}
                  className="accent-app-blue"
                />
                <span className="text-sm text-gray-700 dark:text-gray-300">Degree Audit</span>
              </label>
              <label className="flex items-center gap-2 cursor-pointer">
                <input
                  type="radio"
                  name="fileType"
                  value="document"
                  checked={selectedType === "document"}
                  onChange={() => setSelectedType("document")}
                  className="accent-app-blue"
                />
                <span className="text-sm text-gray-700 dark:text-gray-300">Other Document</span>
              </label>
            </div>

            <div
              className="flex cursor-pointer flex-col items-center justify-center gap-3 rounded-xl border-2 border-dashed border-gray-200 dark:border-slate-600 bg-gray-50 dark:bg-slate-800 py-14 transition-colors hover:bg-gray-100 dark:hover:bg-slate-700"
              onDrop={handleDrop}
              onDragOver={handleDragOver}
              onClick={() => inputRef.current?.click()}
            >
              <UploadImageIcon />
              <span className="text-sm font-medium text-gray-400 dark:text-slate-400">
                Drag and drop or click here
              </span>
            </div>

            <p className="mt-4 text-center text-xs text-gray-400 dark:text-slate-400">
              Accepted file types:{" "}
              {ACCEPTED_LABEL.split(", ").map((t, i) => (
                <span key={t}>
                  {i > 0 && ", "}
                  <span className="text-blue-500 dark:text-blue-400">{t}</span>
                </span>
              ))}
            </p>

            {selectedType === "audit" && (
              <p className="mt-4 text-center text-xs text-gray-400 dark:text-slate-400">
                Don't have your audit yet?{" "}
                <button
                  onClick={() => setShowGuide(true)}
                  className="font-medium text-app-blue dark:text-app-gold hover:underline cursor-pointer"
                >
                  Need personalized help? Learn how!
                </button>
              </p>
            )}
          </>
        )}

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

export default FileUploadPopover;
