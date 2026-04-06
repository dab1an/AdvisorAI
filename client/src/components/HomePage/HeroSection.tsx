import { useRef, useState, type MouseEvent } from "react";
import { Link } from "react-router-dom";
import robotHand from "../../assets/RobotHandNoBackground.png";
import humanHand from "../../assets/human-hand Background Removed.png";
import graduationCap from "../../assets/Cap2.png";

const NAVBAR_HEIGHT_PX = 100;

const HeroSection = () => {
  const iconRef = useRef<HTMLDivElement>(null);
  const [tilt, setTilt] = useState({ rotateX: 0, rotateY: 0 });

  const handleMouseMove = (e: MouseEvent<HTMLElement>) => {
    if (!iconRef.current) return;
    const rect = iconRef.current.getBoundingClientRect();
    const centerX = rect.left + rect.width / 2;
    const centerY = rect.top + rect.height / 2;
    const maxTilt = 20;
    const rotateY = ((e.clientX - centerX) / (window.innerWidth / 2)) * maxTilt;
    const rotateX =
      -((e.clientY - centerY) / (window.innerHeight / 2)) * maxTilt;
    setTilt({ rotateX, rotateY });
  };

  const handleMouseLeave = () => {
    setTilt({ rotateX: 0, rotateY: 0 });
  };

  return (
    <section
      id="home"
      className="relative flex flex-col items-center justify-center overflow-hidden px-6"
      style={{
        minHeight: `calc(100vh - ${NAVBAR_HEIGHT_PX}px)`,
        paddingTop: `${NAVBAR_HEIGHT_PX}px`,
        background:
          "radial-gradient(ellipse 60% 55% at 50% 45%, #00416a 0%, #002a45 20%, #0f1f35 40%, #050a10 65%, #000000 85%)",
      }}
      onMouseMove={handleMouseMove}
      onMouseLeave={handleMouseLeave}
    >
      {/* Hero text */}
      <h1 className="relative z-10 font-instrument text-4xl md:text-5xl lg:text-7xl leading-tight text-white text-center animate-fade-in">
        Your Personal
        <br />
        <span className="text-app-gold">Academic Advisor</span>
      </h1>

      <p
        className="relative z-10 mt-4 lg:mt-6 max-w-xl text-center text-base lg:text-lg text-gray-400 animate-fade-in"
        style={{ animationDelay: "0.2s", animationFillMode: "both" }}
      >
        AI-powered academic advising for FIU students. Get instant answers about
        your degree, courses, and academic path.
      </p>

      <Link
        to="/chat"
        className="relative z-10 mt-8 lg:mt-10 flex items-center gap-2 rounded-full bg-app-gold px-6 lg:px-8 py-3 text-base lg:text-lg font-semibold text-white hover:brightness-110 animate-fade-in liquid-glass liquid-glass-glow"
        style={{ animationDelay: "0.4s", animationFillMode: "both" }}
      >
        Start Chatting
        <span className="ml-1">&rarr;</span>
      </Link>

      {/* Center icon with float + 3D tilt + glow */}
      <div
        className="relative z-10 mt-10 lg:mt-16 animate-fade-in"
        style={{
          animationDelay: "0.6s",
          animationFillMode: "both",
        }}
      >
        <div
          ref={iconRef}
          className="animate-float"
          style={{ perspective: "800px" }}
        >
          <img
            src={graduationCap}
            alt="Graduation Cap"
            className="relative h-24 md:h-44 lg:h-56 animate-pulse-glow"
            style={{
              transform: `rotateX(${tilt.rotateX}deg) rotateY(${tilt.rotateY}deg)`,
              transition: "transform 0.15s ease-out",
            }}
          />
        </div>
      </div>

      {/* Robot hand -- left side */}
      <img
        src={robotHand}
        alt="Robot hand"
        className="pointer-events-none absolute left-[-3%] bottom-[28%] md:bottom-auto md:top-[38%] lg:top-[30%] h-[28vw] md:h-[35vw] lg:h-[30vw] max-h-140 object-contain"
      />

      {/* Human hand -- right side, bottom corner */}
      <img
        src={humanHand}
        alt="Human hand"
        className="pointer-events-none absolute right-[-3%] bottom-[18%] md:bottom-0 h-[22vw] md:h-[28vw] lg:h-[25vw] max-h-100 object-contain"
      />
    </section>
  );
};

export default HeroSection;
