using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraFollow : MonoBehaviour
{

    public Transform target;
    private Vector3 offset;
    private bool gameOver;
    


    void Start()
    {
        offset = target.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameOver)
        transform.position = target.position - offset;

        if(transform.position.y < -1)
        {
            transform.position = new Vector3(transform.position.x, -1, transform.position.z);
        }

    }


    public void focusOnWinner(Transform winner)
    {
        gameOver = true;

        target = winner;

        transform.DOMove(target.position - offset, 0.5f);
        Camera.main.fieldOfView = 30;
    }


}
