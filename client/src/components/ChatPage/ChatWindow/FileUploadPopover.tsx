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
        className="relative w-[90vw] max-w-[480px] rounded-2xl bg-white p-6 md:p-8 shadow-xl"
        onClick={(e) => e.stopPropagation()}
      >
        <button
          onClick={onClose}
          className="absolute top-4 right-4 text-gray-400 hover:text-gray-600 transition-colors"
          aria-label="Close"
        >
          <CloseModalIcon />
        </button>

        <h2 className="mb-5 text-xl font-semibold text-gray-900">
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
            <span className="text-sm text-gray-700">Degree Audit</span>
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
            <span className="text-sm text-gray-700">Other Document</span>
          </label>
        </div>

        <div
          className="flex cursor-pointer flex-col items-center justify-center gap-3 rounded-xl border-2 border-dashed border-gray-200 bg-gray-50 py-14 transition-colors hover:bg-gray-100"
          onDrop={handleDrop}
          onDragOver={handleDragOver}
          onClick={() => inputRef.current?.click()}
        >
          <UploadImageIcon />
          <span className="text-sm font-medium text-gray-400">
            Drag and drop or click here
          </span>
        </div>

        <p className="mt-4 text-center text-xs text-gray-400">
          Accepted file types:{" "}
          {ACCEPTED_LABEL.split(", ").map((t, i) => (
            <span key={t}>
              {i > 0 && ", "}
              <span className="text-blue-500">{t}</span>
            </span>
          ))}
        </p>

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
