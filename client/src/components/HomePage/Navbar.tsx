import { Link } from "react-router-dom";
import brain from "../../assets/Cap2.png";

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

export default Navbar;
