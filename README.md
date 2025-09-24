# Satisfactory Planner

A production planning tool for the PC game Satisfactory. Currently in early development with backend infrastructure complete and GUI in progress.

## Current Status

This project implements the core data structures and algorithms for Satisfactory production planning. The console application is primarily used for testing and demonstrating the backend functionality.

**What works:**
- Game data loading (400+ items, 300+ recipes, 50+ buildings, 50+ milestones)
- Basic production graph calculation
- Recipe and building repository system
- Icon loading system (local files, not in repository)
- Machine → Building terminology migration (backward compatible)

**What doesn't work yet:**
- No GUI (Avalonia UI planned)
- No save/load functionality
- No optimization algorithms
- Limited production planning features

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

### Running the Console App

```powershell
dotnet run --project SatisfactoryPlanner.App
```

The console app demonstrates basic functionality like loading game data and showing available items/buildings. It's primarily a development tool for testing the backend.

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

### Completed
- Game data loading system
- Basic repository pattern for items, recipes, buildings, milestones
- Icon system (files kept local via .gitignore)
- Machine → Building terminology migration with backward compatibility
- Unit tests for core functionality

### TODO
- Avalonia UI implementation
- Actual production planning algorithms
- Save/load functionality
- Recipe optimization
- User interface design
## Architecture

The project follows a layered architecture:

- **GameData** layer handles JSON loading and game data models
- **Core** layer contains domain logic and repository interfaces  
- **App** layer is a simple console application for testing
- **Tests** verify the core functionality works

## Game Data

The project includes data for:
- ~400 items (raw materials, products)
- ~300 recipes (standard and alternate)
- ~50 buildings (production facilities)
- ~50 milestones (research progression)

## License

MIT License - see LICENSE file for details.