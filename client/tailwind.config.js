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
        glow: {
          "0%, 100%": { boxShadow: "0 0 20px 5px rgba(182, 134, 44, 0.3)" },
          "50%": { boxShadow: "0 0 40px 10px rgba(182, 134, 44, 0.6)" },
        },
        fadeIn: {
          "0%": { opacity: "0", transform: "translateY(20px)" },
          "100%": { opacity: "1", transform: "translateY(0)" },
        },
        spinSlow: {
          "0%": { transform: "rotate(0deg)" },
          "100%": { transform: "rotate(360deg)" },
        },
        shine: {
          "0%, 100%": { opacity: "0.5", filter: "brightness(1)" },
          "50%": { opacity: "1", filter: "brightness(1.4)" },
        },
      },
      animation: {
        "slide-up": "slideUp 0.25s ease forwards",
        shimmer: "shimmer 2s ease-in-out infinite",
        glow: "glow 3s ease-in-out infinite",
        "fade-in": "fadeIn 0.8s ease forwards",
        "spin-slow": "spinSlow 8s linear infinite",
        shine: "shine 3s ease-in-out infinite",
      },
      fontFamily: {
        instrument: ['"instrument-serif"', "sans-serif"],
      },
    },
  },
};
