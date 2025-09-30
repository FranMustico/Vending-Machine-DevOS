namespace Vending_Machine_DevOS;

public static partial class CustomerMode
{
    private static double insertedMoney = 0.0;
    private const double DRINK_PRICE = 0.75;

    public static void CustomerModeSwitcher(string? input)
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
            case "insert":
                Insert(input);
                break;
            case "buy":
                Buy(input);
                break;
            case "return":
                ReturnMoney();
                break;
            case "unlock":
                Unlock(input);
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
        Console.WriteLine("Commands in Normal Mode Are:\nHelp - Show this help message\nStatus - Show drink options and inserted money\nInsert [COIN|BILL] <denomination> - Insert money (coins: 5, 10, 25; bills: 1, 5)\nBuy <drink_name> - Purchase a drink (Coke, Pepsi, RC, Jolt, Faygo)\nReturn - Return all inserted money\nUnlock <password> - Enter service mode\nExit - Exit the program");
    }

    public static void Status()
    {
        var statusMessage = $"\nInserted Money: ${insertedMoney:F2}\nDrink Price: ${DRINK_PRICE:F2}\n\nAvailable Drinks:";
        
        foreach (var item in VendingMachine.productInventory)
        {
            if (item.Value > 0)
            {
                statusMessage += $"\n{item.Key.name} - ${item.Key.price:F2} (Stock: {item.Value})";
            }
        }
        
        statusMessage += $"\n\nCups Available: {VendingMachine.cups}";
        
        if (VendingMachine.cups == 0)
        {
            statusMessage += "\nWarning: No cups available. Machine cannot dispense drinks.";
        }
        
        Console.WriteLine(statusMessage);
    }

    public static void Insert(string? input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Error: Insert command requires parameters.");
                return;
            }

            var parts = input.Split(' ');
            if (parts.Length < 3)
            {
                Console.WriteLine("Error: Insert command format: Insert [COIN|BILL] <denomination>");
                return;
            }

            string type = parts[1].ToUpper();
            if (!int.TryParse(parts[2], out int userDenomination))
            {
                Console.WriteLine("Error: Denomination must be a valid number.");
                return;
            }

            // Currency system stores everything in cents internally, but users enter friendly values
            // Coins: user enters 5, 10, 25 (stored as-is in cents)  
            // Bills: user enters 1, 5 (converted to 100, 500 cents)
            string currencyType;
            int internalValue;
            
            if (type == "COIN")
            {
                currencyType = "Coin";
                internalValue = userDenomination;
                // Validate coin denominations
                if (internalValue != 5 && internalValue != 10 && internalValue != 25)
                {
                    Console.WriteLine("Error: Invalid coin denomination. Valid coins: 5, 10, 25");
                    return;
                }
            }
            else if (type == "BILL")
            {
                currencyType = "Bill";
                internalValue = userDenomination * 100; // Convert dollars to cents
                // Validate bill denominations (user enters 1 or 5, stored as 100 or 500)
                if (userDenomination != 1 && userDenomination != 5)
                {
                    Console.WriteLine("Error: Invalid bill denomination. Valid bills: 1, 5");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Error: Type must be COIN or BILL.");
                return;
            }

            // Find the matching currency in the machine inventory
            var currency = VendingMachine.CurrencyHolder.getInventory().Keys
                .FirstOrDefault(c => c.type == currencyType && c.value == internalValue);

            if (currency == null)
            {
                Console.WriteLine("Error: Currency system not properly initialized.");
                return;
            }

            // Add the money to machine inventory and track inserted amount
            VendingMachine.CurrencyHolder.Add(currency, 1);
            insertedMoney += internalValue / 100.0;
            
            string displayValue = type == "COIN" ? $"{userDenomination} cent" : $"${userDenomination:F2}";
            Console.WriteLine($"Inserted {displayValue}. Total inserted: ${insertedMoney:F2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Insert method: {ex.Message}");
        }
    }

    public static void Buy(string? input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Error: Buy command requires a drink name.");
                return;
            }

            var parts = input.Split(' ');
            if (parts.Length < 2)
            {
                Console.WriteLine("Error: Buy command format: Buy <drink_name>");
                return;
            }

            string drinkName = parts[1];

            // Check if enough money inserted
            if (insertedMoney < DRINK_PRICE)
            {
                Console.WriteLine($"Error: Insufficient funds. You need ${DRINK_PRICE - insertedMoney:F2} more.");
                return;
            }

            // Machine cannot dispense without cups - core business rule
            if (VendingMachine.cups <= 0)
            {
                Console.WriteLine("Error: No cups available. Cannot dispense drink.");
                return;
            }

            // Find the product using case-insensitive search
            var product = VendingMachine.productInventory.Keys
                .FirstOrDefault(p => p.name.Equals(drinkName, StringComparison.OrdinalIgnoreCase));

            if (product == null)
            {
                Console.WriteLine($"Error: Unknown drink '{drinkName}'. Available drinks: Coke, Pepsi, RC, Jolt, Faygo");
                return;
            }

            // Check if product is in stock
            if (VendingMachine.productInventory[product] <= 0)
            {
                Console.WriteLine($"Error: {product.name} is out of stock.");
                return;
            }

            // All checks passed - dispense the drink
            VendingMachine.productInventory[product]--;
            VendingMachine.cups--;

            // Calculate change and try to dispense it
            double change = insertedMoney - DRINK_PRICE;
            
            if (change > 0)
            {
                if (DispenseChange(change))
                {
                    Console.WriteLine($"Dispensed {product.name} and ${change:F2} change.");
                }
                else
                {
                    Console.WriteLine($"Dispensed {product.name}. Warning: Could not provide exact change of ${change:F2}.");
                }
            }
            else
            {
                Console.WriteLine($"Dispensed {product.name}. Thank you!");
            }

            insertedMoney = 0; // Reset inserted money
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Buy method: {ex.Message}");
        }
    }

    private static bool DispenseChange(double changeAmount)
    {
        int changeCents = (int)Math.Round(changeAmount * 100);
        var changeGiven = new Dictionary<Currency, int>();

        // Get available currency in descending order (largest denominations first)
        var currencies = VendingMachine.CurrencyHolder.getInventory()
            .Where(c => c.Value > 0)
            .OrderByDescending(c => c.Key.value)
            .ToList();

        foreach (var currencyPair in currencies)
        {
            var currency = currencyPair.Key;
            int available = currencyPair.Value;
            int needed = changeCents / currency.value;
            int toGive = Math.Min(needed, available);

            if (toGive > 0)
            {
                changeGiven[currency] = toGive;
                changeCents -= toGive * currency.value;
            }
        }

        // If we can't make exact change, return false
        if (changeCents > 0)
        {
            return false;
        }

        // Actually dispense the change by removing from inventory
        foreach (var change in changeGiven)
        {
            VendingMachine.CurrencyHolder.Remove(change.Key, change.Value);
            string unit = change.Key.type == "Coin" ? "cent" : "dollar";
            Console.WriteLine($"Returning {change.Value} {change.Key.value} {unit} {change.Key.type.ToLower()}(s)");
        }

        return true;
    }

    public static void ReturnMoney()
    {
        if (insertedMoney > 0)
        {
            if (DispenseChange(insertedMoney))
            {
                Console.WriteLine($"Returned ${insertedMoney:F2}");
            }
            else
            {
                Console.WriteLine($"Warning: Could not return exact change of ${insertedMoney:F2}");
            }
            insertedMoney = 0;
        }
        else
        {
            Console.WriteLine("No money to return.");
        }
    }

    public static void Unlock(string? input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Error: Unlock command requires a password.");
                return;
            }

            var parts = input.Split(' ');
            if (parts.Length < 2)
            {
                Console.WriteLine("Error: Unlock command format: Unlock <password>");
                return;
            }

            string enteredPassword = parts[1];

            // Password comparison is case-sensitive as per requirements
            if (enteredPassword == VendingMachine.password)
            {
                VendingMachine.serviceMode = true;
                insertedMoney = 0; // Clear any inserted money when entering service mode
                Console.WriteLine("Entered service mode.");
            }
            else
            {
                Console.WriteLine("Invalid password, try again");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Unlock method: {ex.Message}");
        }
    }

    public static void Exit()
    {
        VendingMachine.exit = true;
    }
}