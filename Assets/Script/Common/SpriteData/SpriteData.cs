using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;

public static class SpriteData
{
    public static Sprite getSprite(string file_name, string folder_name = "")
    {
        if (string.IsNullOrEmpty(file_name))
        {
            return null;
        }

        Sprite sprite = null;
    
        if (string.IsNullOrEmpty(folder_name))
        {
            sprite = Resources.Load<Sprite>("Texture/" + file_name) as Sprite;
        }
        else
        {
            sprite = Resources.Load<Sprite>("Texture/" + folder_name + "/" + file_name) as Sprite;
        }

        return sprite;
    }
}
