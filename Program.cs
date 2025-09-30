namespace Vending_Machine_DevOS;

class Program
{
    public static void Main()
    {
        try
        {
            VendingMachine.initialize();
            Console.WriteLine("Please Enter a command and its parameter \n(Type Help for list of commands, EXIT to quit)");
            do
            {
                if (VendingMachine.serviceMode)
                {
                    Console.Write("\n[SERVICE MODE]> ");
                    string? input = Console.ReadLine();
                    VendingMachine.serviceModeCtr += 1; 
                    ServiceMode.ServiceModeSwitcher(input);
                }
                else if (!VendingMachine.serviceMode)
                {
                    Console.Write("\n[NORMAL MODE]> ");
                    string? input = Console.ReadLine();
                    CustomerMode.CustomerModeSwitcher(input);
                }
            } while (!VendingMachine.exit);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString()); 
        }
    }
}