using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{

    [SerializeField] Texture2D _cursorView;
    [SerializeField] Texture2D _cursor;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver(){
        Cursor.SetCursor(_cursorView, Vector2.zero, CursorMode.ForceSoftware);
    }
    private void OnMouseExit(){
        Cursor.SetCursor(_cursor, Vector2.zero, CursorMode.ForceSoftware);
    }
}
