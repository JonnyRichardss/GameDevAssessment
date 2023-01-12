using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDoorScript : MonoBehaviour
{
    private float doorTimer =0;
    private Transform previousDoor;
    void Update()
    {
        if (doorTimer <= 0)
        {
            if (previousDoor != null) { previousDoor.localPosition -= Vector3.up * 5f; }
            

            previousDoor = transform.GetChild(Mathf.RoundToInt(Random.Range(0, 11)));
            previousDoor.localPosition += Vector3.up * 5f;
            doorTimer = 5f;
        }
        else
        {
            doorTimer -= Time.deltaTime;
        }
    }
}
