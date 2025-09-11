namespace Neusta.UserService.TestBase.Attributes;

using System.Collections.Generic;
using System.Linq;
using Neusta.UserService.TestBase.Helper;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

/// <summary>
///     Test attribute to use random test parameters for types supported by <see cref="TypedRandomizer" />.
/// </summary>
public class RandomTestAttribute : TestCaseAttribute, ITestBuilder
{
    private readonly NUnitTestCaseBuilder builder = new();
    private readonly TypedRandomizer typedRandomizer;

    public RandomTestAttribute()
    {
        this.typedRandomizer = new TypedRandomizer();
    }

    IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test suite)
    {
        TestCaseParameters parameters = this.GetTestCaseParameters(method);
        parameters.TestName = method.Name;

        yield return this.builder.BuildTestMethod(method, suite, parameters);
    }

    private TestCaseParameters GetTestCaseParameters(IMethodInfo method)
    {
        object[] arguments = method.GetParameters()
            .Select(this.GetArgument)
            .ToArray();

        return new TestCaseParameters(arguments);
    }

    private object GetArgument(IParameterInfo parameterInfo)
    {
        return this.typedRandomizer.NextValue(parameterInfo.ParameterType);
    }
}