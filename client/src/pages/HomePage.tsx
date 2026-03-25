import FeaturesSection from "../components/HomePage/FeaturesSection";
import Footer from "../components/HomePage/Footer";
import HeroSection from "../components/HomePage/HeroSection";
import Navbar from "../components/HomePage/Navbar";
import ResourcesSection from "../components/HomePage/ResourcesSection";

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
