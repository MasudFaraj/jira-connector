namespace Neusta.UserService.TestBase.Helper;

using System.Collections.Generic;

public static class TestDelegateHelper
{
    /// <summary>
    ///     Iterate over the enumerable to ensure iterator methods ran to completion.
    /// </summary>
    public static void Iterate<T>(this IEnumerable<T> enumerable)
    {
        using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
            }
        }
    }
}