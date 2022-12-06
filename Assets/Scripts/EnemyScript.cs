using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float health;
    // Start is called before the first frame update
    void Start()
    {
        health = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnScannedHit()
    {
        Debug.Log("SCNA");
        Destroy(gameObject);
    }
    void OnProjHit()
    {
        Debug.Log("PROJ");
        Destroy(gameObject);
    }
    void OnMeleeHit()
    {
        Debug.Log("Ow");
        Destroy(gameObject);
    }
}

