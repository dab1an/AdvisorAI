import externalLinkArrow from "../../assets/external_link_arrow.png";
import { RESOURCE_LINKS } from "./constants";

const ResourcesSection = () => (
  <section
    id="resources"
    className="px-6 lg:px-12 py-16 lg:py-24"
    style={{
      background:
        "radial-gradient(ellipse 60% 55% at 50% 50%, #00416a 0%, #002a45 20%, #0f1f35 40%, #050a10 65%, #000000 85%)",
    }}
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

export default ResourcesSection;
