interface DegreeAuditGuideProps {
  onClick: () => void;
}

const DegreeAuditGuide = ({ onClick }: DegreeAuditGuideProps) => (
  <button
    onClick={onClick}
    className="speech-bubble group rounded-xl bg-white dark:bg-[#1a2535] px-4 py-2.5 text-sm font-medium text-app-blue dark:text-app-gold border border-gray-200 dark:border-[rgba(255,255,255,0.1)] shadow-sm transition-all hover:shadow-md cursor-pointer"
  >
    Need personalized help? Learn how!
  </button>
);

export default DegreeAuditGuide;
