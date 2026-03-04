import React from "react";
import SidebarLink from "./SidebarLink";

interface SidebarSectionProps {
  title: string;
  links: {
    link: string;
    text: string;
  }[];
}

const SidebarSection: React.FC<SidebarSectionProps> = ({ title, links }) => {
  return (
    <div className="flex w-full flex-col gap-3">
      <h2 className="text-app-yellow text-md font-bold underline">{title}</h2>
      {links.map((item, index) => (
        <SidebarLink key={index} link={item.link} text={item.text} />
      ))}
    </div>
  );
};

export default SidebarSection;
