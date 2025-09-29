# ToolGet
**Cross-platform NuGet Package Search Tool with Desktop and Console UI**

ToolGet is an Avalonia-based application that provides both desktop and console interfaces for searching and installing NuGet packages. Built with .NET 9 and featuring dual UI targets for maximum accessibility.

![ToolGet Console Demo](https://github.com/user-attachments/assets/7644dee8-1696-43d8-a771-a111467e507e)

## Features

✨ **Dual Interface Support**
- **Desktop App**: Modern Avalonia desktop application with rich UI
- **Console App**: Terminal-based interface using Consolonia for accessibility

🔍 **Package Search**
- Real-time NuGet package search using official NuGet API
- Search by package name, keywords, or authors
- Display comprehensive package information including:
  - Package title, description, and version
  - Author information and download statistics
  - Tags and prerelease indicators
  - Project URLs and icons

📦 **Package Cards**
- Card-based layout for easy package browsing
- Install buttons on each package
- Responsive design that works on both desktop and console

⚙️ **Installation Support**
- Simulated package installation (ready for real `dotnet tool install` integration)
- Version-specific installation
- Installation status feedback

## Architecture

The solution is structured with clean separation of concerns:

```
├── src/
│   ├── ToolGet.Core/           # Shared business logic
│   │   ├── Models/             # NuGet API models
│   │   ├── Services/           # NuGet API service
│   │   └── ViewModels/         # MVVM ViewModels
│   ├── ToolGet.Desktop/        # Avalonia desktop app
│   │   └── Views/              # Desktop UI views
│   └── ToolGet.Console/        # Consolonia console app
│       └── Views/              # Console-optimized UI views
```

### Technologies Used

- **.NET 9**: Latest .NET runtime
- **Avalonia UI 11.2**: Cross-platform UI framework
- **Consolonia**: Console-based Avalonia renderer
- **CommunityToolkit.Mvvm**: MVVM implementation
- **System.Text.Json**: JSON serialization for NuGet API
- **Microsoft.Extensions**: Dependency injection and HTTP client

## Getting Started

### Prerequisites

- .NET 9 SDK
- Visual Studio 2022 or VS Code (optional)

### Building the Solution

```bash
git clone https://github.com/tomlm/ToolGet.git
cd ToolGet
dotnet build
```

### Running the Applications

**Desktop Application:**
```bash
dotnet run --project src/ToolGet.Desktop
```

**Console Application:**
```bash
dotnet run --project src/ToolGet.Console
```

### Testing the Core Functionality

You can test the NuGet API integration independently:

```bash
# Build and test the core library
dotnet build src/ToolGet.Core
```

## Usage

1. **Launch** either the desktop or console version
2. **Search** for packages by entering keywords in the search box
3. **Browse** results displayed as cards with package information
4. **Install** packages by clicking the install button on any card

The application will show package details including:
- Package name and description
- Current version and prerelease status
- Author information
- Download statistics
- Associated tags

## Development

### Project Structure

- **ToolGet.Core**: Contains shared models, services, and ViewModels
- **ToolGet.Desktop**: Avalonia desktop application with Windows/macOS/Linux support
- **ToolGet.Console**: Console application using Consolonia for terminal display

### Key Components

- **NuGetService**: Handles API calls to NuGet search endpoints
- **MainViewModel**: Manages search state and package collection
- **PackageCardViewModel**: Represents individual packages with install functionality
- **NuGet Models**: Strongly-typed models for NuGet API responses

### Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests as appropriate
5. Submit a pull request

## License

MIT License - see [LICENSE](LICENSE) file for details.

---

**Note**: This implementation includes simulated package installation. For production use, integrate with actual `dotnet tool install` or `dotnet add package` commands.
