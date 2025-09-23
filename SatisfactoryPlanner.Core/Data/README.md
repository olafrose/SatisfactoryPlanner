# Game Data Format

This directory contains the game data files for the Satisfactory Production Planner.

## GameData.json

The main data file containing items, recipes, milestones, and machines in JSON format.

### Structure

```json
{
  "items": [
    {
      "id": "unique_item_id",
      "name": "Display Name",
      "description": "Item description",
      "category": "ItemCategory",
      "isRawResource": false,
      "iconPath": "path/to/icon.png"
    }
  ],
  "recipes": [
    {
      "id": "unique_recipe_id",
      "name": "Recipe Display Name",
      "description": "Recipe description", 
      "inputs": [
        {
          "itemId": "required_item_id",
          "quantity": 1.0
        }
      ],
      "outputs": [
        {
          "itemId": "produced_item_id", 
          "quantity": 1.0
        }
      ],
      "productionTimeSeconds": 2.0,
      "compatibleMachineIds": ["machine_id"],
      "unlockTier": 0,
      "isAlternate": false
    }
  ],
  "milestones": [
    {
      "id": "unique_milestone_id",
      "name": "Milestone Display Name",
      "description": "Milestone description",
      "tier": 0,
      "isRequired": true,
      "prerequisiteMilestoneIds": ["prerequisite_milestone_id"],
      "unlockedRecipeIds": ["recipe_id1", "recipe_id2"],
      "unlockedMachineIds": ["machine_id1", "machine_id2"]
    }
  ],
  "machines": [
    {
      "id": "unique_machine_id",
      "name": "Machine Display Name",
      "description": "Machine description",
      "type": "MachineType",
      "productionSpeed": 1.0,
      "powerConsumption": 4.0,
      "unlockTier": 0,
      "maxInputConnections": 1,
      "maxOutputConnections": 1,
      "canOverclock": true
    }
  ]
}
```

### Item Categories

- `RawResource` - Raw materials like Iron Ore, Copper Ore
- `Ingot` - Smelted materials like Iron Ingot, Copper Ingot  
- `BasicPart` - Basic components like Iron Plate, Iron Rod
- `IntermediatePart` - Intermediate components like Reinforced Iron Plate
- `AdvancedPart` - Advanced components like Motors, Computers
- `Fuel` - Fuel items like Coal, Oil
- `Liquid` - Liquid items like Water, Oil, Fuel
- `SpaceElevator` - Space Elevator parts
- `MAM` - MAM research parts
- `Equipment` - Tools and equipment
- `Building` - Building components
- `Other` - Miscellaneous items

### Machine Types

- `Smelter` - Smelts ore into ingots (Smelter, Foundry)
- `Constructor` - Constructs basic parts from single inputs
- `Assembler` - Assembles parts from multiple inputs
- `Manufacturer` - Manufactures complex parts from multiple inputs
- `Refinery` - Refines liquids and gases
- `Extractor` - Extracts resources from nodes (Miners, Oil Extractors)

### Milestone System

Milestones represent the game's progression system where players unlock new recipes and machines by completing research tiers:

- **Required Milestones**: Must be completed to progress to the next tier
- **Optional Milestones**: Provide additional unlocks but are not mandatory
- **Prerequisites**: Some milestones require completing other milestones first
- **Unlocks**: Each milestone unlocks specific recipes and/or machines

### Adding New Data

1. **Items**: Add to the `items` array with unique `id`
2. **Recipes**: Add to the `recipes` array, ensuring:
   - All referenced `itemId` values exist in the items array
   - `compatibleMachineIds` reference valid machine IDs
   - `unlockTier` reflects the game progression
3. **Milestones**: Add to the `milestones` array, ensuring:
   - `prerequisiteMilestoneIds` reference valid milestone IDs
   - `unlockedRecipeIds` reference valid recipe IDs
   - `unlockedMachineIds` reference valid machine IDs
4. **Machines**: Add to the `machines` array with unique `id` and appropriate specifications

### Player Research State

The system supports tracking which milestones a player has completed and which alternate recipes they have researched. This affects:

- **Recipe Availability**: Only recipes from completed milestones are available
- **Machine Availability**: Only machines from completed milestones can be used
- **Alternate Recipes**: Players can selectively enable discovered alternate recipes
- **Production Optimization**: The planner considers only available recipes and machines

### Validation

Use the milestone demo to verify data integrity and test progression scenarios:
```bash
dotnet run --project SatisfactoryPlanner.App
```

The demo application will:
- Load all items, recipes, milestones, and machines
- Test production planning with different player research states
- Demonstrate milestone-based progression scenarios
- Show alternate recipe optimization
- Report any missing references or data integrity issues

### Data Dependencies

The data loading system follows this dependency chain:
1. **Items** are loaded first (no dependencies)
2. **Recipes** depend on items (via input/output itemIds) and machines (via compatibleMachineIds)
3. **Milestones** depend on recipes (via unlockedRecipeIds) and machines (via unlockedMachineIds)  
4. **Machines** are independent but referenced by recipes and milestones

All repositories use lazy loading and cache data in memory after first access for optimal performance.