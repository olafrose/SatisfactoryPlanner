using SatisfactoryPlanner.Core;
using SatisfactoryPlanner.Core.Services;

Console.WriteLine("=== Satisfactory Production Planner ===");
Console.WriteLine();

// Initialize the planner service
var planner = new SatisfactoryPlannerService();

try
{
    // Example: Plan production for 10 Reinforced Iron Plates per minute at Tier 0
    Console.WriteLine("Planning production for 10 Reinforced Iron Plates per minute...");
    
    var targetItems = new Dictionary<string, double>
    {
        ["reinforced_iron_plate"] = 10.0 // 10 per minute
    };

    var options = new ProductionGraphOptions
    {
        OptimizeFor = OptimizationTarget.Simplicity,
        AllowOverclocking = true,
        MaxOverclockPercentage = 200.0
    };

    var productionGraph = await planner.PlanProductionAsync(targetItems, gameTier: 0, options);
    
    Console.WriteLine($"Production Graph: {productionGraph.Name}");
    Console.WriteLine($"Total Nodes: {productionGraph.Nodes.Count}");
    Console.WriteLine($"Total Power Consumption: {productionGraph.TotalPowerConsumption:F1} MW");
    Console.WriteLine();

    // Display production nodes
    Console.WriteLine("Production Steps:");
    foreach (var node in productionGraph.Nodes.OrderBy(n => n.Recipe.Name))
    {
        Console.WriteLine($"  • {node.Recipe.Name}:");
        Console.WriteLine($"    - Machine: {node.MachineCount:F1}x {node.Machine.Name}");
        Console.WriteLine($"    - Clock Speed: {node.ClockSpeed * 100:F0}%");
        Console.WriteLine($"    - Production Rate: {node.ActualProductionRate:F1}/min");
        Console.WriteLine($"    - Power: {node.TotalPowerConsumption:F1} MW");
        Console.WriteLine();
    }

    // Display required resources
    Console.WriteLine("Required Raw Resources:");
    foreach (var resource in productionGraph.RequiredResources)
    {
        Console.WriteLine($"  • {resource.Item.Name}: {resource.QuantityPerMinute:F1}/min");
    }
    Console.WriteLine();

    // Analyze the production
    var analysis = planner.AnalyzeProduction(productionGraph);
    Console.WriteLine("Production Analysis:");
    Console.WriteLine($"  • Total Machines: {analysis.TotalMachines}");
    Console.WriteLine($"  • Efficiency Score: {analysis.EfficiencyScore:F2}");
    
    if (analysis.BottleneckNodes.Any())
    {
        Console.WriteLine($"  • Bottlenecks: {string.Join(", ", analysis.BottleneckNodes.Select(b => b.Recipe.Name))}");
    }
    
    if (analysis.Suggestions.Any())
    {
        Console.WriteLine("  • Suggestions:");
        foreach (var suggestion in analysis.Suggestions)
        {
            Console.WriteLine($"    - {suggestion}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
