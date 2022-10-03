using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMapTool_MaterialList : UIBase
{
    private int _current_select_menu;

    private const int _cube_list = 0;
    private const int _structure_list = 1;

    public Transform tr_parent;
    public Item_Cube source_cube;
    private List<Item_Cube> _list_item_cube = new List<Item_Cube>();

    private Material[] _materials;
    private Material _current_material;

    public RectTransform[] rt_root_tap;
    public RectTransform rt_tap_select;

    private Item_Cube _current_cube;
    public Item_Cube getCurrentCube => _current_cube;

    public void Awake()
    {
        _materials = Resources.LoadAll<Material>("BJYPrefab/Materials");
    }

    public override void initUI()
    {
        base.initUI();

        _current_select_menu = _cube_list;

        source_cube.gameObject.SetActive(false);
    }

    public override void show()
    {
        base.show();

        _setup();
    }

    private void _setup()
    {
        onBtnTapSelect(_current_select_menu);   
    }

    private void _refresh_cube_list()
    {
        for (int i = 0; i < _list_item_cube.Count; i++)
            _list_item_cube[i].gameObject.SetActive(false);

        int j = 0;
        for (int i = 0; i < _materials.Length; i++)
        {
            if (_list_item_cube.Count <= i)
                _list_item_cube.Add(GameObjectCache.Make<Item_Cube>(source_cube, tr_parent));

            _list_item_cube[i].setup(_materials[i]);
            _list_item_cube[i].gameObject.SetActive(true);

            j++;
        }

        for (int i = j; i < _list_item_cube.Count; i++)
        {
            _list_item_cube[i].gameObject.SetActive(false);
        }
    }

    private void _refresh_sturcture_list()
    {
        for (int i = 0; i < _list_item_cube.Count; i++)
            _list_item_cube[i].gameObject.SetActive(false);
    }

    public void onBtnTapSelect(int tap_index)
    {
        switch (tap_index)
        {
            case _cube_list:
                {
                    _current_select_menu = _cube_list;
                    _refresh_cube_list();
                }
                break;

            case _structure_list:
                {
                    _current_select_menu = _structure_list;
                    _refresh_sturcture_list();
                }
                break;
        }

        rt_tap_select.localPosition = rt_root_tap[_current_select_menu].localPosition;
    }

    public void select_cube(Item_Cube item_Cube)
    {
        _current_cube = item_Cube;
        for (int i = 0; i < _list_item_cube.Count; i++)
        {
            _list_item_cube[i].refresh_select(false);
        }

        _current_cube.refresh_select(!_current_cube.img_select.gameObject.activeSelf);
    }

    public void onBtnSelectMaterial()
    {
        if (null != _current_cube)
        {
            UIManager.getinstance<UIMapTool>().select_cube(_current_cube);
            _current_cube = null;
            onBtnClose();
        }
        else
        {
            Debug.LogError("select cube is null");
        }
    }

    public void onBtnClose()
    {
        _current_select_menu = _cube_list;

        hide();
    }
}
