using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStage : UIBase
{
    public UIStage uIStage;
    public Transform tr_parent;
    public Image img_start;
    public override void initUI()
    {
        base.initUI();
    }

    public override void show()
    {
        base.show();

        _setup();
    }

    private void _setup()
    {
        GameObjectCache.Make<UIStage>(uIStage, tr_parent);
        img_start.sprite = SpriteData.getSprite("activeBG_2");
    }

    public void Update()
    {

    }

    public void onBtnStart()
    {
        hide();
    }

    public void onBtnEnd()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public override void hide()
    {
        base.hide();
    }
}
