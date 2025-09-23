# Game Data Format

This directory contains the game data files for the Satisfactory Production Planner.

## GameData.json

The main data file containing items and recipes in JSON format.

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

### Adding New Data

1. **Items**: Add to the `items` array with unique `id`
2. **Recipes**: Add to the `recipes` array, ensuring:
   - All referenced `itemId` values exist in the items array
   - `compatibleMachineIds` reference valid machine IDs
   - `unlockTier` reflects the game progression

### Validation

Use the DataValidation tool to verify data integrity:
```bash
# Rename Program.cs and DataValidation.txt to swap them
dotnet run --project SatisfactoryPlanner.App
```

The validation tool will:
- Load all items and recipes
- Group by categories and tiers
- Test complex production chains
- Report any missing references or errors