import { useRef, useState } from "react";
import { Link } from "react-router-dom";
import brain from "../assets/Cap2.png";
import fiuLogo from "../assets/fiu_logo_hrz.png";
import externalLinkArrow from "../assets/external_link_arrow.png";
import robotHand from "../assets/RobotHandNoBackground.png";
import humanHand from "../assets/human-hand Background Removed.png";
import graduationCap from "../assets/Cap2.png";

const RESOURCE_LINKS = [
  {
    text: "Career & Talent Development",
    url: "https://career.fiu.edu/",
  },
  {
    text: "FIU Academic Calendar",
    url: "https://onestop.fiu.edu/academic-calendar/",
  },
  {
    text: "CASE Drop-In Advising",
    url: "https://case.fiu.edu/advising/",
  },
  {
    text: "Undergraduate Advising",
    url: "https://www.cis.fiu.edu/academics/advising/undergraduate/",
  },
  {
    text: "Graduate Advising",
    url: "https://cec.fiu.edu/students/academic-advising/graduate-advising/",
  },
];

const FEATURES = [
  {
    step: "1",
    title: "Upload Your Audit",
    description:
      "Upload your degree audit and we'll analyze your academic progress instantly.",
  },
  {
    step: "2",
    title: "Ask Anything",
    description:
      "Ask about courses, prerequisites, graduation requirements, or academic policies.",
  },
  {
    step: "3",
    title: "Get Personalized Advice",
    description:
      "Receive tailored recommendations based on your specific academic situation.",
  },
];

const Navbar = () => (
  <nav className="liquid-glass fixed top-0 z-50 flex w-full items-center justify-between px-6 lg:px-12 py-5">
    <div className="flex items-center gap-3">
      <img src={brain} alt="AdvisorAI" className="h-10" />
      <span className="font-instrument text-2xl text-white">AdvisorAI</span>
    </div>

    <div className="hidden md:flex items-center gap-8">
      <a
        href="#home"
        className="text-sm text-gray-400 transition-colors hover:text-white"
      >
        Home
      </a>
      <a
        href="#features"
        className="text-sm text-gray-400 transition-colors hover:text-white"
      >
        Features
      </a>
      <a
        href="#resources"
        className="text-sm text-gray-400 transition-colors hover:text-white"
      >
        Resources
      </a>
    </div>

    <Link
      to="/chat"
      className="rounded-full bg-app-gold px-6 py-2 text-sm font-semibold text-white hover:brightness-110 liquid-glass liquid-glass-glow"
    >
      Get Started
    </Link>
  </nav>
);

const HeroSection = () => {
  const iconRef = useRef<HTMLDivElement>(null);
  const [tilt, setTilt] = useState({ rotateX: 0, rotateY: 0 });

  const handleMouseMove = (e: React.MouseEvent<HTMLElement>) => {
    if (!iconRef.current) return;
    const rect = iconRef.current.getBoundingClientRect();
    const centerX = rect.left + rect.width / 2;
    const centerY = rect.top + rect.height / 2;
    const maxTilt = 20;
    const rotateY = ((e.clientX - centerX) / (window.innerWidth / 2)) * maxTilt;
    const rotateX = -((e.clientY - centerY) / (window.innerHeight / 2)) * maxTilt;
    setTilt({ rotateX, rotateY });
  };

  const handleMouseLeave = () => {
    setTilt({ rotateX: 0, rotateY: 0 });
  };

  return (
    <section
      id="home"
      className="relative flex h-screen flex-col items-center justify-center overflow-hidden px-6"
      style={{ background: "radial-gradient(ellipse 60% 55% at 50% 45%, #00416a 0%, #002a45 20%, #0f1f35 40%, #050a10 65%, #000000 85%)" }}
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

      {/* Robot hand — left side */}
      <img
        src={robotHand}
        alt="Robot hand"
        className="pointer-events-none absolute left-[-3%] bottom-[28%] md:bottom-auto md:top-[38%] lg:top-[30%] h-[28vw] md:h-[35vw] lg:h-[30vw] max-h-[560px] object-contain"
      />

      {/* Human hand — right side, bottom corner */}
      <img
        src={humanHand}
        alt="Human hand"
        className="pointer-events-none absolute right-[-3%] bottom-[18%] md:bottom-0 h-[22vw] md:h-[28vw] lg:h-[25vw] max-h-[400px] object-contain"
      />
    </section>
  );
};

const FeaturesSection = () => (
  <section
    id="features"
    className="px-6 lg:px-12 py-16 lg:py-24"
    style={{ background: "radial-gradient(ellipse 60% 55% at 50% 50%, #00416a 0%, #002a45 20%, #0f1f35 40%, #050a10 65%, #000000 85%)" }}
  >
    <h2 className="font-instrument text-center text-3xl lg:text-5xl text-white">
      How It Works
    </h2>
    <p className="mt-4 text-center text-gray-400">
      Three simple steps to better advising
    </p>

    <div className="mx-auto mt-12 lg:mt-16 flex flex-col md:flex-row max-w-5xl gap-6 lg:gap-8">
      {FEATURES.map((feature) => (
        <div
          key={feature.step}
          className="flex-1 rounded-2xl p-6 lg:p-8 liquid-glass liquid-glass-glow"
        >
          <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-xl bg-app-gold/10">
            <span className="text-xl font-bold text-app-gold">
              {feature.step}
            </span>
          </div>
          <h3 className="text-xl font-semibold text-white">{feature.title}</h3>
          <p className="mt-3 text-sm leading-relaxed text-gray-400">
            {feature.description}
          </p>
        </div>
      ))}
    </div>
  </section>
);

const ResourcesSection = () => (
  <section
    id="resources"
    className="px-6 lg:px-12 py-16 lg:py-24"
    style={{ background: "radial-gradient(ellipse 60% 55% at 50% 50%, #00416a 0%, #002a45 20%, #0f1f35 40%, #050a10 65%, #000000 85%)" }}
  >
    <h2 className="font-instrument text-center text-3xl lg:text-5xl text-white">
      FIU Resources
    </h2>
    <p className="mt-4 text-center text-gray-400">
      Quick access to helpful FIU advising resources
    </p>

    <div className="mx-auto mt-12 grid max-w-3xl grid-cols-1 md:grid-cols-2 gap-4">
      {RESOURCE_LINKS.map((resource) => (
        <a
          key={resource.url}
          href={resource.url}
          target="_blank"
          rel="noopener noreferrer"
          className="flex items-center gap-3 rounded-xl px-6 py-4 transition hover:border-app-gold/30 liquid-glass"
        >
          <img
            src={externalLinkArrow}
            alt=""
            className="h-3 brightness-0 invert"
          />
          <span className="text-sm text-gray-300">{resource.text}</span>
        </a>
      ))}
    </div>
  </section>
);

const Footer = () => (
  <footer className="flex items-center justify-between bg-black px-6 lg:px-12 py-8">
    <div className="flex items-center gap-3">
      <img src={brain} alt="" className="h-8" />
      <span className="font-instrument text-xl text-white">AdvisorAI</span>
    </div>
    <img src={fiuLogo} alt="FIU Logo" className="h-12 lg:h-16" />
  </footer>
);

const HomePage = () => {
  return (
    <div className="min-h-screen scroll-smooth bg-black">
      <Navbar />
      <HeroSection />
      <FeaturesSection />
      <ResourcesSection />
      <Footer />
    </div>
  );
};

export default HomePage;
