using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitCardImage : MonoBehaviour
{
    [SerializeReference]
    [SerializeField] public GameObject StatRef;

    List<Sprite> Imagelist;
    Image unitImage;

    private void Awake()
    {
        Imagelist = StatRef.GetComponent<Battle_Status_Script>().Imagelist;
        unitImage = GetComponent<Image>();
    }


    // Start is called before the first frame update
    public void ChangeImage(int unitID)
    {
        unitImage.sprite = Imagelist[unitID/3];
    }

}
