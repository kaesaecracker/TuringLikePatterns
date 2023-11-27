namespace TuringLikePatterns;

internal sealed record class Quantity
{
    internal static Quantity Water { get; } = new() { Name = nameof(Water), Color = SKColors.Aqua };
    internal static Quantity Hydrogen { get; } = new() { Name = nameof(Hydrogen), Color = SKColors.Blue };
    internal static Quantity Oxygen { get; } = new() { Name = nameof(Oxygen), Color = SKColors.White };

    public required string Name { get; init; }
    public required SKColor Color { get; init; }
}
