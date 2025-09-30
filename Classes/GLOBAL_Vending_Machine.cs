namespace Vending_Machine_DevOS;

public static class VendingMachine
{
    public static bool serviceMode { get; set; } = true;
    public static bool exit { get; set; } = false; 
    public static string password { get; set; } = string.Empty;
    public static int cups { get; set; }
    public static int serviceModeCtr { get; set; } = 0; 
    public static Dictionary<Product, int> productInventory { get; set; } = new Dictionary<Product, int>();

    public static void initialize()
    {
        serviceMode = true;
        password = "Admin";
        cups = 0;

        initializeDefaultProducts();
        initializeCurrency(); 
    }

    public static void initializeDefaultProducts()
    {
        Product Coke = new Product("Coke", 0.75);
        Product Pepsi = new Product("Pepsi", 0.75);
        Product RC = new Product("RC", 0.75);
        Product Jolt = new Product("Jolt", 0.75);
        Product Faygo = new Product("Faygo", 0.75);

        productInventory.Add(Coke, 0);
        productInventory.Add(Pepsi, 0);
        productInventory.Add(RC, 0);
        productInventory.Add(Jolt, 0);
        productInventory.Add(Faygo, 0); 
    }
    public static void initializeCurrency()
    {
        Currency Nickel = new Currency("Coin", 5);
        Currency Dime = new Currency("Coin", 10);
        Currency Quarter = new Currency("Coin", 25);

        // Bills stored as cents for consistency (100 cents = $1, 500 cents = $5)
        Currency DollarBill = new Currency("Bill", 100);  
        Currency FiveDollarBill = new Currency("Bill", 500);  

        CurrencyHolder.Add(Nickel, 0);
        CurrencyHolder.Add(Dime, 0);
        CurrencyHolder.Add(Quarter, 0);
        CurrencyHolder.Add(DollarBill, 0);
        CurrencyHolder.Add(FiveDollarBill, 0); 
    }
    public static class CurrencyHolder
    {
        static readonly Dictionary<Currency, int> inventory = new Dictionary<Currency, int>();

        public static void Add(Currency currency, int quantity)
        {
            if (inventory.ContainsKey(currency))
            {
                inventory[currency] += quantity;
            }
            else
            {
                inventory[currency] = quantity;
            }
        }
        public static void Remove(Currency currency, int quantity)
        {
            if (inventory.ContainsKey(currency) && (inventory[currency] - quantity >= 0))
            {
                inventory[currency] -= quantity;
            }
            else
            {
                Console.WriteLine("Cannot remove more quantity bills than there are in the machine");
            }
        }
        public static int getCurrencyAmount(Currency currency)
        {
            if (inventory.ContainsKey(currency))
            {
                return inventory[currency];
            }
            else
            {
                return 0;
            }
        }
        public static Dictionary<Currency, int> getInventory()
        {
            return inventory;
        }
    } 

}