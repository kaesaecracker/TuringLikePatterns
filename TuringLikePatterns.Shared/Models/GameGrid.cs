using System;
using System.Collections.Generic;

namespace TuringLikePatterns.Models;

public sealed class GameGrid<T> where T : struct
{
    private readonly Dictionary<GamePosition, T> _raw = new();

    public T this[GamePosition position]
    {
        get => _raw.GetValueOrDefault(position);
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _raw[position] = value;
        }
    }
}