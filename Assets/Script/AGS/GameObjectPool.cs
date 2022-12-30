using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : class
{
    int count;
    public delegate T Func();
    Func create_fn;
    // Instances.  
    Stack<T> objects;
    // Construct  
    public GameObjectPool(int count, Func fn)
    {
        this.count = count;
        this.create_fn = fn;
        this.objects = new Stack<T>(this.count);
        allocate();

    }
    void allocate()
    {
        for (int i = 0; i < this.count; ++i)
        {
            this.objects.Push(this.create_fn());
        }
    }
    public T pop()
    {
        if (this.objects.Count <= 0)
        {
            Debug.Log(this + " allocate more");
            allocate();
        }
        return this.objects.Pop();
    }
    public void push(T obj)
    {
        this.objects.Push(obj);
    }
    public int get_Count()
    {
        return objects.Count;
    }
    public Stack<T> get_Stack()
    {
        return objects;
    }

}
