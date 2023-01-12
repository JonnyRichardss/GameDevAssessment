using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunSpawnScript : MonoBehaviour
{
    public void OnMeleeHit(GameObject sender)
    {
        sender.GetComponent<PlayerController>().shotgun = true;
    }
}
