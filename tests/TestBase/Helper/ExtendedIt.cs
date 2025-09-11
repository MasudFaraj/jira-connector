namespace Neusta.UserService.TestBase.Helper;

using System;
using System.Diagnostics;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

public static class ExtendedIt
{
    public static TValue Is<TValue>(Action<TValue> action)
    {
        Func<TValue, bool> actionWrapper = value =>
        {
            using (new TestExecutionContext.IsolatedContext())
            {
                try
                {
                    action(value);
                    return true;
                }
                catch (ResultStateException ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
            }
        };

        return It.Is<TValue>(x => actionWrapper(x));
    }
}