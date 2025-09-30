using System.Diagnostics;

namespace Vending_Machine_DevOS;

public partial class Product
{
    public string name { get; set; }
    public double price { get; set; }
    public Product(string name, double price)
    {
        this.name = name;
        this.price = price;
    }
}