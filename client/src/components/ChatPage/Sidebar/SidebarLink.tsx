import arrow_link from "../../../assets/external_link_arrow.png";

interface SidebarLinkProps {
  link: string;
  text: string;
}

const SidebarLink = ({ link, text }: SidebarLinkProps) => {
  return (
    <div>
      <a
        href={link}
        target="_blank"
        className="text-sm text-white hover:underline"
      >
        <span className="flex items-center gap-2">
          <img src={arrow_link} alt="" className="h-3" />
          {text}
        </span>
      </a>
    </div>
  );
};

export default SidebarLink;
