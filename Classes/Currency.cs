using System.Text.Encodings.Web;

namespace Vending_Machine_DevOS;

public class Currency
{
    public string type { get; }
    public int value { get; }
    public Currency(string type, int value)
    {
        this.type = type;
        this.value = value;
    }
}