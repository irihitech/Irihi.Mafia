namespace Irihi.Mirana.Demo.Models;

public record PhoneResolution(string Name, int Width, int Height)
{
    public override string ToString() => $"{Name} ({Width}x{Height})";
}
