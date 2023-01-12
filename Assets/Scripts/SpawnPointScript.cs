using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    public GameObject enemyType;
    public GameObject player;
    private GameObject newEnemy;
    private EnemyScript newScript;
    private List<GameObject> enemies = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (enemies.Count <=5)
        {
            newEnemy = Instantiate(enemyType,gameObject.transform.position,gameObject.transform.rotation);
            newScript = newEnemy.GetComponent<EnemyScript>();
            newScript.targetObject = player;
            enemies.Add(newEnemy);
        }
        foreach(GameObject enemy in enemies)
        {
            if (enemy == null)
            {
                enemies.Remove(enemy);
            }
        }
    }
}
