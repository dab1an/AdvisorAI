/** @type {import('tailwindcss').Config} */
export default {
  theme: {
    extend: {
      colors: {
        "app-blue": "#081F3F",
        "app-gold": "#B6862C",
        "app-yellow": "#FFCC02",
        "app-gray": "#DCDCDC",
        "font-color-bot": "#4D4D4D",
      },
      keyframes: {
        slideUp: {
          "0%": {
            opacity: "0",
            transform: "translateY(8px)",
          },
          "100%": {
            opacity: "1",
            transform: "translateY(0)",
          },
        },
        shimmer: {
          "0%": { backgroundPosition: "200% 0" },
          "100%": { backgroundPosition: "-200% 0" },
        },
      },
      animation: {
        "slide-up": "slideUp 0.25s ease forwards",
        shimmer: "shimmer 2s ease-in-out infinite",
      },
      fontFamily: {
        instrument: ['"instrument-serif"', "sans-serif"],
      },
    },
  },
};
