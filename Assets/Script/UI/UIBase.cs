using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public bool	renewLayerIndex = false;
    private Transform _tr_init_parent_layer;
    private int _init_sibling_index;

    public virtual void hide()
    {
        _hide();
    }

    private void _hide()
    {
        if (_tr_init_parent_layer != null && _tr_init_parent_layer != this.transform.parent)
        {
            this.transform.SetParent(_tr_init_parent_layer);
            this.transform.SetSiblingIndex(_init_sibling_index);
        }

        gameObject.SetActive(false);
    }

    public virtual void show()
    {
        if (renewLayerIndex)
        {
            transform.SetAsLastSibling();
        }

        gameObject.SetActive(true);
    }

    public virtual void show(Transform tr_ui)
    {
        this.transform.SetParent(tr_ui);
        this.transform.SetSiblingIndex(tr_ui.GetSiblingIndex() + 1);

        show();
    }

    public virtual void initUI()
    {
        this.gameObject.SetActive(false);

        _tr_init_parent_layer = this.transform.parent;
        _init_sibling_index = this.transform.GetSiblingIndex();
    }
}
