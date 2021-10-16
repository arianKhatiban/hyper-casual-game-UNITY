using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public Transform[] wayPoint;
    private gamemanager thegamemanager;

    private void Start()
    {
        thegamemanager = FindObjectOfType<gamemanager>();
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            thegamemanager.PlayerInZone.Add(collision.transform);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            thegamemanager.PlayerInZone.Remove(collision.transform);
        }
    }

}
