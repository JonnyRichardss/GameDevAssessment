using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    public GameObject enemyType;
    private GameObject newEnemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (newEnemy == null)
        {
            newEnemy = Instantiate(enemyType,gameObject.transform.position,gameObject.transform.rotation);
        }
    }
}
