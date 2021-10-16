using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tubes : MonoBehaviour
{
    public float speed;
    public float amount;


    private void Start()
    {
        animate();
    }


    private void animate()
    {
        transform.DOMoveY(transform.position.y + amount, speed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}
