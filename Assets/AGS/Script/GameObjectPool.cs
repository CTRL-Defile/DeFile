using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class GameObjectPool<T> where T : class
{
    int count;
    public delegate T Func(int i);
    Func create_fn;
    // Instances.  
    [SerializeField]
    public SerialStack<T> objects;
    // Construct  
    public GameObjectPool(int count, Func fn)
    {
        this.count = count;
        this.create_fn = fn;
        this.objects = new SerialStack<T>(this.count);
        allocate();

    }
    void allocate()
    {
        for (int i = 0; i < this.count; ++i)
        {
            //this.objects.m_Stack.Push(this.create_fn());
            this.objects.PushStack(this.create_fn(i));
        }
    }
    public void allocate_onemore(T val)
    {
        count += 1;
        this.objects.PushStack(val);
    }
    public T pop()
    {
        if (this.objects.m_Stack.Count <= 0)
        {
            Debug.Log(this + " allocate more");
            allocate();
        }
        return this.objects.m_Stack.Pop();
    }
    public void push(T obj)
    {
        this.objects.PushStack(obj);
    }
    public int get_tot_count()
    {
        return this.count;
    }

    public int get_Count()
    {
        return objects.m_Stack.Count;
    }
    public Stack<T> get_Stack()
    {
        return objects.m_Stack;
    }

}


[System.Serializable]
public class SerialStack<T>
{
    public Stack<T> m_Stack;
    [SerializeField]
    public List<T> m_ShowStack;

    public SerialStack()
    {
        m_Stack = new Stack<T>();
        m_ShowStack = new List<T>();
    }
    public SerialStack(int cnt)
    {
        m_Stack = new Stack<T>(cnt);
        m_ShowStack = new List<T>(cnt);
    }

    public void ClearStack()
    {
        m_Stack.Clear();
        m_ShowStack.Clear();
    }
    
    public T PopStack()
    {
        if (m_Stack.Count > 0)
        {
            m_ShowStack.RemoveAt(m_Stack.Count - 1);

        }

        return m_Stack.Pop();
    }

    public void PushStack(T val)
    {
        m_Stack.Push(val);
        m_ShowStack.Add(val);
    }
}

[System.Serializable]
public class SerialDictionary<Tkey, Tvalue>
{
    [SerializeField]
    public List<Tkey> m_key;
    [SerializeField]
    public List<string> m_value;
    
    Dictionary<Tkey, Tvalue> m_dic;

    public SerialDictionary()
    {
        m_key = new List<Tkey>();
        m_value = new List<string>();
        m_dic = new Dictionary<Tkey, Tvalue>();
    }

    public void Dic_Copy(List<Dictionary<Tkey, Tvalue>> dic, int idx)
    {
        int list_cnt = dic.Count;
        int dic_cnt = dic[0].Count;

        //for (int i = 0; i < list_cnt; i++)
        {
            m_dic = dic[idx];
            List<Tkey> Key_list = dic[idx].Keys.ToList();
            List<Tvalue> Value_list = dic[idx].Values.ToList();

            for (int j = 0; j < dic_cnt; j++)
            {
                m_key.Add(Key_list[j]);
                m_value.Add(Value_list[j].ToString());
            }

        }

    }



}