# Satisfactory Planner

A comprehensive production planning tool for the PC game Satisfactory, featuring an interactive visual interface for complex factory planning.

## Current Status - v0.5 Alpha ✨

**Major Milestone: Visual Production Planner Complete!**

**✅ What works:**
- **Interactive Visual Interface** - Full Avalonia UI with MVVM architecture
- **Flow Graph Visualization** - Drag-and-drop production nodes with dynamic connection lines
- **Multi-Target Production Planning** - Handle multiple production chains simultaneously
- **Real-time Calculations** - Live updates of building counts and production rates
- **Complete Game Data Integration** - 400+ items, 300+ recipes, 50+ buildings, 50+ milestones
- **Milestone Management** - Research progression tracking and unlock status
- **Advanced Production Algorithms** - Dependency resolution and resource optimization
- **Clean Modern UI** - Professional interface with debugging console (toggle-able)

**🚧 In Development:**
- Visual icons for buildings and items
- Output item nodes for clearer multi-target visualization
- Enhanced production chain analysis
- Save/load functionality for production plans

## Project Structure

- **SatisfactoryPlanner.Core** - Domain models and basic repository interfaces
- **SatisfactoryPlanner.GameData** - Game data loading, JSON parsing, icon system
- **SatisfactoryPlanner.App** - Console application (mainly for testing backend functionality)
- **SatisfactoryPlanner.Tests** - Unit tests
- **SatisfactoryPlanner.Tools** - Utilities for data scraping/processing

## Technology Stack

- **.NET 9.0** - Target framework
- **Avalonia UI** - Planned for cross-platform GUI (not implemented yet)
- **MSTest** - Unit testing
- **JSON** - Game data storage format

## 🛠️ Getting Started

### Prerequisites

- **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Visual Studio 2022** or **VS Code** (recommended for development)
- **Git** (for cloning the repository)

### 🏗️ Building the Solution

```powershell
# Clone the repository
git clone https://github.com/yourusername/SatisfactoryPlanner.git
cd SatisfactoryPlanner

# Build all projects
dotnet build

# Restore NuGet packages (if needed)
dotnet restore
```

### Running the Visual Planner (Primary Application)

```powershell
# Run the main Avalonia UI application
dotnet run --project SatisfactoryPlanner.Avalonia
```

**Features Available:**
- **Production Planning**: Add production targets and see the complete production chain
- **Visual Flow Graph**: Drag nodes around to arrange your factory layout
- **Milestone Manager**: Track research progression and building unlocks
- **Real-time Updates**: Connection lines follow nodes as you move them
- **Multi-chain Support**: Plan multiple production targets simultaneously

### Running the Console App (Development Tool)

```powershell
dotnet run --project SatisfactoryPlanner.App
```

The console app demonstrates backend functionality and is primarily used for testing core algorithms.

### 🧪 Running Tests

```powershell
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run specific test project
dotnet test SatisfactoryPlanner.Tests
```

## Development Notes

This project is in early development. The console application currently just loads game data and displays some basic information to verify the backend is working correctly.

### ✅ Recently Completed (December 2024)
- **Full Avalonia UI Implementation** - Modern cross-platform interface
- **Interactive Production Flow Graphs** - Visual drag-and-drop node system
- **Dynamic Connection Lines** - Real-time updates as nodes are moved
- **Multi-target Production Chains** - Support for complex factory planning
- **Advanced Production Algorithms** - Dependency resolution and flow optimization
- **Milestone Integration** - Research progression affects available recipes
- **MVVM Architecture** - Clean separation of concerns with data binding
- **Comprehensive Debug System** - Toggle-able debug console for development

### 🎯 Next Priorities
- **Visual Icons** - Building and item icons for enhanced clarity
- **Output Item Nodes** - Dedicated nodes for final products
- **Enhanced UI Polish** - Improved styling and user experience
- **Save/Load System** - Persist production plans between sessions
- **Factory Layout Tools** - Spatial planning and optimization features
## Architecture

The project follows a clean layered architecture with MVVM pattern:

- **SatisfactoryPlanner.Avalonia** - Main UI application with interactive visual planning
- **SatisfactoryPlanner.GameData** - JSON loading, game data models, and repositories
- **SatisfactoryPlanner.Core** - Domain logic, production algorithms, and business rules
- **SatisfactoryPlanner.App** - Console application for testing and debugging
- **SatisfactoryPlanner.Tests** - Comprehensive unit test coverage

### Key Components
- **ViewModels** - MVVM data binding with CommunityToolkit.Mvvm
- **Production Graph Builder** - Advanced dependency resolution algorithms
- **Flow Graph Positioning** - Automatic layout of production chains
- **Dynamic Connection System** - Real-time visual connections between nodes
- **Milestone System** - Research progression and recipe unlocking

## Game Data

The project includes data for:
- ~400 items (raw materials, products)
- ~300 recipes (standard and alternate)
- ~50 buildings (production facilities)
- ~50 milestones (research progression)

## License

MIT License - see LICENSE file for details.