using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchTriger : MonoBehaviour
{
    public Player PlayerScript;



    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript.Punch(other.transform);
        }  
    }


}
