namespace SatisfactoryPlanner.GameData.Models;

/// <summary>
/// Represents a production machine in Satisfactory
/// </summary>
[Obsolete("Use Building instead of Machine to align with wiki terminology")]
public class Machine : Building
{
    public new MachineType Type 
    { 
        get => (MachineType)base.Type; 
        set => base.Type = (BuildingType)value; 
    }
}

/// <summary>
/// Types of production machines in Satisfactory
/// </summary>
[Obsolete("Use BuildingType instead of MachineType to align with wiki terminology")]
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
    Workshop,        // Equipment Workshop
    Other            // Special or unique machines
}