namespace SatisfactoryPlanner.GameData.Models;

/// <summary>
/// Represents a production building in Satisfactory
/// </summary>
public class Building
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BuildingType Type { get; set; }
    
    /// <summary>
    /// Base production speed multiplier (1.0 = normal speed)
    /// </summary>
    public double ProductionSpeed { get; set; } = 1.0;
    
    /// <summary>
    /// Power consumption in MW
    /// </summary>
    public double PowerConsumption { get; set; }
    
    /// <summary>
    /// Maximum number of input conveyor/pipe connections
    /// </summary>
    public int MaxInputConnections { get; set; } = 1;
    
    /// <summary>
    /// Maximum number of output conveyor/pipe connections
    /// </summary>
    public int MaxOutputConnections { get; set; } = 1;
    
    /// <summary>
    /// Whether this building can be overclocked
    /// </summary>
    public bool CanOverclock { get; set; } = true;

    public override string ToString() => Name;
    public override bool Equals(object? obj) => obj is Building building && Id == building.Id;
    public override int GetHashCode() => Id.GetHashCode();
}

/// <summary>
/// Types of production buildings in Satisfactory
/// </summary>
public enum BuildingType
{
    Extractor,       // Miners, Oil Extractors, Water Extractors
    Smelter,         // Smelter, Foundry
    Constructor,     // Constructor
    Assembler,       // Assembler
    Manufacturer,    // Manufacturer
    Refinery,        // Oil Refinery
    Packager,        // Packager
    Blender,         // Blender
    ParticleAccelerator, // Particle Accelerator
    Converter,       // Converter
    QuantumEncoder,  // Quantum Encoder
    Workshop,        // Equipment Workshop
    Other            // Special or unique buildings
}