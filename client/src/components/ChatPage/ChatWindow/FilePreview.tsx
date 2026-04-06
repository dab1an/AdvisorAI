import type { UploadedFile } from "./FileUploadPopover";
import FileIcon from "../../../assets/icons/FileIcon";
import RemoveIcon from "../../../assets/icons/RemoveIcon";

interface FilePreviewProps {
  uploadedFile: UploadedFile;
  onRemove: (() => void) | null;
}

const FilePreview = ({ uploadedFile, onRemove }: FilePreviewProps) => (
  <div className="inline-flex items-center gap-2 rounded-xl border border-gray-200 bg-gray-50 px-3 py-2 text-xs text-gray-600 dark:text-gray-300 shadow-sm dark-glass">
    {uploadedFile.previewUrl ? (
      <img
        src={uploadedFile.previewUrl}
        alt="preview"
        className="h-8 w-8 rounded object-cover"
      />
    ) : (
      <div className="relative flex h-10 w-10 items-center justify-center rounded-lg bg-gray-100 dark:bg-slate-700">
        <FileIcon />
      </div>
    )}
    <span className="max-w-[120px] truncate">{uploadedFile.file.name}</span>
    {onRemove && (
      <button
        onClick={onRemove}
        className="ml-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-200 transition-colors"
        aria-label="Remove file"
      >
        <RemoveIcon />
      </button>
    )}
  </div>
);

export default FilePreview;
