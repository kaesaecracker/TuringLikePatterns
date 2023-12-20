using System;

namespace TuringLikePatterns.Shared;

public sealed record class Statistic(string Name, Func<string> TextFunc);
