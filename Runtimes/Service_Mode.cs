using System.ComponentModel.Design;

namespace Vending_Machine_DevOS;

public static partial class ServiceMode
{
    public static void ServiceModeSwitcher(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("No command entered. Please try again, or type Help to view a complete list of commands");
            return;
        }
        string splitInput = input.Split(' ')[0].Trim().ToLower();
        // Commands are case-insensitive, but parameters (like passwords) are case-sensitive
        switch (splitInput)
        {
            case "help":
                Help();
                break;
            case "status":
                Status();
                break;
            case "add":
                Add(input);
                break;
            case "remove":
                Remove(input);
                break;
            case "lock":
                Lock(input);
                break;
            case "exit":
                Exit();
                break; 
            default:
                Console.WriteLine("The command you have entered does not exist. Please try again, or type Help to view a complete list of commands");
                break;
        }
    }
    public static void Help()
    {
        Console.WriteLine("Commands in Service Mode Are:\nStatus\nAdd [COLA|CUPS] <brand> <quantity>\nAdd | Remove [COINS|BILLS] <denomination> <quantity>\nLock <password>\nExit");
    }
    public static void Status()
    {
        var statusMessage = "\nCurrency Inventory:";
        foreach (var item in VendingMachine.CurrencyHolder.getInventory())
        {
            if (item.Key.type == "Coin")
                statusMessage += $"\nTotal {item.Key.value} Cent {item.Key.type}: {item.Value}";
            else if (item.Key.type == "Bill")
                statusMessage += $"\nTotal ${item.Key.value / 100.0:F2} {item.Key.type}: {item.Value}";
        }
        statusMessage += "\n\nCola Inventory:";
        foreach (var item in VendingMachine.productInventory)
        {
            statusMessage += $"\nTotal {item.Key.name} = {item.Value}";
        }
        statusMessage += $"\nCups: {VendingMachine.cups}";
        
        Console.WriteLine(statusMessage);
    }
    public static void Lock(string? password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Error: Lock command requires a password.");
                return;
            }

            var parts = password.Split(' ');
            if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
            {
                Console.WriteLine("Error: Lock command format: Lock <password>");
                return;
            }

            string enteredPassword = parts[1].Trim();
            // Store the password for future unlock attempts (case-sensitive)
            VendingMachine.password = enteredPassword;
            VendingMachine.serviceMode = false;

            // Clear terminal when switching to normal mode for security
            ClearTerminalCompletely();
            VendingMachine.serviceModeCtr = 0;            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Lock method: {ex.Message}");
        }
    }
    
    public static void Add(string? input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Error: Add command requires parameters.");
                return;
            }

            var parts = input.Split(' ');
            if (parts.Length < 3)
            {
                Console.WriteLine("Error: Add command format: Add [COLA|CUPS|COINS|BILLS] <brand/denomination> <quantity>");
                return;
            }

            string type = parts[1].ToUpper();
            string item = parts[2];
            
            if (!int.TryParse(parts[parts.Length - 1], out int quantity))
            {
                Console.WriteLine("Error: Quantity must be a valid number.");
                return;
            }

            switch (type)
            {
                case "COLA":
                    AddCola(item, quantity);
                    break;
                case "CUPS":
                    VendingMachine.cups += quantity;
                    Console.WriteLine($"Added {quantity} cups. Total cups: {VendingMachine.cups}");
                    break;
                case "COINS":
                    AddCurrency("Coin", item, quantity);
                    break;
                case "BILLS":
                    AddCurrency("Bill", item, quantity);
                    break;
                default:
                    Console.WriteLine("Error: Valid types are COLA, CUPS, COINS, BILLS");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Add method: {ex.Message}");
        }
    }

    public static void Remove(string? input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Error: Remove command requires parameters.");
                return;
            }

            var parts = input.Split(' ');
            if (parts.Length < 3)
            {
                Console.WriteLine("Error: Remove command format: Remove [COINS|BILLS] <denomination> <quantity>");
                return;
            }

            string type = parts[1].ToUpper();
            string denomination = parts[2];
            
            if (!int.TryParse(parts[parts.Length - 1], out int quantity))
            {
                Console.WriteLine("Error: Quantity must be a valid number.");
                return;
            }

            switch (type)
            {
                case "COINS":
                    RemoveCurrency("Coin", denomination, quantity);
                    break;
                case "BILLS":
                    RemoveCurrency("Bill", denomination, quantity);
                    break;
                default:
                    Console.WriteLine("Error: Remove command only supports COINS and BILLS");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Remove method: {ex.Message}");
        }
    }

    private static void AddCola(string brand, int quantity)
    {
        var product = VendingMachine.productInventory.Keys.FirstOrDefault(p => p.name.Equals(brand, StringComparison.OrdinalIgnoreCase));
        if (product != null)
        {
            VendingMachine.productInventory[product] += quantity;
            Console.WriteLine($"Added {quantity} {brand}. Total {brand}: {VendingMachine.productInventory[product]}");
        }
        else
        {
            Console.WriteLine($"Error: Unknown cola brand '{brand}'. Available brands: Coke, Pepsi, RC, Jolt, Faygo");
        }
    }

    private static void AddCurrency(string type, string denomination, int quantity)
    {
        if (!int.TryParse(denomination, out int userValue))
        {
            Console.WriteLine("Error: Denomination must be a valid number.");
            return;
        }

        // Convert user-friendly input to internal storage values
        // Same conversion logic as customer insert function
        int internalValue;
        if (type == "Coin")
        {
            internalValue = userValue; // Coins: user enters 5, 10, 25 (cents)
        }
        else // Bill
        {
            internalValue = userValue * 100; // Bills: user enters 1, 5 (dollars) -> convert to cents
        }

        var currency = VendingMachine.CurrencyHolder.getInventory().Keys.FirstOrDefault(c => c.type == type && c.value == internalValue);
        if (currency != null)
        {
            VendingMachine.CurrencyHolder.Add(currency, quantity);
            string displayValue = type == "Coin" ? $"{userValue} cent" : $"${userValue}";
            Console.WriteLine($"Added {quantity} {displayValue} {type.ToLower()}(s)");
        }
        else
        {
            Console.WriteLine($"Error: Invalid {type.ToLower()} denomination. Valid coins: 5, 10, 25. Valid bills: 1, 5");
        }
    }

    private static void RemoveCurrency(string type, string denomination, int quantity)
    {
        if (!int.TryParse(denomination, out int userValue))
        {
            Console.WriteLine("Error: Denomination must be a valid number.");
            return;
        }

        // Convert user input to internal currency values
        int internalValue;
        if (type == "Coin")
        {
            internalValue = userValue; // Coins: user enters 5, 10, 25 (cents)
        }
        else // Bill
        {
            internalValue = userValue * 100; // Bills: user enters 1, 5 (dollars) -> convert to cents
        }

        var currency = VendingMachine.CurrencyHolder.getInventory().Keys.FirstOrDefault(c => c.type == type && c.value == internalValue);
        if (currency != null)
        {
            int current = VendingMachine.CurrencyHolder.getCurrencyAmount(currency);
            if (current >= quantity)
            {
                VendingMachine.CurrencyHolder.Remove(currency, quantity);
                string displayValue = type == "Coin" ? $"{userValue} cent" : $"${userValue}";
                Console.WriteLine($"Removed {quantity} {displayValue} {type.ToLower()}(s)");
            }
            else
            {
                Console.WriteLine($"Error: Cannot remove {quantity} {type.ToLower()}(s). Only {current} available.");
            }
        }
        else
        {
            Console.WriteLine($"Error: Invalid {type.ToLower()} denomination. Valid coins: 5, 10, 25. Valid bills: 1, 5");
        }
    }

    public static void Exit()
    {
        VendingMachine.exit = true; 
    }

    private static void ClearTerminalCompletely()
    {
        // Multiple clearing techniques for maximum cross-platform effectiveness
        Console.Clear();

        // ANSI escape sequences for complete clearing
        // \x1b[2J clears entire screen, \x1b[3J clears scrollback, \x1b[H moves cursor to home
        Console.Write("\x1b[2J\x1b[3J\x1b[H");

        // Reset cursor to top-left corner and clear again for good measure
        Console.SetCursorPosition(0, 0);
        Console.Clear();
    }
}