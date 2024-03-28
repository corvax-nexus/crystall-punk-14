
using Content.Shared.Parallax.Biomes;
using Robust.Shared.Prototypes;

namespace Content.Server.Corvax.CrystallPunk.SpawnMapBiome;

/// <summary>
/// allows you to initialize a planet on a specific map at initialization time.
/// </summary>

[RegisterComponent, Access(typeof(SpawnMapBiomeSystem))]
public sealed partial class SpawnMapBiomeComponent : Component
{
    [DataField]
    public ProtoId<BiomeTemplatePrototype> Biome = "Grasslands";
}
