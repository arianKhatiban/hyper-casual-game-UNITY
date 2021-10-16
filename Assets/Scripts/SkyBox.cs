using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkyBox : MonoBehaviour
{

    public float Speed;

    private void Start()
    {
        animate();
    }


    private void animate()
    {
        transform.DORotate(new Vector3(0, 1, 0), Speed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }


}
