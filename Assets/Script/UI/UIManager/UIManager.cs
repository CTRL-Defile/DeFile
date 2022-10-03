using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static Dictionary<Type, UIBase> _dic_ui_container = new Dictionary<Type, UIBase>();
    private static Dictionary<Type, int> _dic_ui_type = new Dictionary<Type, int>();

    public static UIRoot ui_root { get; private set; }

    public static void init(UIRoot set_ui_root)
    {
        ui_root = set_ui_root;

        UIBase[] child_main = ui_root.tr_ui_root[UIConst.ui_type_main].GetComponentsInChildren<UIBase>(true);
        for (int i = 0; i < child_main.Length; i++)
        {
            child_main[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < UIConst.main_ui_classes.Length; i++)
        {
            Type t = UIConst.main_ui_classes[i];
            _dic_ui_type.Add(t, UIConst.ui_type_main);

            UIBase find_ui = Array.Find(child_main, x => x.GetType() == t);

            if (null != find_ui)
            {
                addUI(t, find_ui);
            }
        }

        UIBase[] child_popup_1 = ui_root.tr_ui_root[UIConst.ui_type_popup_1].GetComponentsInChildren<UIBase>(true);
        for (int i = 0; i < child_popup_1.Length; i++)
        {
            child_popup_1[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < UIConst.popup_1_ui_classes.Length; i++)
        {
            Type t = UIConst.popup_1_ui_classes[i];
            _dic_ui_type.Add(t, UIConst.ui_type_popup_1);

            UIBase find_ui = Array.Find(child_popup_1, x => x.GetType() == t);

            if (null != find_ui)
            {
                addUI(t, find_ui);
            }
        }
    }

    public static void hideAllUI()
    {
        var d_enum = _dic_ui_container.GetEnumerator();
        while (d_enum.MoveNext())
        {
            d_enum.Current.Value.hide();
        }
    }

    public static T getinstance<T>() where T : UIBase
    {
        Type t = typeof(T);
        return (T)_dic_ui_container[t];
    }

    public static void show<T>() where T : UIBase
    {
        Type t = typeof(T);
        _dic_ui_container[t].show();
    }

    public static void hide<T>() where T : UIBase
    {
        Type t = typeof(T);
        _dic_ui_container[t].hide();
    }

    public static bool activeSelf<T>() where T : UIBase
    {
        Type t = typeof(T);
        if (!_dic_ui_container.ContainsKey(t))
        {
            return false;
        }

        return _dic_ui_container[t].gameObject.activeSelf;
    }

    private static void addUI(Type type, UIBase add_ui)
    {
        add_ui.initUI();
        _dic_ui_container.Add(type, add_ui);
    }
}
