namespace TuringLikePatterns;

internal readonly struct Quantity
{
    private Quantity(string name, SKColor color)
    {
        Name = name;
        Color = color;
    }

    public string Name { get; init; }
    public SKColor Color { get; init; }

    internal static Quantity Water { get; } = new(nameof(Water), SKColors.Aqua);
    internal static Quantity Hydrogen { get; } = new(nameof(Hydrogen), SKColors.Blue);
    internal static Quantity Oxygen { get; } = new(nameof(Oxygen), SKColors.White);
}
