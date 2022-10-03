using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapTool : UIBase
{
    public Transform tr_popup_menus;

    private Material[] _materials;
    private Material _current_material;
    private int _material_index;

    private Item_Cube _current_select_cube;
    public Item_Cube getSelectCube => _current_select_cube;
    public MeshRenderer cube_mesh;

    public float posZ;

    public Camera MainCamera;

    public void Awake()
    {
        _material_index = -1;
        _materials = Resources.LoadAll<Material>("SkyBox");
    }

    public override void show()
    {
        base.show();

        _setup();
    }

    private void _setup()
    {
        tr_popup_menus.gameObject.SetActive(false);
        cube_mesh.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (cube_mesh.gameObject.activeSelf)
        {
            Vector3 p = MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -200f));


            Vector3 vec3 = Input.mousePosition;
            Vector3 transpos = new Vector3(Camera.main.ScreenToWorldPoint(vec3).x, Camera.main.ScreenToWorldPoint(vec3).y, (Camera.main.ScreenToWorldPoint(vec3).z + posZ));
            
            cube_mesh.transform.position = p;
        }
    }

    public void onBtnShowMenu()
    {
        bool active = !tr_popup_menus.gameObject.activeSelf;
        tr_popup_menus.gameObject.SetActive(active);
    }

    public void onBtnCreateCube()
    {
        UIManager.getinstance<UIMapTool_MaterialList>().show();
        _current_select_cube = null;
        cube_mesh.gameObject.SetActive(false);
    }

    public void select_cube(Item_Cube item_Cube)
    {
        _current_select_cube = item_Cube;
        cube_mesh.gameObject.SetActive(true);
        cube_mesh.material = item_Cube.getMaterial;
    }

    public void onBtnChangeSkyBox()
    {
        _material_index++;
        if (_materials.Length <= _material_index)
        {
            _material_index = 0;
        }
        RenderSettings.skybox = _materials[_material_index];
    }
}
