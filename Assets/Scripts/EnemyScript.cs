using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public enum EnemyType
    {
        Melee,
        Ranged,
        Boss
    }

    public float health;
    public GameObject targetObject;
    public Transform targetTransform;
    public EnemyType attackType;


    private NavMeshAgent nav;
    // Start is called before the first frame update
    void Start()
    {
        health = 10f;
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Move()
    {


    }
    void Attack()
    {
        switch (attackType)
        {
            case EnemyType.Melee:
                return;
            case EnemyType.Ranged:
                return;
            case EnemyType.Boss:
                return;
                //actually might make a sep boss script as they will prob have mult attks
        }
    }
    void updateTarget(GameObject newTarget)
    {
        targetObject = newTarget;
        targetTransform = newTarget.transform;
    }
    #region
    //messages
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
    #endregion
}

