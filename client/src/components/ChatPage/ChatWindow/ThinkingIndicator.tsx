const bars = [{ width: "w-72" }, { width: "w-96" }, { width: "w-52" }];

const ThinkingIndicator = () => (
  <div className="flex flex-col gap-4">
    <span className="text-md font-light text-gray-400">Thinking...</span>
    <div className="flex flex-col gap-3">
      {bars.map((bar, i) => (
        <div
          key={i}
          className={`${bar.width} h-3 rounded-full bg-gray-200 dark:bg-slate-800 overflow-hidden`}
        >
          <div className="h-full w-full animate-shimmer rounded-full bg-linear-to-r from-gray-200 via-gray-100 to-gray-200 dark:from-slate-800 dark:via-slate-700 dark:to-slate-800 bg-size-[200%_100%]" />
        </div>
      ))}
    </div>
  </div>
);

export default ThinkingIndicator;
