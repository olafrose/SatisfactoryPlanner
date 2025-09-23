namespace SatisfactoryPlanner.Core.Models;

/// <summary>
/// Represents a production machine in Satisfactory
/// </summary>
public class Machine
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MachineType Type { get; set; }
    
    /// <summary>
    /// Base production speed multiplier (1.0 = normal speed)
    /// </summary>
    public double ProductionSpeed { get; set; } = 1.0;
    
    /// <summary>
    /// Power consumption in MW
    /// </summary>
    public double PowerConsumption { get; set; }
    
    /// <summary>
    /// Game tier when this machine becomes available
    /// </summary>
    public int UnlockTier { get; set; }
    
    /// <summary>
    /// Maximum number of input conveyor/pipe connections
    /// </summary>
    public int MaxInputConnections { get; set; } = 1;
    
    /// <summary>
    /// Maximum number of output conveyor/pipe connections
    /// </summary>
    public int MaxOutputConnections { get; set; } = 1;
    
    /// <summary>
    /// Whether this machine can be overclocked
    /// </summary>
    public bool CanOverclock { get; set; } = true;

    public override string ToString() => Name;
    public override bool Equals(object? obj) => obj is Machine machine && Id == machine.Id;
    public override int GetHashCode() => Id.GetHashCode();
}

/// <summary>
/// Types of production machines in Satisfactory
/// </summary>
public enum MachineType
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
    Other            // Special or unique machines
}