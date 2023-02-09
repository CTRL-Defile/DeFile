using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util  {

    public static GameObject FindChildObject(GameObject go, string name)
    {
        foreach(Transform tr in go.transform)
        {
            if(tr.name.Equals(name))
            {
                return tr.gameObject;
            }
            else
            {
                GameObject find = FindChildObject(tr.gameObject, name);
                if(find != null)
                {
                    return find;
                }
            }
        }
        return null;
    }

    public static bool isFloatEqual(float a, float b)
    {
        if(a>=b - Mathf.Epsilon && a <= b + Mathf.Epsilon)
        return true;
        else
        return false;
    }

    public static T ToEnum<T>(string str)
    {
        System.Array A = System.Enum.GetValues(typeof(T)); //이넘의 타입을 배열의형태로 반환해줌
        foreach(T t in A)
        {
            if (t.ToString() == str)
                return t; // str이 해당Enum값에 있는거라서 해당Enum반환
        }
        return default(T);
    }

    public static int Rand(int min, int max)
    {
        return Random.Range(min, max + 1);
    }

    public static float Rand(float min, float max)
    {
        return Random.Range(min, max);
    }

    public static int GetPriority(int[] priorities)
    {
        int sum = 0;
        for (int i = 0; i < priorities.Length; ++i)
        {
            sum += priorities[i];
        }

        if (sum <= 0)
            return 0;

        int num = Rand(1, sum);

        sum = 0;
        for (int i = 0; i < priorities.Length; ++i)
        {
            int start = sum;
            sum += priorities[i];
            if (start < num && num <= sum)
            {
                return i;
            }
        }

        return 0;
    }
}
