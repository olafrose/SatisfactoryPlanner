# 🏭 Satisfactory Planner

A comprehensive production planning application for the PC game Satisfactory, featuring intelligent optimization algorithms, milestone-based progression tracking, and wiki-aligned terminology.

## ✨ Key Features

- **🎯 Smart Production Planning** - Calculate optimal building counts and resource requirements
- **🔄 Alternate Recipe Optimization** - Choose between standard and efficiency-focused alternate recipes
- **📊 Milestone-Based Progression** - Plan production based on your current research tier
- **🖼️ Rich Icon Integration** - Visual item and building browser with official game icons
- **⚡ Advanced Optimization** - Multi-objective optimization (power efficiency, speed, simplicity)
- **🏗️ Wiki-Aligned Terminology** - Uses official Satisfactory wiki naming (Buildings instead of Machines)
- **💾 JSON Data System** - Flexible, updateable game data that stays current with game updates

## 🏗️ Project Architecture

- **SatisfactoryPlanner.Core** - Production planning algorithms, optimization logic, and domain models
- **SatisfactoryPlanner.GameData** - Game data loading, icon management, and wiki-aligned data models
- **SatisfactoryPlanner.App** - Feature-rich console application with production planning demos
- **SatisfactoryPlanner.Avalonia** - Cross-platform GUI (Avalonia UI with MVVM pattern) *[In Development]*
- **SatisfactoryPlanner.Tests** - Comprehensive unit test suite
- **SatisfactoryPlanner.Tools** - Data processing and development utilities

## 🚀 Technologies

- **.NET 9.0** - Latest .NET runtime for performance and cross-platform support
- **Avalonia UI** - Cross-platform UI framework (Windows, macOS, Linux)
- **MVVM Pattern** - Clean separation of UI and business logic  
- **MSTest** - Unit testing framework with full coverage
- **JSON Data System** - Flexible game data storage and icon management

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

### ▶️ Running the Applications

**🖥️ Console Application (Full Featured):**
```powershell
dotnet run --project SatisfactoryPlanner.App
```
*Features production planning demos, icon loading tests, milestone progression examples*

**🎨 GUI Application (In Development):**
```powershell
dotnet run --project SatisfactoryPlanner.Avalonia
```
*Cross-platform graphical interface - currently under development*

### 🧪 Running Tests

```powershell
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run specific test project
dotnet test SatisfactoryPlanner.Tests
```

## 📖 Example Usage

The console application demonstrates the full capabilities:

```powershell
dotnet run --project SatisfactoryPlanner.App
```

**Sample Output:**
```
=== Satisfactory Production Planner ===

Planning production for 10 Reinforced Iron Plates per minute...

Production Graph: Production for Reinforced Iron Plate
Total Nodes: 5
Total Power Consumption: 138.6 MW
Required Iron Ore: 150.0/min

  • Iron Ingot: 2.0x Smelter (STD)
  • Iron Plate: 2.0x Constructor (STD)  
  • Iron Rod: 1.0x Constructor (STD)
  • Reinforced Iron Plate: 1.0x Constructor (STD)
  • Screw: 2.0x Constructor (STD)
```

## 🎯 Current Status

### ✅ **Completed Features**
- **Production Planning Engine** - Full optimization algorithms with multiple objectives
- **Game Data System** - Complete JSON-based data loading with 400+ items, recipes, buildings
- **Icon Management** - Advanced icon loading system with caching and fallbacks
- **Milestone Integration** - Research progression tracking with 50+ milestones
- **Building System** - Wiki-aligned terminology (Machine → Building migration complete)
- **Console Interface** - Rich demonstration of all features
- **Comprehensive Testing** - Full test suite with 100% core functionality coverage

### 🔄 **In Development**
- **Avalonia GUI** - Cross-platform graphical interface (HIGH PRIORITY)
- **Save/Load System** - Production plan persistence
- **Advanced Optimization** - Factory layout and logistics planning

### 📋 **Planned Features**
- **Web Interface** - Browser-based planning tool
- **Save Game Integration** - Import player progression from game files
- **Community Features** - Share and discover production blueprints

## 🏗️ Architecture Overview

### Core Components

```
┌─────────────────────┐    ┌──────────────────────┐
│  SatisfactoryPlanner │    │ SatisfactoryPlanner  │
│       .Avalonia     │    │      .App            │
│   (GUI Interface)   │    │ (Console Interface)  │
└─────────┬───────────┘    └──────────┬───────────┘
          │                           │
          └─────────┬─────────────────┘
                    │
┌─────────────────────────────────────────────────────┐
│           SatisfactoryPlanner.Core                  │
│  • Production Planning & Optimization               │
│  • Building/Recipe Management                       │
│  • Milestone Progression System                     │
└─────────────────────┬───────────────────────────────┘
                      │
┌─────────────────────────────────────────────────────┐
│          SatisfactoryPlanner.GameData               │
│  • JSON Data Loading & Management                   │
│  • Icon Service & Image Processing                  │
│  • Wiki-Aligned Data Models                         │
└─────────────────────────────────────────────────────┘
```

### Key Services

- **`ProductionGraphBuilder`** - Builds optimized production chains
- **`SatisfactoryPlannerService`** - Main planning service with analysis
- **`IconService`** - Manages game icons with caching and fallbacks  
- **`GameDataService`** - Loads and manages all game data
- **`PlayerResearchState`** - Tracks milestone progression

## 🎮 Game Data Coverage

- **📦 400+ Items** - Raw materials, intermediate products, final products
- **🏭 50+ Buildings** - All production buildings with power and efficiency data  
- **📜 300+ Recipes** - Standard and alternate recipes with full optimization data
- **🎯 50+ Milestones** - Complete research progression system
- **🖼️ 1000+ Icons** - High-quality game icons for visual interfaces

## 🔧 Contributing

### Development Setup

1. **Fork the repository** on GitHub
2. **Clone your fork** locally
3. **Create a feature branch** (`git checkout -b feature/amazing-feature`)
4. **Make your changes** with appropriate tests
5. **Run the test suite** (`dotnet test`)
6. **Submit a pull request** with a clear description

### Code Style

- Follow standard C# conventions and .NET guidelines
- Add XML documentation for public APIs
- Include unit tests for new functionality
- Use meaningful commit messages

### Areas for Contribution

- **GUI Development** - Avalonia UI components and MVVM viewmodels
- **Optimization Algorithms** - Advanced production planning algorithms
- **Game Data Updates** - Keep data current with Satisfactory updates
- **Documentation** - User guides, API documentation, tutorials

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Coffee Stain Studios** - For creating the amazing game Satisfactory
- **Satisfactory Wiki Contributors** - For maintaining comprehensive game documentation  
- **Avalonia UI Team** - For the excellent cross-platform UI framework
- **Community Contributors** - For feedback, bug reports, and feature suggestions

---

⭐ **Star this repository if you find it helpful!**  
🐛 **Report bugs and request features** in the [Issues](https://github.com/yourusername/SatisfactoryPlanner/issues) section  
💬 **Join the discussion** about factory optimization strategies