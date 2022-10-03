using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectCache : MonoBehaviour
{
    public class Cache
    {
        public int source_instance_id;
        public string name;
        public int type;
        public Transform root;
        public List<Transform> free_list;
    }

    public class CacheResult
    {
        public bool is_cached;
    }

    private static GameObjectCache _instance;
    public static GameObjectCache instance
    {
        get
        {
            if (_instance == null)
            {
                InitSingleton();
            }

            return _instance;
        }
    }

    private Dictionary<int, Cache> _cache_map;
    private Dictionary<int, Cache> _cache_map_for_instances;
    private Dictionary<int, List<Cache>> _cache_map_type;

    private static void InitSingleton()
    {
        GameObject new_object = new GameObject("GameObjectCache");
        _instance = new_object.AddComponent<GameObjectCache>();
        _instance.Init();
    }

    private void Init()
    {
        _cache_map = new Dictionary<int, Cache>();
        _cache_map_for_instances = new Dictionary<int, Cache>();
        _cache_map_type = new Dictionary<int, List<Cache>>();
    }

    public void ClearAll()
    {
        if (null != _cache_map)
            _cache_map.Clear();

        if (null != _cache_map_for_instances)
            _cache_map_for_instances.Clear();

        if (null != _cache_map_type)
        {
            var d_enum = _cache_map_type.GetEnumerator();
            while (d_enum.MoveNext())
                d_enum.Current.Value.Clear();
            _cache_map_type.Clear();
        }
    }

    void OnDestroy()
    {
        ClearAll();
    }

    public static Transform Make(Transform source, int type = 1, CacheResult cache_result = null)
    {
        Cache cache = PrepareCache(source, type);

        Transform new_transform = null;

        if (cache.free_list.Count <= 0)
        {
            GameObject new_one = (GameObject)Instantiate(source.gameObject);
            new_transform = new_one.transform;
            //			new_one.name = source.GetInstanceID().ToString();
            new_one.gameObject.SetActive(true);

            _instance._cache_map_for_instances.Add(new_one.GetInstanceID(), cache);

            //Debug.Log( string.Format( "create_instance: {0}", source.gameObject.name), new_one);
            if (cache_result != null)
            {
                cache_result.is_cached = false;
            }
        }
        else
        {
            new_transform = cache.free_list[0];
            new_transform.SetParent(null, false);

            cache.free_list.RemoveAt(0);

            Init(new_transform);

            if (cache_result != null)
            {
                cache_result.is_cached = true;
            }
        }

        return new_transform;
    }

    public static typeT Make<typeT>(typeT source, Transform parent, int type = 1, CacheResult cache_result = null) where typeT : Component
    {
        Transform new_t = Make(source.transform, type, cache_result);
        new_t.SetParent(parent, false);
        new_t.gameObject.SetActive(true);

        typeT script = new_t.GetComponent<typeT>();

        return script;
    }

    public static void Init(Transform tr)
    {
        tr.gameObject.SetActive(true);

        tr.BroadcastMessage("Awake", null, SendMessageOptions.DontRequireReceiver);
        tr.BroadcastMessage("Start", null, SendMessageOptions.DontRequireReceiver);
    }

    public static void Delete(Transform tr)
    {
        if (tr == null || tr.gameObject == null)
        {
#if UNITY_EDITOR
            Debug.Log("skip already deleted");
#endif
            return;
        }

        if (_instance == null)
        {
            return;
        }

        tr.gameObject.SetActive(true);
        tr.SendMessage("OnDeleteByGameObjectCache", SendMessageOptions.DontRequireReceiver);

        tr.gameObject.SetActive(false);

        Cache cache = null;
        _instance._cache_map_for_instances.TryGetValue(tr.gameObject.GetInstanceID(), out cache);
        if (cache == null || cache.root == null)
        {
#if UNITY_EDITOR
            Debug.Log("can't find cache", tr.gameObject);
#endif
            GameObject.Destroy(tr.gameObject);

            return;
        }

        tr.SetParent(cache.root, false);

        if (cache.free_list.Contains(tr) == false)
        {
            cache.free_list.Add(tr);
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogWarning("gameobject cache object already deleted", tr.gameObject);
        }
#endif
    }

    private static Cache PrepareCache(Transform source, int type)
    {
        Cache cache = null;
        if (instance._cache_map.TryGetValue(source.GetInstanceID(), out cache) == false)
        {
            GameObject new_go = new GameObject(source.GetInstanceID().ToString());
            new_go.SetActive(false);

            cache = new Cache();
            cache.source_instance_id = source.GetInstanceID();
            cache.name = source.gameObject.name;
            cache.type = type;
            cache.root = new_go.transform;
            cache.root.SetParent(instance.transform, false);
            cache.free_list = new List<Transform>();

            instance._cache_map.Add(source.GetInstanceID(), cache);

            List<Cache> cache_list;
            if (instance._cache_map_type.TryGetValue(type, out cache_list) == false)
            {
                cache_list = new List<Cache>();
                instance._cache_map_type.Add(type, cache_list);
            }

            cache_list.Add(cache);
        }
        return cache;
    }
}
