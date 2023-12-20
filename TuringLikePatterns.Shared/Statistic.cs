using System;

namespace TuringLikePatterns;

public sealed record class Statistic(string Name, Func<string> TextFunc);
