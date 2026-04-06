import sidebar_bg from "../../../assets/sidebar_bg.png";
import brain from "../../../assets/Cap2.png";
import fiu_logo from "../../../assets/fiu_logo_hrz.png";

import SidebarSection from "./SidebarSection";
import NewChatButton from "./NewChatButton";

interface SidebarProps {
  isOpen: boolean;
  onClose: () => void;
  onNewChat: () => void;
}

const Sidebar = ({ isOpen, onClose, onNewChat }: SidebarProps) => {
  return (
    <>
      {/* Mobile backdrop */}
      {isOpen && (
        <div
          className="fixed inset-0 z-30 bg-black/40 md:hidden"
          onClick={onClose}
        />
      )}

      <div
        className={`fixed inset-y-0 left-0 z-40 flex h-full w-72 flex-col items-center justify-between bg-cover bg-bottom-right bg-no-repeat px-6 py-8 transition-transform duration-300
          md:relative md:translate-x-0 md:w-80
          ${isOpen ? "translate-x-0" : "-translate-x-full"}`}
        style={{ backgroundImage: `url(${sidebar_bg})` }}
      >
      <div className="flex w-full flex-col items-center gap-8 hover:cursor-default">
        <span className="flex w-full items-center justify-center gap-3">
          <img src={brain} alt="AdvisorAI Brain Icon" className="h-10" />
          <h1 className="font-instrument text-4xl text-white">AdvisorAI</h1>
        </span>

        <SidebarSection
          title="Helpful Resources"
          links={[
            {
              link: "https://career.fiu.edu/",
              text: "Career & Talent Development",
            },
            {
              link: "https://onestop.fiu.edu/academic-calendar/",
              text: "FIU Academic Calendar",
            },
            {
              link: "https://case.fiu.edu/advising/#:~:text=Drop%2DIn%20Advising%20%2D%20Starting%20March%2013th",
              text: "CASE Drop-In Advising",
            },
            {
              link: "https://www.cis.fiu.edu/academics/advising/undergraduate/",
              text: "Undergraduate Advising",
            },
            {
              link: "https://cec.fiu.edu/students/academic-advising/graduate-advising/",
              text: "Graduate Advising",
            },
          ]}
        />

        <SidebarSection
          title="Help us Improve!"
          links={[
            { link: "https://google.com", text: "Make a Suggestion" },
            { link: "https://google.com", text: "Report a Problem" },
            { link: "https://google.com", text: "Leave a Review" },
          ]}
        />

        <NewChatButton onClick={onNewChat} />
      </div>

      <img src={fiu_logo} alt="FIU Logo" className="h-24" />
    </div>
    </>
  );
};

export default Sidebar;
