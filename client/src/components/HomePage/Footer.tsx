import brain from "../../assets/Cap2.png";
import fiuLogo from "../../assets/fiu_logo_hrz.png";

const Footer = () => (
  <footer className="flex items-center justify-between bg-black px-6 lg:px-12 py-8">
    <div className="flex items-center gap-3">
      <img src={brain} alt="" className="h-8" />
      <span className="font-instrument text-xl text-white">AdvisorAI</span>
    </div>
    <img src={fiuLogo} alt="FIU Logo" className="h-12 lg:h-16" />
  </footer>
);

export default Footer;
