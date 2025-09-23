#!/usr/bin/env python3
"""
Script to validate GameData.json for missing references
"""
import json
import sys
from pathlib import Path

def load_gamedata():
    """Load the GameData.json file"""
    gamedata_path = Path("SatisfactoryPlanner.Core/Data/GameData.json")
    with open(gamedata_path, 'r') as f:
        return json.load(f)

def validate_references(data):
    """Validate all references in the GameData.json"""
    issues = []
    
    # Extract all available IDs
    item_ids = {item["id"] for item in data["items"]}
    recipe_ids = {recipe["id"] for recipe in data["recipes"]}
    machine_ids = {machine["id"] for machine in data["machines"]}
    milestone_ids = {milestone["id"] for milestone in data["milestones"]}
    
    print(f"Found {len(item_ids)} items, {len(recipe_ids)} recipes, {len(machine_ids)} machines, {len(milestone_ids)} milestones")
    
    # Validate recipe references
    for recipe in data["recipes"]:
        recipe_id = recipe["id"]
        
        # Check input items
        for input_item in recipe["inputs"]:
            item_id = input_item["itemId"]
            if item_id not in item_ids:
                issues.append(f"Recipe '{recipe_id}' references unknown input item: '{item_id}'")
        
        # Check output items
        for output_item in recipe["outputs"]:
            item_id = output_item["itemId"]
            if item_id not in item_ids:
                issues.append(f"Recipe '{recipe_id}' references unknown output item: '{item_id}'")
        
        # Check compatible machines
        for machine_id in recipe["compatibleMachineIds"]:
            if machine_id not in machine_ids:
                issues.append(f"Recipe '{recipe_id}' references unknown machine: '{machine_id}'")
    
    # Validate milestone references
    for milestone in data["milestones"]:
        milestone_id = milestone["id"]
        
        # Check unlocked recipes
        for recipe_id in milestone["unlockedRecipeIds"]:
            if recipe_id not in recipe_ids:
                issues.append(f"Milestone '{milestone_id}' references unknown recipe: '{recipe_id}'")
        
        # Check unlocked machines
        for machine_id in milestone["unlockedMachineIds"]:
            if machine_id not in machine_ids:
                issues.append(f"Milestone '{milestone_id}' references unknown machine: '{machine_id}'")
        
        # Check prerequisite milestones
        for prereq_id in milestone["prerequisiteMilestoneIds"]:
            if prereq_id not in milestone_ids:
                issues.append(f"Milestone '{milestone_id}' references unknown prerequisite milestone: '{prereq_id}'")
        
        # Check cost items
        for cost_item in milestone["cost"]:
            item_id = cost_item["itemId"]
            if item_id not in item_ids:
                issues.append(f"Milestone '{milestone_id}' references unknown cost item: '{item_id}'")
    
    return issues

def main():
    """Main validation function"""
    try:
        data = load_gamedata()
        issues = validate_references(data)
        
        if not issues:
            print("✅ No validation issues found! All references are valid.")
        else:
            print(f"❌ Found {len(issues)} validation issues:")
            for issue in issues:
                print(f"  • {issue}")
            sys.exit(1)
            
    except FileNotFoundError:
        print("❌ GameData.json file not found")
        sys.exit(1)
    except json.JSONDecodeError as e:
        print(f"❌ Invalid JSON in GameData.json: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()