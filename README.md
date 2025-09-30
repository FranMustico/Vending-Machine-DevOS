# Vending Machine DevOS

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20macOS-lightgrey.svg)](https://github.com/FranMustico/Vending-Machine-DevOS)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE.md)

A sophisticated command-line vending machine simulator that demonstrates operating system command line interpreter design principles. This project simulates a real pop (soda) vending machine with two distinct operational modes, complete inventory management, and intelligent change dispensing.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Installation](#installation)
- [Usage](#usage)
- [Documentation](#documentation)
- [Development](#development)
- [Contributing](#contributing)
- [License](#license)

## Overview

This vending machine simulator provides insight into what it takes to design an operating system command line interpreter. The software features two working modes that mirror real vending machine operations:

- **Service Mode**: For technicians to manage inventory (sodas, cups, coins, bills)
- **Normal Mode**: For customers to purchase drinks with money handling

The project demonstrates key software engineering concepts including state management, command pattern implementation, and real-world business logic enforcement.

## Features

- **Dual Operating Modes** with secure password-protected transitions
- **Smart Money Management** with exact change calculation using greedy algorithm
- **Complete Inventory Tracking** for products, cups, and currency
- **Security Features** including terminal clearing and case-sensitive passwords
- **Interactive CLI** with comprehensive help system and error handling
- **Business Logic Enforcement** (no cups = no dispensing)
- **Cross-platform Support** (Windows x64, macOS ARM64)

## Architecture

### Project Structure

```
Vending-Machine-DevOS/
├── Classes/
│   ├── Currency.cs                    # Currency denomination management
│   ├── Product.cs                     # Product definition and pricing
│   └── GLOBAL_Vending_Machine.cs      # Core state and inventory management
├── Runtimes/
│   ├── Service_Mode.cs                # Technician operations
│   └── Customer_Mode.cs               # Customer purchase operations
├── bin/Release/net9.0/publish/
│   ├── win-x64/                       # Windows executable
│   └── osx-arm64/                     # macOS executable
├── Program.cs                         # Application entry point
└── README.md
```

### Design Patterns

- **State Pattern**: Service Mode vs Normal Mode with distinct command sets
- **Command Pattern**: Each user input mapped to specific operations
- **Strategy Pattern**: Different currency handling for coins vs bills
- **Singleton Pattern**: Global vending machine state management

### Currency System

The system implements a dual-representation approach:

- **User Interface**: Friendly values (coins: 5, 10, 25; bills: 1, 5)
- **Internal Storage**: Everything stored as cents (bills: 100, 500)
- **Change Algorithm**: Greedy algorithm for optimal change dispensing

## Installation

### Prerequisites

- .NET 9.0 Runtime
- Windows 10+ or macOS (ARM64)
- Terminal/Command Prompt access

### Quick Start

#### Using Pre-built Executables

**Windows x64:**
```bash
cd bin/Release/net9.0/publish/win-x64/
./Vending-Machine-DevOS.exe
```

**macOS ARM64:**
```bash
cd bin/Release/net9.0/publish/osx-arm64/
./Vending-Machine-DevOS
```

#### Building from Source

```bash
# Clone the repository
git clone https://github.com/FranMustico/Vending-Machine-DevOS.git
cd Vending-Machine-DevOS

# Build and run
dotnet build
dotnet run
```

## Usage

### Initial Setup

The vending machine starts in **Service Mode** with:
- Default password: `Admin`
- Empty inventory (0 products, 0 cups, 0 currency)
- All drink prices set to $0.75

### Service Mode Commands

| Command | Syntax | Description |
|---------|--------|-------------|
| `Help` | `Help` | Show command list |
| `Status` | `Status` | Display complete inventory |
| `Add` | `Add COLA <brand> <quantity>` | Add drink inventory |
| `Add` | `Add CUPS <quantity>` | Add cup inventory |
| `Add` | `Add COINS <denomination> <quantity>` | Add coins (5, 10, 25) |
| `Add` | `Add BILLS <denomination> <quantity>` | Add bills (1, 5) |
| `Remove` | `Remove COINS <denomination> <quantity>` | Remove coins |
| `Remove` | `Remove BILLS <denomination> <quantity>` | Remove bills |
| `Lock` | `Lock <password>` | Switch to Normal Mode |
| `Exit` | `Exit` | Quit application |

### Normal Mode Commands

| Command | Syntax | Description |
|---------|--------|-------------|
| `Help` | `Help` | Show command list |
| `Status` | `Status` | Show available drinks and money |
| `Insert` | `Insert COIN <denomination>` | Insert coins (5, 10, 25) |
| `Insert` | `Insert BILL <denomination>` | Insert bills (1, 5) |
| `Buy` | `Buy <drink_name>` | Purchase drink ($0.75) |
| `Return` | `Return` | Get inserted money back |
| `Unlock` | `Unlock <password>` | Return to Service Mode |
| `Exit` | `Exit` | Quit application |

### Example Workflow

#### Service Mode Setup
```
[SERVICE MODE]> add cups 50
Added 50 cups. Total cups: 50

[SERVICE MODE]> add cola coke 10
Added 10 Coke. Total Coke: 10

[SERVICE MODE]> add bills 1 5
Added 5 $1 bill(s)

[SERVICE MODE]> lock mypassword
Service mode locked. Switched to normal mode.
```

#### Customer Purchase
```
[NORMAL MODE]> insert bill 1
Inserted $1.00. Total inserted: $1.00

[NORMAL MODE]> buy coke
Dispensed Coke and $0.25 change.
Returning 1 25 cent coin(s)
```

## Documentation

### Supported Currency

**Coins:** 5¢ (nickel), 10¢ (dime), 25¢ (quarter)  
**Bills:** $1, $5

### Available Products

- Coke ($0.75)
- Pepsi ($0.75)
- RC ($0.75)
- Jolt ($0.75)
- Faygo ($0.75)

### Security Features

- **Case-sensitive passwords** for mode switching
- **Commands are case-insensitive** for usability
- **Complete terminal clearing** when switching modes
- **Input validation** with comprehensive error handling

### Error Handling

The system provides detailed error messages for various scenarios:

```
# Insufficient funds
Error: Insufficient funds. You need $0.50 more.

# No cups available
Error: No cups available. Cannot dispense drink.

# Invalid password
Invalid password, try again
```

## Development

### Building for Different Platforms

```bash
# Windows x64
dotnet publish -c Release -r win-x64 --self-contained

# macOS ARM64
dotnet publish -c Release -r osx-arm64 --self-contained

# All platforms
dotnet publish -c Release
```

### Development Mode

```bash
# Hot reload during development
dotnet watch run
```

### Key Dependencies

- **.NET 9.0** - Core runtime
- **System.Linq** - LINQ operations for inventory management
- **System.Collections.Generic** - Dictionary-based inventory storage

### Testing

#### Manual Testing Scenarios

1. **Complete Purchase Flow**
   - Service Mode setup → Inventory addition → Mode lock → Money insertion → Purchase → Change verification

2. **Edge Cases**
   - Insufficient funds scenarios
   - Out of stock conditions
   - No cups available
   - Exact change requirements

3. **Security Validation**
   - Password protection testing
   - Case sensitivity verification
   - Terminal clearing confirmation

## Contributing

This project is part of an educational exercise demonstrating operating system command line interpreter design. 

### Reporting Issues

If you encounter any issues:

1. Check the existing documentation
2. Review code comments for complex logic explanations
3. Test with the provided examples
4. Create an issue with detailed reproduction steps

### Development Setup

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## Educational Value

This project demonstrates:

### Command Line Interface Design
- Stateful operation with mode switching
- Input parsing and validation
- Error handling and user feedback
- Help system design

### Software Architecture Patterns
- State management across operational modes
- Command pattern for user input handling
- Data modeling for inventory and currency
- Business logic separation

### Algorithm Implementation
- Greedy algorithms for change dispensing
- Search algorithms for inventory lookups
- Input validation algorithms

### Real-World Business Logic
- Inventory management systems
- Point-of-sale operations
- Cash handling procedures
- User access control

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details. 