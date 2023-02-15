using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumUtils
{
    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}
