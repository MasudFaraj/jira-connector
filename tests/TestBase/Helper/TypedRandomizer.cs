namespace Neusta.UserService.TestBase.Helper;

using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;

/// <summary>
///     Typed version of the <see cref="Randomizer" />.
/// </summary>
/// <remarks>
///     Only supports enums, integers and the types defined in <see cref="typeGenerators" />.
/// </remarks>
internal class TypedRandomizer
{
    private const int MaxLength = 5;
    private const int NumericMinValue = 1;
    private const int NumericMaxValue = 100;

    private static readonly long minTicks = DateTime.MinValue.Ticks;
    private static readonly long maxTicks = DateTime.MaxValue.Ticks;

    private static readonly Randomizer randomizer = new();

    private readonly IDictionary<Type, Func<Randomizer, object>> typeGenerators =
        new Dictionary<Type, Func<Randomizer, object>>
        {
            { typeof(bool), x => x.NextBool() },
            { typeof(DateTime), x => new DateTime(x.NextLong(minTicks, maxTicks)) },
            { typeof(decimal), x => x.NextDecimal(NumericMinValue, NumericMaxValue) },
            { typeof(double), x => x.NextDouble(NumericMinValue, NumericMaxValue) },
            { typeof(Guid), x => x.NextGuid() },
            { typeof(int), x => x.Next(NumericMinValue, NumericMaxValue) },
            { typeof(long), x => x.NextLong(NumericMinValue, NumericMaxValue) },
            { typeof(string), x => x.GetString(MaxLength) }
        };

    public object NextValue(Type type)
    {
        object nextValue;

        if (this.typeGenerators.TryGetValue(type, out Func<Randomizer, object> typeGenerator))
        {
            nextValue = typeGenerator(randomizer);
        }
        else if (type.IsEnum)
        {
            nextValue = randomizer.NextEnum(type);
        }
        else
        {
            throw new NotSupportedException($"No randomizer logic for type {type.Name} is configured.");
        }

        return nextValue;
    }
}