using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtentionMethod
{
    public static int FindIndexEx<T>(this T[] array, T find_data, int start_index = 0, int count = -1)
    {
        if (array.Empty())
        {
            return -1;
        }

        count = -1 == count ? array.Length : count;

        return Array.FindIndex(array, start_index, count, x => x.Equals(find_data));
    }
    
    public static T FindExDelegate<T>(this T[] array, Predicate<T> find_predicate)
    {
        if (array.Empty())
        {
            return default(T);
        }

        return Array.Find(array, find_predicate);
    }

    public static bool Empty<T>(this T[] array)
    {
        return 0 == (array?.Length ?? 0);
    }
}
