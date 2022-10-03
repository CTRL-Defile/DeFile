using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControlMapTool : MonoBehaviour
{
    private static MainControlMapTool _instance;
    public static MainControlMapTool instance => _instance;

    public Camera main_camera;

    public UIRoot ui_root;

    private bool _camera_move;

    private float _xmove = 0f;
    private float _ymove = 0f;

    public void Awake()
    {
        _instance = this;
        _camera_move = true;

        ui_root.Init();

        UIManager.hideAllUI();

        _init();

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _camera_move = !_camera_move;

            if (_camera_move)
            {
                Cursor.lockState = CursorLockMode.Locked;
                UIManager.getinstance<UIMapTool>().cube_mesh.gameObject.SetActive(false);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                if (null != UIManager.getinstance<UIMapTool>().getSelectCube)
                    UIManager.getinstance<UIMapTool>().cube_mesh.gameObject.SetActive(true);
            }
        }    

        if (_camera_move && !UIManager.activeSelf<UIMapTool_MaterialList>())
        {
            
            _xmove += Input.GetAxis("Mouse X");
            _ymove -= Input.GetAxis("Mouse Y");
            main_camera.transform.rotation = Quaternion.Euler(_ymove, _xmove, 0);

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.W))
            {
                Vector3 dir = Vector3.right * h + Vector3.forward * v;
                dir = Camera.main.transform.TransformDirection(dir);
                dir.Normalize();
                main_camera.transform.position += dir * 10f * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                Vector3 dir = Vector3.right * h + Vector3.back * v;
                dir = Camera.main.transform.TransformDirection(dir);
                dir.Normalize();
                main_camera.transform.position -= dir * 10f * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                Vector3 dir = Vector3.right * h + Vector3.left * v;
                dir = Camera.main.transform.TransformDirection(dir);
                dir.Normalize();
                main_camera.transform.position += dir * 10f * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Vector3 dir = Vector3.right * h + Vector3.right * v;
                dir = Camera.main.transform.TransformDirection(dir);
                dir.Normalize();
                main_camera.transform.position += dir * 10f * Time.deltaTime;
            }

            if (main_camera.transform.position.y < 1)
            {
                main_camera.transform.position = new Vector3(main_camera.transform.position.x, 1, main_camera.transform.position.z);
            }

            // if (Input.GetButtonDown("Fire1"))
            // {
            //     Vector3 mousePos = Input.mousePosition;
            //     {
            //         Debug.LogError(mousePos.x);
            //         Debug.LogError(mousePos.y);
            //     }
            // }
        }
    }

    private void _init()
    {
        UIManager.init(ui_root);

        UIManager.show<UIMapTool>();
    }

    public void OnDestory()
    {
        _instance = null;
    }
}
