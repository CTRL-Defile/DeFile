using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Basecamp_background : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 초기 설정
        this.transform.GetChild(0).DOScale(1.2f, 0f);

        // ux적용
        this.transform.GetChild(0).DOScale(1f, 4f).SetEase(Ease.OutCubic);
    }

}
