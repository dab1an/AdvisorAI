import { FEATURES } from "./constants";

const FeaturesSection = () => (
  <section
    id="features"
    className="px-6 lg:px-12 py-16 lg:py-24"
    style={{
      background:
        "radial-gradient(ellipse 60% 55% at 50% 50%, #00416a 0%, #002a45 20%, #0f1f35 40%, #050a10 65%, #000000 85%)",
    }}
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

export default FeaturesSection;
