using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSpawnerScript : MonoBehaviour
{
    public void OnNewPlayer(GameObject player)
    {
        foreach(SpawnPointScript spawner in gameObject.GetComponentsInChildren<SpawnPointScript>())
        {
            spawner.player = player;
            spawner.SendMessage("OnNewPlayer");
        }
    }
}
