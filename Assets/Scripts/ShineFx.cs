using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShineFx : MonoBehaviour
{
    public Transform Shine;
    public float offset;
    public float Speed;
    public float minDelay;
    public float MaxDelay;

    private void Start()
    {
        animate();
    }

    private void animate()
    {
        Shine.DOLocalMoveX(offset, Speed).SetDelay(Random.Range(minDelay, MaxDelay)).OnComplete(() =>
       {
           Shine.DOLocalMoveX(-offset, 0);
           animate();
       });
    }

}
