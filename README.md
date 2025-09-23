# Satisfactory Planner

A production line planning application for the PC game Satisfactory, featuring both console and graphical user interfaces.

## Project Structure

- **SatisfactoryPlanner.Core** - Core business logic and data models
- **SatisfactoryPlanner.App** - Console application interface
- **SatisfactoryPlanner.Avalonia** - Cross-platform GUI using Avalonia UI with MVVM pattern
- **SatisfactoryPlanner.Tests** - Unit tests

## Technologies

- .NET 9.0
- Avalonia UI (Cross-platform UI framework)
- MVVM Pattern
- MSTest for unit testing

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code (optional)

### Building the Solution

```powershell
dotnet build
```

### Running the Applications

**Console App:**
```powershell
dotnet run --project SatisfactoryPlanner.App
```

**GUI App:**
```powershell
dotnet run --project SatisfactoryPlanner.Avalonia
```

### Running Tests

```powershell
dotnet test
```

## Features

- Production line planning and optimization
- Cross-platform GUI with Avalonia UI
- Extensible architecture with separate core library
- Comprehensive unit testing

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

[Add your license information here]